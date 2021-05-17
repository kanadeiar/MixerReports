using MixerReports.lib.Models.Base;

namespace MixerReportsServer.Models
{
    class Settings : Entity
    {
        public bool Changed { get; set; } //изменение настроек

        private int _SetSecondsToRead = 100;
        /// <summary> Уставка секунд прошедших заливки для чтения значений из контроллера </summary>
        public int SetSecondsToRead
        {
            get => _SetSecondsToRead;
            set
            {
                Set(ref _SetSecondsToRead, value);
                Changed = true;
            }
        }

        private int _SetSecondsToCorrect = 220;
        /// <summary> Уставка секунд прошедших заливки для корректирования значений по контроллеру </summary>
        public int SetSecondsToCorrect
        {
            get => _SetSecondsToCorrect;
            set => Set(ref _SetSecondsToCorrect, value);
        }

        private string _Address = "10.0.57.10";
        /// <summary> IP адрес контроллера </summary>
        public string Address
        {
            get => _Address;
            set
            {
                Set(ref _Address, value);
                Changed = true;
            }
        }

        private int _AluminiumProp = 20;
        /// <summary> Пропорция алюминий к воде </summary>
        public int AluminiumProp
        {
            get => _AluminiumProp;
            set
            {
                Set(ref _AluminiumProp, value);
                Changed = true;
            }
        }

        private int _SecondsCorrect = 10;
        /// <summary> Коррекция прошедших секунд заливки в полученном результате </summary>
        public int SecondsCorrect
        {
            get => _SecondsCorrect;
            set
            {
                Set(ref _SecondsCorrect, value);
                Changed = true;
            }
        }
    }
}
