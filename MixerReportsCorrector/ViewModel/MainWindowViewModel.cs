using System.Windows;
using System.Windows.Input;
using MixerReportsCorrector.Commands;

namespace MixerReportsCorrector.ViewModel
{
    class MainWindowViewModel : Base.ViewModel
    {


        #region Свойства




        #region Вспомогательные

        private string _Title = "Заливочные отчеты - Уточнение данных";

        /// <summary> Заголовок приложения </summary>
        public string Title
        {
            get => _Title;
            set => Set(ref _Title, value);
        }

        #endregion

        #endregion

        public MainWindowViewModel()
        {
            
        }

        #region Команды




        #region Вспомогательные команды

        private ICommand _CloseAppCommand;

        /// <summary> Закрыть приложение </summary>
        public ICommand CloseAppCommand => _CloseAppCommand ??=
            new LambdaCommand(OnCloseAppCommandExecuted, CanCloseAppCommandExecute);

        private bool CanCloseAppCommandExecute(object p) => true;

        private void OnCloseAppCommandExecuted(object p)
        {
            Application.Current.Shutdown();
        }

        #endregion

        #endregion


    }
}
