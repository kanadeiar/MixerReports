﻿using System;
using System.Diagnostics;
using MixerReports.lib.Interfaces;
using MixerReports.lib.Models;
using Sharp7;

namespace MixerReports.lib.Services
{
    /// <summary> Сервис получения информации по заливкам из контроллера </summary>
    public class Sharp7MixReaderService : ISharp7MixReaderService
    {
        private readonly S7Client _S7Client;
        private readonly LocalDataStorage _storage;

        private readonly string _address; //адрес контроллера
        private int _aluminiumProp; //пропорция алюминия к воде
        private int _seconds; //уточнение времени

        private Mix _tempMix; //временная информация по заливке на время уточнения данных

        public Sharp7MixReaderService(string address = "10.0.57.10", int aluminiumProp = 20, int seconds = 0)
        {
            _S7Client = new S7Client {ConnTimeout = 5_000, RecvTimeout = 5_000};
            _storage = new LocalDataStorage(_S7Client, aluminiumProp, seconds);
            _address = address;
            _aluminiumProp = aluminiumProp;
            _seconds = seconds;
        }

        /// <summary> Тест соединения с контроллером </summary>
        public bool TestConnection(out int error)
        {
            var result = _S7Client.ConnectTo(_address, 0, 2);
            _S7Client.Disconnect();
            if (result != 0)
            {
                error = result;
                return false;
            }
            error = 0;
            return true;
        }
        /// <summary> Проба получения новых данных заливки с контроллера, вызов с таймера </summary>
        /// <param name="secondsBegin">секунд продолжается заливка</param>
        /// <param name="error">ошибка чтения ПЛК</param>
        /// <param name="mix">данные заливки</param>
        /// <returns>Есть новые данные по заливке</returns>
        public bool TryNewMixTick(out int secondsBegin, out int error, out Mix mix)
        {
            var result = false;
            secondsBegin = 0;
            error = 0;
            mix = null;
            var r = _S7Client.ConnectTo(_address, 0, 2);
            if (r == 0)
            {
                var mixRunning = NowMixRunning(out secondsBegin, out bool frontBegin, out bool rearEnd);
                
                _storage.UpdateData(mixRunning);

                if (frontBegin)
                {
                    _tempMix = _storage.GetData(); //получаем новые данные заливки
                }
                if (rearEnd)
                {
                    _storage.CorrectData(ref _tempMix); //получение скорректированных данных
                    mix = _tempMix; //вертаем новые уточненные данные
                    result = true;
                }
            }
            else
            {
                error = r;
                result = false;
            }
            _S7Client.Disconnect();
            DebugDatas(_tempMix);
            return result;
        }
        private static void DebugDatas(Mix row)
        {
            if (row is null)
                return;
            Debug.WriteLine($" {row.DateTime:g} {row.Number} Форма №{row.FormNumber} {row.MixerTemperature}, " +
                            $"обратный шлам: {row.SetRevertMud} {row.ActRevertMud} песчаный шлам: {row.SetSandMud} {row.ActSandMud} " +
                            $"холодная вода: {row.SetColdWater} {row.ActColdWater} горячая вода {row.SetHotWater} {row.ActHotWater} " +
                            $"ипв1: {row.SetMixture1} {row.ActMixture1} ипв2: {row.SetMixture2} {row.ActMixture2} " +
                            $"цемент1: {row.SetCement1} {row.ActCement1} цемент2: {row.SetCement2} {row.ActCement2} " +
                            $"алюминий1: {row.SetAluminium1} {row.ActAluminium1} алюминий2: {row.SetAluminium2} {row.ActAluminium2} песок в шламе: {row.SandInMud}" +
                            $"норма: {row.Normal} комментарий: {row.Comment}");
        }


        /// <summary> Состояние текущей заливки </summary>
        /// <param name="secondsBegin">сколько секунд идет</param>
        /// <param name="frontBegin">начало</param>
        /// <param name="rearEnd">конец</param>
        /// <returns>продолжается</returns>
        private bool NowMixRunning(out int secondsBegin, out bool frontBegin, out bool rearEnd)
        {
            byte[] buffer = new byte[4];
            _S7Client.ReadArea(S7Area.MK, 0, 3330, 1, S7WordLength.Byte, buffer);
            var result = (buffer[0] & 0b1) != 0; //идет заливка
            frontBegin = result && !_edgeMixRunning; //новая заливка началась
            rearEnd = !result && _edgeMixRunning; //новая заливка закончилась
            _edgeMixRunning = result;
            _S7Client.DBRead(314, 100, 4, buffer);
            secondsBegin = buffer.GetDIntAt(0); //прошло секунд заливки
            return result;
        }
        private bool _edgeMixRunning; //звонок для переменной

        /// <summary> Промежуточное локальное хранилище данных по заливкам </summary>
        private class LocalDataStorage
        {
            private bool _enableData; //есть данные в хранилище, не нужно получить при первой же возможности
            private Mix _mix; //данные
            private S7Client _S7Client;
            private int _aluminiumProp; //пропорция алюминия к воде
            private int _seconds; //уточнение времени
            //сигналы появления новых значений
            private SignalReadValue _signalRevertMud = new (454,0);
            private SignalReadValue _signalSandMud = new(452, 6);
            private SignalReadValue _signalColdWater = new(453,6);
            private SignalReadValue _signalHotWater = new(453, 4);
            private SignalReadValue _signalMixture1 = new(452,0);
            private SignalReadValue _signalMixture2 = new(454, 4);
            private SignalReadValue _signalCement1 = new(452, 4);
            private SignalReadValue _signalCement2 = new(454, 6);
            private SignalReadValue _signalAluminium1 = new(453, 0);
            private SignalReadValue _signalAluminium2 = new(453, 2);
            public LocalDataStorage(S7Client s7Client, int aluminiumProp, int seconds)
            {
                _S7Client = s7Client;
                _mix = new Mix();
                _aluminiumProp = aluminiumProp;
                _seconds = seconds;
            }
            /// <summary> Обновление данных </summary>
            public void UpdateData(bool mixRunning)
            {
                byte[] bufferDb = new byte[455];
                _S7Client.DBRead(401, 0, 455, bufferDb);

                //Debug.WriteLine($"Data 451: {bufferDb.GetByteAt(451)} Data 452: {bufferDb.GetByteAt(452)} Data 453: {bufferDb.GetByteAt(453)} Data 454: {bufferDb.GetByteAt(454)}");

                if (!_enableData && mixRunning)
                {
                    _mix.SetRevertMud = bufferDb.GetDIntAt(170) / 100.0f;
                    _mix.ActRevertMud = bufferDb.GetDIntAt(174) / 100.0f;
                    _mix.SetSandMud = bufferDb.GetDIntAt(178) / 100.0f;
                    _mix.ActSandMud = bufferDb.GetDIntAt(182) / 100.0f;
                    _mix.SetColdWater = bufferDb.GetDIntAt(186) / 100.0f;
                    _mix.ActColdWater = bufferDb.GetDIntAt(190) / 100.0f;
                    _mix.SetHotWater = bufferDb.GetDIntAt(194) / 100.0f;
                    _mix.ActHotWater = bufferDb.GetDIntAt(198) / 100.0f;
                    _mix.SetMixture1 = bufferDb.GetDIntAt(202) / 100.0f;
                    _mix.ActMixture1 = bufferDb.GetDIntAt(206) / 100.0f;
                    _mix.SetMixture2 = bufferDb.GetDIntAt(210) / 100.0f;
                    _mix.ActMixture2 = bufferDb.GetDIntAt(214) / 100.0f;
                    _mix.SetCement1 = bufferDb.GetDIntAt(234) / 100.0f;
                    _mix.ActCement1 = bufferDb.GetDIntAt(238) / 100.0f;
                    _mix.SetCement2 = bufferDb.GetDIntAt(242) / 100.0f;
                    _mix.ActCement2 = bufferDb.GetDIntAt(246) / 100.0f;
                    _mix.SetAluminium1 = bufferDb.GetDIntAt(250) / 100.0f / (_aluminiumProp + 1);
                    _mix.ActAluminium1 = bufferDb.GetDIntAt(254) / 100.0f / (_aluminiumProp + 1);
                    _mix.SetAluminium2 = bufferDb.GetDIntAt(258) / 100.0f / (_aluminiumProp + 1);
                    _mix.ActAluminium2 = bufferDb.GetDIntAt(262) / 100.0f / (_aluminiumProp + 1);
                    _enableData = true;
                    return;
                }

                //сигналы появления новых значений - прочитать и запомнить данные
                if (_signalRevertMud.GetSignalReadValue(ref bufferDb))
                {
                    _mix.SetRevertMud = bufferDb.GetDIntAt(170) / 100.0f;
                    _mix.ActRevertMud = bufferDb.GetDIntAt(174) / 100.0f;
                    Debug.Write($"ОШ: {_mix.SetRevertMud} {_mix.ActRevertMud} ");
                }
                if (_signalSandMud.GetSignalReadValue(ref bufferDb))
                {
                    _mix.SetSandMud = bufferDb.GetDIntAt(178) / 100.0f;
                    _mix.ActSandMud = bufferDb.GetDIntAt(182) / 100.0f;
                    Debug.Write($"ПШ: {_mix.SetSandMud} {_mix.ActSandMud} ");
                }
                if (_signalColdWater.GetSignalReadValue(ref bufferDb))
                {
                    _mix.SetColdWater = bufferDb.GetDIntAt(186) / 100.0f;
                    _mix.ActColdWater = bufferDb.GetDIntAt(190) / 100.0f;
                    Debug.Write($"холодной воды: {_mix.SetColdWater} {_mix.ActColdWater} ");
                }
                if (_signalHotWater.GetSignalReadValue(ref bufferDb))
                {
                    _mix.SetHotWater = bufferDb.GetDIntAt(194) / 100.0f;
                    _mix.ActHotWater = bufferDb.GetDIntAt(198) / 100.0f;
                    Debug.Write($"горячей воды: {_mix.SetHotWater} {_mix.ActHotWater} ");
                }
                if (_signalMixture1.GetSignalReadValue(ref bufferDb))
                {
                    _mix.SetMixture1 = bufferDb.GetDIntAt(202) / 100.0f;
                    _mix.ActMixture1 = bufferDb.GetDIntAt(206) / 100.0f;
                    Debug.Write($"ИПВ1: {_mix.SetMixture1} {_mix.ActMixture1} ");
                }
                if (_signalMixture2.GetSignalReadValue(ref bufferDb))
                {
                    _mix.SetMixture2 = bufferDb.GetDIntAt(210) / 100.0f;
                    _mix.ActMixture2 = bufferDb.GetDIntAt(214) / 100.0f;
                    Debug.Write($"ИПВ2: {_mix.SetMixture2} {_mix.ActMixture2} ");
                }
                if (_signalCement1.GetSignalReadValue(ref bufferDb))
                {
                    _mix.SetCement1 = bufferDb.GetDIntAt(234) / 100.0f;
                    _mix.ActCement1 = bufferDb.GetDIntAt(238) / 100.0f;
                    Debug.Write($"цемента1: {_mix.SetCement1} {_mix.ActCement1} ");
                }
                if (_signalCement2.GetSignalReadValue(ref bufferDb))
                {
                    _mix.SetCement2 = bufferDb.GetDIntAt(242) / 100.0f;
                    _mix.ActCement2 = bufferDb.GetDIntAt(246) / 100.0f;
                    Debug.Write($"цемента2: {_mix.SetCement2} {_mix.ActCement2} ");
                }
                if (_signalAluminium1.GetSignalReadValue(ref bufferDb))
                {
                    _mix.SetAluminium1 = bufferDb.GetDIntAt(250) / 100.0f / (_aluminiumProp + 1);
                    _mix.ActAluminium1 = bufferDb.GetDIntAt(254) / 100.0f / (_aluminiumProp + 1);
                    Debug.Write($"алюминия1: {_mix.SetAluminium1} {_mix.ActAluminium1} ");
                }
                if (_signalAluminium2.GetSignalReadValue(ref bufferDb))
                {
                    _mix.SetAluminium2 = bufferDb.GetDIntAt(258) / 100.0f / (_aluminiumProp + 1);
                    _mix.ActAluminium2 = bufferDb.GetDIntAt(262) / 100.0f / (_aluminiumProp + 1);
                    Debug.Write($"алюминия2: {_mix.SetAluminium2} {_mix.ActAluminium2} ");
                }
                Debug.WriteLine("");
            }



            /// <summary> Корректирование данных </summary>
            public void CorrectData(ref Mix mix)
            {
                byte[] bufferDb = new byte[454];
                _S7Client.DBRead(401, 0, 454, bufferDb);

                mix.Number = 1;
                mix.DateTime = DateTime.Now.AddSeconds(_seconds);
                mix.FormNumber = bufferDb.GetIntAt(0);
                mix.MixerTemperature = bufferDb.GetIntAt(6) / 10.0f;
                var densSand = bufferDb.GetIntAt(284);
                var sandInMud = 0.0f;
                if (densSand >= 900 && densSand <= 2000)
                    sandInMud = (bufferDb.GetDIntAt(182) / 100.0f / densSand) * 1110.0f * 1.0075f;
                mix.SandInMud = sandInMud;
                mix.DensitySandMud = bufferDb.GetIntAt(284) / 1000.0f;
                mix.DensityRevertMud = bufferDb.GetIntAt(282) / 1000.0f;
                mix.Normal = true;
            }
            /// <summary> Выдача данных </summary>
            public Mix GetData()
            {
                var result = new Mix
                {
                    SetRevertMud = _mix.SetRevertMud,
                    ActRevertMud = _mix.ActRevertMud,
                    SetSandMud = _mix.SetSandMud,
                    ActSandMud = _mix.ActSandMud,
                    SetColdWater = _mix.SetColdWater,
                    ActColdWater = _mix.ActColdWater,
                    SetHotWater = _mix.SetHotWater,
                    ActHotWater = _mix.ActHotWater,
                    SetMixture1 = _mix.SetMixture1,
                    ActMixture1 = _mix.ActMixture1,
                    SetMixture2 = _mix.SetMixture2,
                    ActMixture2 = _mix.ActMixture2,
                    SetCement1 = _mix.SetCement1,
                    ActCement1 = _mix.ActCement1,
                    SetCement2 = _mix.SetCement2,
                    ActCement2 = _mix.ActCement2,
                    SetAluminium1 = _mix.SetAluminium1,
                    ActAluminium1 = _mix.ActAluminium1,
                    SetAluminium2 = _mix.SetAluminium2,
                    ActAluminium2 = _mix.ActAluminium2,
                };
                _mix.SetRevertMud = 0.0f;
                _mix.ActRevertMud = 0.0f;
                _mix.SetSandMud = 0.0f;
                _mix.ActSandMud = 0.0f;
                _mix.SetColdWater = 0.0f;
                _mix.ActColdWater = 0.0f;
                _mix.SetHotWater = 0.0f;
                _mix.ActHotWater = 0.0f;
                _mix.SetMixture1 = 0.0f;
                _mix.ActMixture1 = 0.0f;
                _mix.SetMixture2 = 0.0f;
                _mix.ActMixture2 = 0.0f;
                _mix.SetCement1 = 0.0f;
                _mix.ActCement1 = 0.0f;
                _mix.SetCement2 = 0.0f;
                _mix.ActCement2 = 0.0f;
                _mix.SetAluminium1 = 0.0f;
                _mix.ActAluminium1 = 0.0f;
                _mix.SetAluminium2 = 0.0f;
                _mix.ActAluminium2 = 0.0f;
                return result;
            }

            private class SignalReadValue
            {
                private readonly int _numByte;
                private readonly int _numBit;
                //private bool _doneRead;

                public SignalReadValue(int numByte, int numBit)
                {
                    _numByte = numByte;
                    _numBit = numBit;
                }
                public bool GetSignalReadValue(ref byte[] bufferDb)
                {
                    //var readValue = bufferDb.GetBitAt(_numByte, _numBit);
                    //var result = readValue && !_doneRead;
                    //_doneRead = readValue;
                    //return result;
                    return bufferDb.GetBitAt(_numByte, _numBit);
                }
            }

        }
    }
}
