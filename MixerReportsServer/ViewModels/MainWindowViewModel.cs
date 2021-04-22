using MixerReportsServer.ViewModels.Base;

namespace MixerReportsServer.ViewModels
{
    class MainWindowViewModel : ViewModel
    {
        #region Свойства

        private string _Title = "Заливочные отчеты - Сервер";

        /// <summary> Заголовок приложения </summary>
        public string Title
        {
            get => _Title;
            set => Set(ref _Title, value);
        }

        #endregion

        public MainWindowViewModel()
        {
            
        }
    }
}
