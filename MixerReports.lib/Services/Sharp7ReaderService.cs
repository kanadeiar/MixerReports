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

        private bool _nowMixRunning = false; //пошла новая заливка

        private bool _goToCorrecting = false; //корректирование данных

        private bool _edgeMixRunning = false; //звонок новой заливки

        /// <summary> Адрес контроллера </summary>
        public string Address { get; set; } = "10.0.57.10";
        /// <summary> Уставка срабатывания чтения новой заливки </summary>
        public int SetSecondsToRead { get; set; } = 100;
        /// <summary> Уставка срабатывания корректирования времени и температуры данных заливки </summary>
        public int SetSecondsToCorrect { get; set; } = 220;
        /// <summary> Пропорция алюминий к воде </summary>
        public int AluminiumProp { get; set; } = 20;
        /// <summary> Корректировка временная каждой заливки </summary>
        public int SecondsCorrect { get; set; } = - 10;
        /// <summary> Включение корректирования данных температуры и времени заливки </summary>
        public bool EnableCorrecting { get; set; } = true;

        public Sharp7ReaderService()
        {
            _S7Client = new S7Client();
            _S7Client.ConnTimeout = 10_000; //ожидание соединения
            _S7Client.RecvTimeout = 10_000; //ожидание данных
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
        public bool GetMixOnTime(out int secondsBegin, out int error, ref Mix mix)
        {
            secondsBegin = 0;
            error = 0;
            var outResult = false;
            int result = _S7Client.ConnectTo(Address, 0, 2);
            if (result == 0)
            {
                byte[] buffer = new byte[4];
                _S7Client.ReadArea(S7Area.MK, 0, 3330, 1, S7WordLength.Byte, buffer);
                var mixRunning = (buffer[0] & 0b1) != 0; //идет заливка
                _S7Client.DBRead(314, 100, 4, buffer);
                secondsBegin = buffer.GetDIntAt(0); //прошло секунд заливки
                if (EnableCorrecting) //включено корректирование данных
                {
                    if (secondsBegin > SetSecondsToRead && _nowMixRunning && !_goToCorrecting) //заливка предварительно окончена - получение данных весовых
                    {
                        mix = GetGoodMix(_S7Client, SecondsCorrect);
                        if (mix.MixerTemperature > 1.0f || mix.SetSandMud > 1.0f || mix.SetRevertMud > 1.0f) //должны быть получены корректные данные, иначе - повторно получать данные
                        {
                            _goToCorrecting = true;
                        }
                    }
                    if (secondsBegin > SetSecondsToCorrect && _nowMixRunning && _goToCorrecting) //заливка еще раз окончена - корректирование данных температуры и времени
                    {
                        GetCorrectedMix(_S7Client, ref mix, SecondsCorrect);
                        _goToCorrecting = false;
                        _nowMixRunning = false;
                        outResult = true;
                    }
                }
                else
                {
                    if (secondsBegin > SetSecondsToRead && _nowMixRunning) //заливка окончена по времени получение данных весовых
                    {
                        mix = GetGoodMix(_S7Client, SecondsCorrect);
                        if (mix.MixerTemperature > 1.0f || mix.SetSandMud > 1.0f || mix.SetRevertMud > 1.0f) //должны быть получены корректные данные, иначе - повторно получать данные
                        {
                            _nowMixRunning = false;
                            outResult = true;
                        }
                    }
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
        /// <summary> Получение данных по заливке из контроллера </summary>
        private Mix GetGoodMix(S7Client client, int seconds = 0)
        {
            byte[] bufferDb = new byte[300];
            client.DBRead(401, 0, 300, bufferDb);
            //var actSandMud = bufferDb.GetDIntAt(20) / 100.0f;
            //var densSand = bufferDb.GetIntAt(122);
            var actSandMud = bufferDb.GetDIntAt(182) / 100.0f;
            var densSand = bufferDb.GetIntAt(284);
            var sandInMud = 0.0f;
            if (densSand >= 900 && densSand <= 2000)
                sandInMud = (actSandMud / densSand) * 1110.0f;
            var mix = new Mix
            {
                Number = 1,
                DateTime = DateTime.Now.AddSeconds(seconds),
                FormNumber = bufferDb.GetIntAt(0),
                RecipeNumber = 0,
                MixerTemperature = bufferDb.GetIntAt(6) / 10.0f,
                #region Получение инфы по компонентам в смесителе
                //SetRevertMud = bufferDb.GetDIntAt(8) / 100.0f,
                //ActRevertMud = bufferDb.GetDIntAt(12) / 100.0f,
                //SetSandMud = bufferDb.GetDIntAt(16) / 100.0f,
                //ActSandMud = actSandMud,
                //SetColdWater = bufferDb.GetDIntAt(24) / 100.0f,
                //ActColdWater = bufferDb.GetDIntAt(28) / 100.0f,
                //SetHotWater = bufferDb.GetDIntAt(32) / 100.0f,
                //ActHotWater = bufferDb.GetDIntAt(36) / 100.0f,
                //SetMixture1 = bufferDb.GetDIntAt(40) / 100.0f,
                //ActMixture1 = bufferDb.GetDIntAt(44) / 100.0f,
                //SetMixture2 = bufferDb.GetDIntAt(48) / 100.0f,
                //ActMixture2 = bufferDb.GetDIntAt(52) / 100.0f,
                //SetCement1 = bufferDb.GetDIntAt(72) / 100.0f,
                //ActCement1 = bufferDb.GetDIntAt(76) / 100.0f,
                //SetCement2 = bufferDb.GetDIntAt(80) / 100.0f,
                //ActCement2 = bufferDb.GetDIntAt(84) / 100.0f,
                //SetAluminium1 = bufferDb.GetDIntAt(88) / 100.0f / (AluminiumProp + 1),
                //ActAluminium1 = bufferDb.GetDIntAt(92) / 100.0f / (AluminiumProp + 1),
                //SetAluminium2 = bufferDb.GetDIntAt(96) / 100.0f / (AluminiumProp + 1),
                //ActAluminium2 = bufferDb.GetDIntAt(100) / 100.0f / (AluminiumProp + 1),
                //SandInMud = sandInMud,
                //DensitySandMud = densSand / 1000.0f,
                //DensityRevertMud = bufferDb.GetIntAt(120) / 1000.0f,

                #endregion
                SetRevertMud = bufferDb.GetDIntAt(170) / 100.0f,
                ActRevertMud = bufferDb.GetDIntAt(174) / 100.0f,
                SetSandMud = bufferDb.GetDIntAt(178) / 100.0f,
                ActSandMud = actSandMud,
                SetColdWater = bufferDb.GetDIntAt(186) / 100.0f,
                ActColdWater = bufferDb.GetDIntAt(190) / 100.0f,
                SetHotWater = bufferDb.GetDIntAt(194) / 100.0f,
                ActHotWater = bufferDb.GetDIntAt(198) / 100.0f,
                SetMixture1 = bufferDb.GetDIntAt(202) / 100.0f,
                ActMixture1 = bufferDb.GetDIntAt(206) / 100.0f,
                SetMixture2 = bufferDb.GetDIntAt(210) / 100.0f,
                ActMixture2 = bufferDb.GetDIntAt(214) / 100.0f,
                SetCement1 = bufferDb.GetDIntAt(234) / 100.0f,
                ActCement1 = bufferDb.GetDIntAt(238) / 100.0f,
                SetCement2 = bufferDb.GetDIntAt(242) / 100.0f,
                ActCement2 = bufferDb.GetDIntAt(246) / 100.0f,
                SetAluminium1 = bufferDb.GetDIntAt(250) / 100.0f / (AluminiumProp + 1),
                ActAluminium1 = bufferDb.GetDIntAt(254) / 100.0f / (AluminiumProp + 1),
                SetAluminium2 = bufferDb.GetDIntAt(258) / 100.0f / (AluminiumProp + 1),
                ActAluminium2 = bufferDb.GetDIntAt(262) / 100.0f / (AluminiumProp + 1),
                SandInMud = sandInMud,
                DensitySandMud = densSand / 1000.0f,
                DensityRevertMud = bufferDb.GetIntAt(282) / 1000.0f,
                Normal = true,
            };
            return mix;
        }
        /// <summary> Уточнение данных заливки по контроллеру </summary>
        private void GetCorrectedMix(S7Client client, ref Mix mix, int seconds = 0)
        {
            byte[] bufferDb = new byte[300];
            client.DBRead(401, 0, 300, bufferDb);
            mix.DateTime = DateTime.Now.AddSeconds(seconds);
            mix.MixerTemperature = bufferDb.GetIntAt(6) / 10.0f;
        }
        private Mix GetBadMix(S7Client client, int seconds = 0)
        {
            byte[] bufferDb = new byte[300];
            client.DBRead(401, 0, 300, bufferDb);
            //var actSandMud = bufferDb.GetDIntAt(20) / 100.0f;
            //var densSand = bufferDb.GetIntAt(122);
            var actSandMud = bufferDb.GetDIntAt(182) / 100.0f;
            var densSand = bufferDb.GetIntAt(284);
            var sandInMud = 0.0f;
            if (densSand >= 900 && densSand <= 2000)
                sandInMud = (actSandMud / densSand) * 1150.0f;
            var mix = new Mix
            {
                Number = 1,
                DateTime = DateTime.Now.AddSeconds(seconds),
                FormNumber = bufferDb.GetIntAt(0),
                RecipeNumber = 0,
                MixerTemperature = bufferDb.GetIntAt(6) / 10.0f,
                #region Опрос инфы по компонентам в смесителе
                //SetRevertMud = bufferDb.GetDIntAt(8) / 100.0f,
                //ActRevertMud = bufferDb.GetDIntAt(12) / 100.0f,
                //SetSandMud = bufferDb.GetDIntAt(16) / 100.0f,
                //ActSandMud = actSandMud,
                //SetColdWater = bufferDb.GetDIntAt(24) / 100.0f,
                //ActColdWater = bufferDb.GetDIntAt(28) / 100.0f,
                //SetHotWater = bufferDb.GetDIntAt(32) / 100.0f,
                //ActHotWater = bufferDb.GetDIntAt(36) / 100.0f,
                //SetMixture1 = bufferDb.GetDIntAt(40) / 100.0f,
                //ActMixture1 = bufferDb.GetDIntAt(44) / 100.0f,
                //SetMixture2 = bufferDb.GetDIntAt(48) / 100.0f,
                //ActMixture2 = bufferDb.GetDIntAt(52) / 100.0f,
                //SetCement1 = bufferDb.GetDIntAt(72) / 100.0f,
                //ActCement1 = bufferDb.GetDIntAt(76) / 100.0f,
                //SetCement2 = bufferDb.GetDIntAt(80) / 100.0f,
                //ActCement2 = bufferDb.GetDIntAt(84) / 100.0f,
                //SetAluminium1 = bufferDb.GetDIntAt(88) / 100.0f / (AluminiumProp + 1),
                //ActAluminium1 = bufferDb.GetDIntAt(92) / 100.0f / (AluminiumProp + 1),
                //SetAluminium2 = bufferDb.GetDIntAt(96) / 100.0f / (AluminiumProp + 1),
                //ActAluminium2 = bufferDb.GetDIntAt(100) / 100.0f / (AluminiumProp + 1),
                //SandInMud = sandInMud,
                //DensitySandMud = densSand / 1000.0f,
                //DensityRevertMud = bufferDb.GetIntAt(120) / 1000.0f,
                #endregion
                SetRevertMud = bufferDb.GetDIntAt(170) / 100.0f,
                ActRevertMud = bufferDb.GetDIntAt(174) / 100.0f,
                SetSandMud = bufferDb.GetDIntAt(178) / 100.0f,
                ActSandMud = actSandMud,
                SetColdWater = bufferDb.GetDIntAt(186) / 100.0f,
                ActColdWater = bufferDb.GetDIntAt(190) / 100.0f,
                SetHotWater = bufferDb.GetDIntAt(194) / 100.0f,
                ActHotWater = bufferDb.GetDIntAt(198) / 100.0f,
                SetMixture1 = bufferDb.GetDIntAt(202) / 100.0f,
                ActMixture1 = bufferDb.GetDIntAt(206) / 100.0f,
                SetMixture2 = bufferDb.GetDIntAt(210) / 100.0f,
                ActMixture2 = bufferDb.GetDIntAt(214) / 100.0f,
                SetCement1 = bufferDb.GetDIntAt(234) / 100.0f,
                ActCement1 = bufferDb.GetDIntAt(238) / 100.0f,
                SetCement2 = bufferDb.GetDIntAt(242) / 100.0f,
                ActCement2 = bufferDb.GetDIntAt(246) / 100.0f,
                SetAluminium1 = bufferDb.GetDIntAt(250) / 100.0f / (AluminiumProp + 1),
                ActAluminium1 = bufferDb.GetDIntAt(254) / 100.0f / (AluminiumProp + 1),
                SetAluminium2 = bufferDb.GetDIntAt(258) / 100.0f / (AluminiumProp + 1),
                ActAluminium2 = bufferDb.GetDIntAt(262) / 100.0f / (AluminiumProp + 1),
                SandInMud = sandInMud,
                DensitySandMud = densSand / 1000.0f,
                DensityRevertMud = bufferDb.GetIntAt(282) / 1000.0f,
                Normal = false,
                Comment = $"Заливка с ошибкой.",
            };
            return mix;
        }

        #endregion
    }
}
