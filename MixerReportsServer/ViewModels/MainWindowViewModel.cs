using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using MixerReports.lib.Interfaces;
using MixerReports.lib.Models;
using MixerReportsServer.Commands;
using MixerReportsServer.ViewModels.Base;

namespace MixerReportsServer.ViewModels
{
    class MainWindowViewModel : ViewModel
    {
        private readonly IRepository<Mix> _Mixes;

        #region Свойства

        public ObservableCollection<Mix> Mixes { get; } = new ();

        private string _Log;

        /// <summary> Лог работы </summary>
        public string Log
        {
            get => _Log;
            set => Set(ref _Log, value);
        }

        #region Вспомогательное

        private string _Title = "Заливочные отчеты - Сервер";

        /// <summary> Заголовок приложения </summary>
        public string Title
        {
            get => _Title;
            set => Set(ref _Title, value);
        }

        #endregion

        #endregion

        public MainWindowViewModel(IRepository<Mix> mixes)
        {
            _Mixes = mixes;
        }

        #region Команды

        private ICommand _LoadDataCommand;

        /// <summary> Команда загрузки данных </summary>
        public ICommand LoadDataCommand => _LoadDataCommand ??=
            new LambdaCommand(OnLoadDataCommandExecuted, CanLoadDataCommandExecute);

        private bool CanLoadDataCommandExecute(object p) => true;

        private void OnLoadDataCommandExecuted(object p)
        {
            LoadData();
        }



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

        #region Вспомогательные

        private void LoadData()
        {
            Mixes.Clear();
            foreach (var mix in _Mixes.GetAll())
            {
                Mixes.Add(mix);
            }
        }

        #endregion
    }
}
