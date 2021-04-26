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
    }
}
