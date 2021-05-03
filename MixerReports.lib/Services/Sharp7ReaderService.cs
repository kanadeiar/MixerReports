using System;
using System.Net.Sockets;
using MixerReports.lib.Interfaces;
using MixerReports.lib.Models;
using Sharp7;

namespace MixerReports.lib.Services
{
    public class Sharp7ReaderService : ISharp7ReaderService
    {
        private S7Client _S7Client;

        private bool _nowMixRunning = false; //идет заливка
        private bool _edgeMixRunning = false; //звонок новой заливки

        /// <summary> Адрес контроллера </summary>
        public string Address { get; set; } = "10.0.57.10";
        /// <summary> Уставка срабатывания чтения новой заливки </summary>
        public int SetSecondsToRead { get; set; } = 220;
        /// <summary> Пропорция алюминий к воде </summary>
        public int AluminiumProp { get; set; } = 20;
        /// <summary> Корректировка временная каждой заливки </summary>
        public int SecondsCorrect { get; set; } = - 10;

        public Sharp7ReaderService()
        {
            _S7Client = new S7Client();
        }

        /// <summary> Тест соединения с контроллером </summary>
        public bool TestConnection(out int error)
        {
            var result = _S7Client.ConnectTo(Address, 0, 2);
            _S7Client.Disconnect();
            if (result != 0)
            {
                error = result;
                return false;
            }
            error = 0;
            return true;
        }

        /// <summary> Чтение данных по заливке по времени </summary>
        /// <param name="secondsBegin">прошло секунд заливки</param>
        /// <param name="error">ошибка</param>
        /// <param name="mix">заливка</param>
        /// <returns>получены новые данные заливки</returns>
        public bool GetMixOnTime(out int secondsBegin, out int error, out Mix mix)
        {
            secondsBegin = 0;
            error = 0;
            mix = null;
            var outResult = false;
            int result = _S7Client.ConnectTo(Address, 0, 2);
            if (result == 0)
            {
                byte[] buffer = new byte[4];
                _S7Client.ReadArea(S7Area.MK, 0, 3330, 1, S7WordLength.Byte, buffer);
                var mixRunning = (buffer[0] & 0b1) != 0; //идет заливка
                _S7Client.DBRead(314, 100, 4, buffer);
                secondsBegin = buffer.GetDIntAt(0); //прошло секунд заливки
                if (secondsBegin > SetSecondsToRead && _nowMixRunning) //заливка окончена по времени
                {
                    mix = GetGoodMix(_S7Client, SecondsCorrect);
                    _nowMixRunning = false;
                    outResult = true;
                }
                if (!mixRunning && _nowMixRunning) //заливка ошибочно закончена
                {
                    mix = GetBadMix(_S7Client, SecondsCorrect);
                    _nowMixRunning = false;
                    outResult = true;
                }
                if (mixRunning && !_edgeMixRunning) //пошла новая заливка
                    _nowMixRunning = true;
                _edgeMixRunning = mixRunning;
            }
            else
            {
                error = result;
            }
            _S7Client.Disconnect();
            return outResult;
        }

        #region Вспомогательные методы

        private Mix GetGoodMix(S7Client client, int seconds = 0)
        {
            byte[] bufferDb = new byte[300];
            client.DBRead(401, 0, 300, bufferDb);
            var actSandMud = bufferDb.GetDIntAt(20) / 100.0f;
            var densSand = bufferDb.GetIntAt(122);
            var sandInMud = (actSandMud / densSand) * 1150.0f;
            var mix = new Mix
            {
                Number = 1,
                DateTime = DateTime.Now.AddSeconds(seconds),
                FormNumber = bufferDb.GetIntAt(0),
                RecipeNumber = 0,
                MixerTemperature = bufferDb.GetIntAt(6) / 10.0f,
                SetRevertMud = bufferDb.GetDIntAt(8) / 100.0f,
                ActRevertMud = bufferDb.GetDIntAt(12) / 100.0f,
                SetSandMud = bufferDb.GetDIntAt(16) / 100.0f,
                ActSandMud = actSandMud,
                SetColdWater = bufferDb.GetDIntAt(24) / 100.0f,
                ActColdWater = bufferDb.GetDIntAt(28) / 100.0f,
                SetHotWater = bufferDb.GetDIntAt(32) / 100.0f,
                ActHotWater = bufferDb.GetDIntAt(36) / 100.0f,
                SetMixture1 = bufferDb.GetDIntAt(40) / 100.0f,
                ActMixture1 = bufferDb.GetDIntAt(44) / 100.0f,
                SetMixture2 = bufferDb.GetDIntAt(48) / 100.0f,
                ActMixture2 = bufferDb.GetDIntAt(52) / 100.0f,
                SetCement1 = bufferDb.GetDIntAt(72) / 100.0f,
                ActCement1 = bufferDb.GetDIntAt(76) / 100.0f,
                SetCement2 = bufferDb.GetDIntAt(80) / 100.0f,
                ActCement2 = bufferDb.GetDIntAt(84) / 100.0f,
                SetAluminium1 = bufferDb.GetDIntAt(88) / 100.0f / (AluminiumProp + 1),
                ActAluminium1 = bufferDb.GetDIntAt(92) / 100.0f / (AluminiumProp + 1),
                SetAluminium2 = bufferDb.GetDIntAt(96) / 100.0f / (AluminiumProp + 1),
                ActAluminium2 = bufferDb.GetDIntAt(100) / 100.0f / (AluminiumProp + 1),
                SandInMud = sandInMud,
                DensitySandMud = densSand / 1000.0f,
                DensityRevertMud = bufferDb.GetIntAt(120) / 1000.0f,
                Normal = true,
            };
            return mix;
        }
        private Mix GetBadMix(S7Client client, int seconds = 0)
        {
            byte[] bufferDb = new byte[300];
            client.DBRead(401, 0, 300, bufferDb);
            var actSandMud = bufferDb.GetDIntAt(20) / 100.0f;
            var densSand = bufferDb.GetIntAt(122);
            var sandInMud = (actSandMud / densSand) * 1150.0f;
            var mix = new Mix
            {
                Number = 1,
                DateTime = DateTime.Now.AddSeconds(seconds),
                FormNumber = bufferDb.GetIntAt(0),
                RecipeNumber = 0,
                MixerTemperature = bufferDb.GetIntAt(6) / 10.0f,
                SetRevertMud = bufferDb.GetDIntAt(8) / 100.0f,
                ActRevertMud = bufferDb.GetDIntAt(12) / 100.0f,
                SetSandMud = bufferDb.GetDIntAt(16) / 100.0f,
                ActSandMud = actSandMud,
                SetColdWater = bufferDb.GetDIntAt(24) / 100.0f,
                ActColdWater = bufferDb.GetDIntAt(28) / 100.0f,
                SetHotWater = bufferDb.GetDIntAt(32) / 100.0f,
                ActHotWater = bufferDb.GetDIntAt(36) / 100.0f,
                SetMixture1 = bufferDb.GetDIntAt(40) / 100.0f,
                ActMixture1 = bufferDb.GetDIntAt(44) / 100.0f,
                SetMixture2 = bufferDb.GetDIntAt(48) / 100.0f,
                ActMixture2 = bufferDb.GetDIntAt(52) / 100.0f,
                SetCement1 = bufferDb.GetDIntAt(72) / 100.0f,
                ActCement1 = bufferDb.GetDIntAt(76) / 100.0f,
                SetCement2 = bufferDb.GetDIntAt(80) / 100.0f,
                ActCement2 = bufferDb.GetDIntAt(84) / 100.0f,
                SetAluminium1 = bufferDb.GetDIntAt(88) / 100.0f / (AluminiumProp + 1),
                ActAluminium1 = bufferDb.GetDIntAt(92) / 100.0f / (AluminiumProp + 1),
                SetAluminium2 = bufferDb.GetDIntAt(96) / 100.0f / (AluminiumProp + 1),
                ActAluminium2 = bufferDb.GetDIntAt(100) / 100.0f / (AluminiumProp + 1),
                SandInMud = sandInMud,
                DensitySandMud = densSand / 1000.0f,
                DensityRevertMud = bufferDb.GetIntAt(120) / 1000.0f,
                Normal = false,
                Comment = $"Заливка с ошибкой, продолжительность заливки {seconds} сек.",
            };
            return mix;
        }

        #endregion
    }
}
