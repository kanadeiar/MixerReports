using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MixerReports.lib.Interfaces;
using MixerReports.lib.Models;
using MixerReportsEditor.Commands;

namespace MixerReportsEditor.ViewModel
{
    class MainWindowViewModel : Base.ViewModel
    {
        #region Data

        private readonly IRepository<Mix> _Mixes;

        #endregion

        #region Properties

        /// <summary> Заливки за два последних часа, доступные для изменения </summary>
        public ObservableCollection<Mix> Mixes { get; } = new ();


        private DateTime _ShiftSelectDateTime = DateTime.Today.AddDays(- 1);

        /// <summary> Выбранная дата работы двух смен </summary>
        public DateTime ShiftSelectDateTime
        {
            get => _ShiftSelectDateTime;
            set => Set(ref _ShiftSelectDateTime, value);
        }

        /// <summary> Данные дневной смены за выбранную дату </summary>
        public ICollection<Mix> ShiftDayMixes
        {
            get
            {
                var date = ShiftSelectDateTime.Date.AddHours(8);
                var mixs = _Mixes.GetAll()
                    .Where(m => m.DateTime >= date && m.DateTime < date.AddHours(12))
                    .OrderBy(m => m.DateTime).ToList();
                return mixs;
            }
        }
        /// <summary> Данные ночной смены за выбранную дату </summary>
        public ICollection<Mix> ShiftNightMixes
        {
            get
            {
                var date = ShiftSelectDateTime.Date.AddHours(8);
                var mixs = _Mixes.GetAll()
                    .Where(m => m.DateTime >= date.AddHours(12) && m.DateTime < date.AddHours(24))
                    .OrderBy(m => m.DateTime).ToList();
                return mixs;
            }
        }

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


        public MainWindowViewModel(IRepository<Mix> mixes)
        {
            _Mixes = mixes;
        }


        #region Commands
        
        #region Данные

        private ICommand _LoadDataCommand;

        /// <summary> Команда загрузки данных </summary>
        public ICommand LoadDataCommand => _LoadDataCommand ??=
            new LambdaCommand(OnLoadDataCommandExecuted, CanLoadDataCommandExecute);

        private bool CanLoadDataCommandExecute(object p) => true;

        private void OnLoadDataCommandExecuted(object p)
        {
            LoadData();
        }

        #endregion



        #region Данные по сменам

        private ICommand _SetYesterdayShiftMixesCommand;

        /// <summary> Установить дату на вчерашний день </summary>
        public ICommand SetYesterdayShiftMixesCommand => _SetYesterdayShiftMixesCommand ??=
            new LambdaCommand(OnSetYesterdayShiftMixesCommandExecuted, CanSetYesterdayShiftMixesCommandExecute);

        private bool CanSetYesterdayShiftMixesCommandExecute(object p) => true;

        private void OnSetYesterdayShiftMixesCommandExecuted(object p)
        {
            ShiftSelectDateTime = DateTime.Today.AddDays(- 1);
        }

        private ICommand _UpdateShiftMixesCommand;

        /// <summary> Обновить данные смены за выбранную дату </summary>
        public ICommand UpdateShiftMixesCommand => _UpdateShiftMixesCommand ??=
            new LambdaCommand(OnUpdateShiftMixesCommandExecuted, CanUpdateShiftMixesCommandExecute);

        private bool CanUpdateShiftMixesCommandExecute(object p) => true;

        private void OnUpdateShiftMixesCommandExecuted(object p)
        {
            OnPropertyChanged(nameof(ShiftDayMixes));
            OnPropertyChanged(nameof(ShiftNightMixes));
        }

        #endregion


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

        #region Methods

        /// <summary> Загрузка данных из бд во вьюмодель </summary>
        private void LoadData()
        {
            Mixes.Clear();
            foreach (var mix in _Mixes.GetAll()
                .Where(m => m.DateTime >= DateTime.Now.AddHours(- 2))
                .OrderByDescending(m => m.DateTime))
            {
                Mixes.Add(mix);
            }
        }

        private DateTime GetDateTimesFrom(DateTime date)
        {
            var dateFrom = date.Date.AddHours(8);
            if (DateTime.Now.Hour < 8)
                dateFrom = dateFrom.AddHours(-12);
            if (DateTime.Now.Hour >= 20)
                dateFrom = dateFrom.AddHours(12);
            return dateFrom;
        }

        #endregion

    }
}
