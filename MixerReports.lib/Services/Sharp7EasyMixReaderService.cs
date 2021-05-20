using System.Diagnostics;
using MixerReports.lib.Interfaces;
using MixerReports.lib.Models;
using MixerReports.lib.Services.Tools;
using Sharp7;

namespace MixerReports.lib.Services
{
    public class Sharp7EasyMixReaderService : ISharp7MixReaderService
    {
        private readonly S7Client _client;
        private readonly BufferedPLCConnector _connector;

        private readonly string _address;

        private bool _edgeFrontBegin;

        public Sharp7EasyMixReaderService(S7Client client, string address = "10.0.57.10", int aluminiumProp = 20, int seconds = - 10)
        {
            //_client = new S7Client {ConnTimeout = 5_000, RecvTimeout = 5_000};
            _client = client;
            _address = address;
            _connector = new BufferedPLCConnector(_client, aluminiumProp, seconds);
        }
        /// <summary> Тест соединения с контроллером </summary>
        public bool TestConnection(out int error)
        {
            var result = _client.ConnectTo(_address, 0, 2);
            _client.Disconnect();
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
            var r = _client.ConnectTo(_address, 0, 2);
            if (r == 0)
            {
                var run = _connector.NowMixRunning(out secondsBegin, out bool begin, out bool end);
                _connector.GetNewDataFromPLCToBuffetDbTick();
                var count = _connector.UpdateBuffer(begin);
                if (begin)
                    _edgeFrontBegin = true;
                if (_edgeFrontBegin && count <= 4)
                {
                    _connector.SetNewMixFromBuffer();
                    _connector.ClearBuffer();
                    _edgeFrontBegin = false;
                }
                if (end)
                {
                    mix = _connector.GetUpdatedMix();
                    if (mix != null)
                        result = true;
                }
                if (_edgeFrontBegin)
                    Debug.WriteLine($"Начало заливки начато, число компонентов не в смесителе: {count}");
            }
            else
            {
                error = r;
            }
            _client.Disconnect();
            return result;
        }
    }
}
