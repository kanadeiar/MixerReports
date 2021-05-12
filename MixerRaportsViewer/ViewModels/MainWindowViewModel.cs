using System.Windows;
using System.Windows.Input;
using MixerRaportsViewer.Commands;

namespace MixerRaportsViewer.ViewModels
{
    class MainWindowViewModel : Base.ViewModel
    {
        #region Данные



        #endregion

        #region Свойства



        #region Вспомогательные данные

        private string _Title = "Заливочные отчеты - Обзор данных";

        /// <summary> Название приложения </summary>
        public string Title
        {
            get => _Title;
            set => Set(ref _Title, value);
        }

        private bool _ConnectToDataBase;

        /// <summary> Инфа про соединение с базой данных </summary>
        public bool ConnectToDataBase
        {
            get => _ConnectToDataBase;
            set
            {
                Set(ref _ConnectToDataBase, value);
                OnPropertyChanged(nameof(ConnectToDataBaseStr));
            }
        }
        public string ConnectToDataBaseStr => (ConnectToDataBase) ? "Соединение с базой данных установлено" : "Соединение с базой данных потеряно";

        #endregion

        #endregion

        public MainWindowViewModel()
        {
            
        }

        #region Команды




        #region Вспомогательные команды

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

        #region Вспомогательные методы



        #endregion
    }
}
