using System;
using System.Diagnostics;
using System.Text;
using MixerReports.lib.Models;
using Sharp7;

namespace MixerReports.lib.Services.Tools
{
    public class BufferedPLCConnector
    {
        private readonly S7Client _client;
        private readonly int _aluminiumProp;
        private readonly int _seconds;

        private Mix _mixBuffer;
        private byte[] _bufferDb;
        private Mix _mix;
        private bool _edgeMixRunning;
        public BufferedPLCConnector(S7Client client, int aluminiumProp, int seconds)
        {
            _client = client;
            _aluminiumProp = aluminiumProp;
            _seconds = seconds;
            _bufferDb = new byte[455];
        }
        /// <summary> Получить новые данные из блока данных из контроллера, всегда его вызывать по тику </summary>
        /// <returns>Данные получены</returns>
        public void GetNewDataFromPLCToBuffetDbTick()
        {
            var r = _client.DBRead(401, 0, 455, _bufferDb);
            if (r != 0)
                throw new ApplicationException(
                    $"Ошибка обновления данных внутреннего буфера.\nНе удалось прочитать блок данных 401 с контроллера с адресом {_client.PLCIpAddress}, ошибка = {r}");
        }
        /// <summary> Обновление данных во внутреннем буфере </summary>
        /// <param name="begin">сигнал что идет процесс заливки, смешивание</param>
        /// <returns>количество обновленных данных за этот раз</returns>
        public int UpdateBuffer(bool begin)
        {
            var count = 0;
            if (_mixBuffer is null)
            {
                if (begin)
                {
                    _mixBuffer = new Mix
                    {
                        #region Запомнить текущие данные уже ведущейся заливки - т.е. приложение запустили во время ведения заливки

                        SetRevertMud = _bufferDb.GetDIntAt(170) / 100.0f,
                        ActRevertMud = _bufferDb.GetDIntAt(174) / 100.0f,
                        SetSandMud = _bufferDb.GetDIntAt(178) / 100.0f,
                        ActSandMud = _bufferDb.GetDIntAt(182) / 100.0f,
                        SetColdWater = _bufferDb.GetDIntAt(186) / 100.0f,
                        ActColdWater = _bufferDb.GetDIntAt(190) / 100.0f,
                        SetHotWater = _bufferDb.GetDIntAt(194) / 100.0f,
                        ActHotWater = _bufferDb.GetDIntAt(198) / 100.0f,
                        SetMixture1 = _bufferDb.GetDIntAt(202) / 100.0f,
                        ActMixture1 = _bufferDb.GetDIntAt(206) / 100.0f,
                        SetMixture2 = _bufferDb.GetDIntAt(210) / 100.0f,
                        ActMixture2 = _bufferDb.GetDIntAt(214) / 100.0f,
                        SetCement1 = _bufferDb.GetDIntAt(234) / 100.0f,
                        ActCement1 = _bufferDb.GetDIntAt(238) / 100.0f,
                        SetCement2 = _bufferDb.GetDIntAt(242) / 100.0f,
                        ActCement2 = _bufferDb.GetDIntAt(246) / 100.0f,
                        SetAluminium1 = _bufferDb.GetDIntAt(250) / 100.0f / (_aluminiumProp + 1),
                        ActAluminium1 = _bufferDb.GetDIntAt(254) / 100.0f / (_aluminiumProp + 1),
                        SetAluminium2 = _bufferDb.GetDIntAt(258) / 100.0f / (_aluminiumProp + 1),
                        ActAluminium2 = _bufferDb.GetDIntAt(262) / 100.0f / (_aluminiumProp + 1),

                        #endregion
                    };
                    return 10;
                }
                _mixBuffer = new Mix();
            }
            #region Обновление данных в буфере на основе сигналов дозирования с контроллера

            StringBuilder sb = new StringBuilder();
            if (GetSignalFromBit(ref _bufferDb, 454, 0))
            {
                _mixBuffer.SetRevertMud = _bufferDb.GetDIntAt(170) / 100.0f;
                _mixBuffer.ActRevertMud = _bufferDb.GetDIntAt(174) / 100.0f;
                count++;
                sb.Append($"ОШ: {_mixBuffer.SetRevertMud} {_mixBuffer.ActRevertMud} ");
            }
            if (GetSignalFromBit(ref _bufferDb, 452, 6))
            {
                _mixBuffer.SetSandMud = _bufferDb.GetDIntAt(178) / 100.0f;
                _mixBuffer.ActSandMud = _bufferDb.GetDIntAt(182) / 100.0f;
                count++;
                sb.Append($"ПШ: {_mixBuffer.SetSandMud} {_mixBuffer.ActSandMud} ");
            }
            if (GetSignalFromBit(ref _bufferDb, 453, 6))
            {
                _mixBuffer.SetColdWater = _bufferDb.GetDIntAt(186) / 100.0f;
                _mixBuffer.ActColdWater = _bufferDb.GetDIntAt(190) / 100.0f;
                count++;
                sb.Append($"холодной воды: {_mixBuffer.SetColdWater} {_mixBuffer.ActColdWater} ");
            }
            if (GetSignalFromBit(ref _bufferDb, 453, 4))
            {
                _mixBuffer.SetHotWater = _bufferDb.GetDIntAt(194) / 100.0f;
                _mixBuffer.ActHotWater = _bufferDb.GetDIntAt(198) / 100.0f;
                count++;
                sb.Append($"горячей воды: {_mixBuffer.SetHotWater} {_mixBuffer.ActHotWater} ");
            }
            if (GetSignalFromBit(ref _bufferDb, 452, 0))
            {
                _mixBuffer.SetMixture1 = _bufferDb.GetDIntAt(202) / 100.0f;
                _mixBuffer.ActMixture1 = _bufferDb.GetDIntAt(206) / 100.0f;
                count++;
                sb.Append($"ИПВ1: {_mixBuffer.SetMixture1} {_mixBuffer.ActMixture1} ");
            }
            if (GetSignalFromBit(ref _bufferDb, 454, 4))
            {
                _mixBuffer.SetMixture2 = _bufferDb.GetDIntAt(210) / 100.0f;
                _mixBuffer.ActMixture2 = _bufferDb.GetDIntAt(214) / 100.0f;
                count++;
                sb.Append($"ИПВ2: {_mixBuffer.SetMixture2} {_mixBuffer.ActMixture2} ");
            }
            if (GetSignalFromBit(ref _bufferDb, 452, 4))
            {
                _mixBuffer.SetCement1 = _bufferDb.GetDIntAt(234) / 100.0f;
                _mixBuffer.ActCement1 = _bufferDb.GetDIntAt(238) / 100.0f;
                count++;
                sb.Append($"цемента1: {_mixBuffer.SetCement1} {_mixBuffer.ActCement1} ");
            }
            if (GetSignalFromBit(ref _bufferDb, 454, 6))
            {
                _mixBuffer.SetCement2 = _bufferDb.GetDIntAt(242) / 100.0f;
                _mixBuffer.ActCement2 = _bufferDb.GetDIntAt(246) / 100.0f;
                count++;
                sb.Append($"цемента2: {_mixBuffer.SetCement2} {_mixBuffer.ActCement2} ");
            }
            if (GetSignalFromBit(ref _bufferDb, 453, 0) || GetSignalFromBit(ref _bufferDb, 453, 2))
            {
                _mixBuffer.SetAluminium1 = _bufferDb.GetDIntAt(250) / 100.0f / (_aluminiumProp + 1);
                _mixBuffer.ActAluminium1 = _bufferDb.GetDIntAt(254) / 100.0f / (_aluminiumProp + 1);
                count++;
                sb.Append($"алюминия1: {_mixBuffer.SetAluminium1} {_mixBuffer.ActAluminium1} ");
                _mixBuffer.SetAluminium2 = _bufferDb.GetDIntAt(258) / 100.0f / (_aluminiumProp + 1);
                _mixBuffer.ActAluminium2 = _bufferDb.GetDIntAt(262) / 100.0f / (_aluminiumProp + 1);
                count++;
                sb.Append($"алюминия2: {_mixBuffer.SetAluminium2} {_mixBuffer.ActAluminium2} ");
            }
            if (sb.Length > 0)
                Debug.WriteLine(sb.ToString());

            #endregion
            return count;

            static bool GetSignalFromBit(ref byte[] bufferDb, int pos, int bit) => 
                bufferDb.GetBitAt(pos, bit);
        }
        /// <summary> Очистка буфера внутреннего </summary>
        public void ClearBuffer()
        {
            _mixBuffer = new Mix();
        }
        /// <summary> Получение данных новой заливки из внутреннего буфера и контроллера в новую заливку  </summary>
        public bool SetNewMixFromBuffer()
        {
            var densSand = _bufferDb.GetIntAt(284);
            var densRevert = _bufferDb.GetIntAt(282);
            var sandInMud = 0.0f;
            if (densSand >= 900 && densSand <= 2000)
                sandInMud = (_bufferDb.GetDIntAt(182) / 100.0f / densSand) * 1110.0f * 1.0075f;
            _mix = new Mix
            {
                #region Копирование данных из буфера в новую заливку

                Number = 1,
                FormNumber = _bufferDb.GetIntAt(0),
                SetRevertMud = _mixBuffer.SetRevertMud,
                ActRevertMud = _mixBuffer.ActRevertMud,
                SetSandMud = _mixBuffer.SetSandMud,
                ActSandMud = _mixBuffer.ActSandMud,
                SetColdWater = _mixBuffer.SetColdWater,
                ActColdWater = _mixBuffer.ActColdWater,
                SetHotWater = _mixBuffer.SetHotWater,
                ActHotWater = _mixBuffer.ActHotWater,
                SetMixture1 = _mixBuffer.SetMixture1,
                ActMixture1 = _mixBuffer.ActMixture1,
                SetMixture2 = _mixBuffer.SetMixture2,
                ActMixture2 = _mixBuffer.ActMixture2,
                SetCement1 = _mixBuffer.SetCement1,
                ActCement1 = _mixBuffer.ActCement1,
                SetCement2 = _mixBuffer.SetCement2,
                ActCement2 = _mixBuffer.ActCement2,
                SetAluminium1 = _mixBuffer.SetAluminium1,
                ActAluminium1 = _mixBuffer.ActAluminium1,
                SetAluminium2 = _mixBuffer.SetAluminium2,
                ActAluminium2 = _mixBuffer.ActAluminium2,
                SandInMud = sandInMud,
                DensitySandMud = densSand / 1000.0f,
                DensityRevertMud = densRevert / 1000.0f,
                Normal = true,
                
                #endregion
            };
            Debug.WriteLine($"Копирование данных из буфера в новую заливку: {_mix.FormNumber} обратный шлам: {_mix.SetRevertMud} {_mix.ActRevertMud} песчаный шлам: {_mix.SetSandMud} {_mix.ActSandMud} холодная вода: {_mix.SetColdWater} {_mix.ActColdWater} горячая вода: {_mix.SetHotWater} {_mix.ActHotWater} ипв1: {_mix.SetMixture1} {_mix.ActMixture1} ипв2: {_mix.SetMixture2} {_mix.ActMixture2} \n" +
                            $"    цемент1: {_mix.SetCement1} {_mix.ActCement1} цемент2: {_mix.SetCement2} {_mix.ActCement2} алюминий1: {_mix.SetAluminium1} {_mix.ActAluminium1} алюминий2: {_mix.SetAluminium2} {_mix.ActAluminium2} песок в шламе: {_mix.SandInMud} плотность песок: {_mix.DensitySandMud} плотность шлам: {_mix.DensityRevertMud} норма: {_mix.NormalStr}");
            return true;
        }
        /// <summary> Получение уточненных по дате и температуре данных по заливке, на основе ранее скопированых данных  </summary>
        /// <returns>Новая заливка с уточнением даты, температуры</returns>
        public Mix GetUpdatedMix()
        {
            if (_mix is null)
                return null;
            _mix.DateTime = DateTime.Now.AddSeconds(_seconds);
            _mix.MixerTemperature = _bufferDb.GetIntAt(6) / 10.0f;
            return _mix;
        }

        /// <summary> Состояние текущей заливки </summary>
        /// <param name="secondsBegin">сколько секунд идет</param>
        /// <param name="frontBegin">начало</param>
        /// <param name="rearEnd">конец</param>
        /// <returns>продолжается</returns>
        public bool NowMixRunning(out int secondsBegin, out bool begin, out bool end)
        {
            byte[] buffer = new byte[4];
            _client.ReadArea(S7Area.MK, 0, 3330, 1, S7WordLength.Byte, buffer);
            var result = (buffer[0] & 0b1) != 0; //идет заливка
            begin = result && !_edgeMixRunning; //новая заливка началась
            end = !result && _edgeMixRunning; //новая заливка закончилась
            _edgeMixRunning = result;
            _client.DBRead(314, 100, 4, buffer);
            secondsBegin = buffer.GetDIntAt(0); //прошло секунд заливки
            return result;
        }
    }
}
