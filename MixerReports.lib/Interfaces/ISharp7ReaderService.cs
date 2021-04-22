﻿using MixerReports.lib.Models;

namespace MixerReports.lib.Interfaces
{
    public interface ISharp7ReaderService
    {
        /// <summary> Уставка срабатывания чтения новой заливки </summary>
        int SetSecondsToRead { get; set; }

        /// <summary> Пропорция алюминий к воде </summary>
        int AluminiumProp { get; set; }

        /// <summary> Тест соединения с контроллером </summary>
        bool TestConnection(out int error);

        /// <summary> Чтение данных по заливке по времени </summary>
        /// <param name="secondsBegin">прошло секунд заливки</param>
        /// <param name="error">ошибка</param>
        /// <param name="mix">заливка</param>
        /// <returns>получены новые данные заливки</returns>
        bool GetMixOnTime(out int secondsBegin, out int error, out Mix mix);
    }
}
