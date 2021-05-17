using MixerReports.lib.Models;

namespace MixerReports.lib.Interfaces
{
    /// <summary> Сервис получения информации по заливкам из контроллера </summary>
    public interface ISharp7MixReaderService
    {
        /// <summary> Тест соединения с контроллером </summary>
        bool TestConnection(out int error);
        /// <summary> Проба получения новых данных заливки с контроллера, вызов с таймера </summary>
        /// <param name="secondsBegin">секунд продолжается заливка</param>
        /// <param name="error">ошибка чтения ПЛК</param>
        /// <param name="mix">данные заливки</param>
        /// <returns>Есть новые данные по заливке</returns>
        bool TryNewMixTick(out int secondsBegin, out int error, out Mix mix);
    }
}
