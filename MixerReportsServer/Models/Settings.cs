using MixerReports.lib.Models.Base;

namespace MixerReportsServer.Models
{
    class Settings : Entity
    {
        private int _SetSecondsToRead = 220;

        /// <summary> Уставка секунд прошедших заливки для акта записи значений </summary>
        public int SetSecondsToRead
        {
            get => _SetSecondsToRead;
            set => Set(ref _SetSecondsToRead, value);
        }

        private string _Address = "10.0.57.10";

        /// <summary> IP адрес контроллера </summary>
        public string Address
        {
            get => _Address;
            set => Set(ref _Address, value);
        }
    }
}
