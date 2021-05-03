using System.Windows;
using System.Windows.Input;
using MixerReportsEditor.Commands;

namespace MixerReportsEditor.ViewModel
{
    class MainWindowViewModel : Base.ViewModel
    {
        #region Data

        

        #endregion

        #region Properties





        #region Supports

        private string _Title = "Заливочные отчеты - Редактирование данных";

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

        #region Commands





        #region Supports

        private ICommand _CloseApplicationCommand;

        /// <summary> Закрыть приложение </summary>
        public ICommand CloseApplicationCommand => _CloseApplicationCommand ??=
            new LambdaCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecute);

        private bool CanCloseApplicationCommandExecute(object p) => true;

        private void OnCloseApplicationCommandExecuted(object p)
        {
            Application.Current.Shutdown();
        }

        #endregion

        #endregion

    }
}
