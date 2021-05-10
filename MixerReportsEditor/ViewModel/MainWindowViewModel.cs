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

        #region Данные по заливкам за последние два часа

        /// <summary> Заливки за два последних часа, доступные для изменения </summary>
        public ObservableCollection<Mix> Mixes { get; } = new ();

        #endregion

        #region Данные по заливкам за смену

        /// <summary> Время сколько длиться эта смена </summary>
        public string TimeSpanCurrentShiftMixes
        {
            get
            {
                var span = DateTime.Now - GetDateTimesFrom(DateTime.Now);
                return $"{span.Hours} час. {span.Minutes} мин. {span.Seconds} сек.";
            }
        }

        /// <summary> Количество заливок текущей смены </summary>
        public int CountCurrentShiftMixes => CurrentShiftMixes.Count;

        /// <summary> Количество нормальных заливок текущей смены </summary>
        public int CountNormalCurrentShiftMixes => CurrentShiftMixes.Count(m => m.Normal);

        /// <summary> Данные по заливкам текущей смены </summary>
        public ICollection<Mix> CurrentShiftMixes
        {
            get
            {
                var date = GetDateTimesFrom(DateTime.Now);
                var mixs = _Mixes.GetAll()
                    .Where(m => m.DateTime >= date)
                    .OrderByDescending(m => m.DateTime).ToList();
                return mixs;
            }
        }

        /// <summary> Время и дата начала предидущей смены </summary>
        public string TimeBeginPreShiftMixes
        {
            get
            {
                var date = GetDateTimesFrom(DateTime.Now).AddHours(-12);
                return $"{date:HH:mm:ss dd.MM.yyyy}";
            }
        }

        /// <summary> Количество заливок предидущей смены </summary>
        public int CountPreShiftMixes => PreShiftMixes.Count;

        /// <summary> Количество нормальных заливок предидущей смены </summary>
        public int CountNormalPreShiftMixes => PreShiftMixes.Count(m => m.Normal);

        /// <summary> Данные по заливкам предидущей смены </summary>
        public ICollection<Mix> PreShiftMixes
        {
            get
            {
                var date = GetDateTimesFrom(DateTime.Now).AddHours(-12);
                var mixs = _Mixes.GetAll()
                    .Where(m => m.DateTime >= date && m.DateTime < date.AddHours(12))
                    .OrderByDescending(m => m.DateTime).ToList();
                return mixs;
            }
        }

        #endregion

        #region Данные по сменам за выбранную дату - дневная и ночная

        private DateTime _ShiftSelectDateTime = DateTime.Today.AddDays(- 1);

        /// <summary> Выбранная дата работы двух смен </summary>
        public DateTime ShiftSelectDateTime
        {
            get => _ShiftSelectDateTime;
            set
            {
                Set(ref _ShiftSelectDateTime, value);
                OnPropertyChanged(nameof(ShiftDayMixes));
                OnPropertyChanged(nameof(ShiftNightMixes));
                OnPropertyChanged(nameof(CountShiftDayMixes));
                OnPropertyChanged(nameof(CountShiftNightMixes));
                OnPropertyChanged(nameof(CountNormalShiftDayMixes));
                OnPropertyChanged(nameof(CountNormalShiftNightMixes));
            }
        }

        /// <summary> Количество заливок дневной смены </summary>
        public int CountShiftDayMixes
        {
            get
            {
                var date = ShiftSelectDateTime.Date.AddHours(8);
                return _Mixes.GetAll()
                    .Count(m => m.DateTime >= date && m.DateTime < date.AddHours(12));
            }
        }
        /// <summary> Количество нормальных заливок </summary>
        public int CountNormalShiftDayMixes
        {
            get
            {
                var date = ShiftSelectDateTime.Date.AddHours(8);
                return _Mixes.GetAll()
                    .Count(m => m.DateTime >= date && m.DateTime < date.AddHours(12) && m.Normal);
            }
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

        /// <summary> Количество заливок ночной смены </summary>
        public int CountShiftNightMixes
        {
            get
            {
                var date = ShiftSelectDateTime.Date.AddHours(8);
                return _Mixes.GetAll()
                    .Count(m => m.DateTime >= date.AddHours(12) && m.DateTime < date.AddHours(24));
            }
        }
        /// <summary> Количество нормальных заливок </summary>
        public int CountNormalShiftNightMixes
        {
            get
            {
                var date = ShiftSelectDateTime.Date.AddHours(8);
                return _Mixes.GetAll()
                    .Count(m => m.DateTime >= date.AddHours(12) && m.DateTime < date.AddHours(24) && m.Normal);
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

        #endregion

        #region Архивные данные за выбранный диапазон дат

        private DateTime _FilterArchivesBeginDateTime = DateTime.Today.AddDays(- 1);

        /// <summary> Выбранная дата начала архивных данных </summary>
        public DateTime FilterArchivesBeginDateTime
        {
            get => _FilterArchivesBeginDateTime;
            set
            {
                Set(ref _FilterArchivesBeginDateTime, value);
                OnPropertyChanged(nameof(FilteredArchivesMixes));
            }
        }

        private DateTime _FilterArchivesEndDateTime = DateTime.Today;

        /// <summary> Выбранная дата окончания архивных данных </summary>
        public DateTime FilterArchivesEndDateTime
        {
            get => _FilterArchivesEndDateTime;
            set
            {
                Set(ref _FilterArchivesEndDateTime, value);
                OnPropertyChanged(nameof(FilteredArchivesMixes));
            }
        }

        /// <summary> Отфильтрованные архивные данные </summary>
        public ICollection<Mix> FilteredArchivesMixes
        {
            get
            {
                var mixs = _Mixes.GetAll()
                    .Where(m => m.DateTime >= FilterArchivesBeginDateTime && m.DateTime < FilterArchivesEndDateTime.AddHours(24))
                    .OrderBy(m => m.DateTime).ToList();
                return mixs;
            }
        }

        #endregion

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

        #region Данные по заливкам за последние два часа

        private ICommand _ShowDetailMixWithEditCommand;

        /// <summary> Детальный просмотр данных по заливке с целью ввода данных </summary>
        public ICommand ShowDetailMixWithEditCommand => _ShowDetailMixWithEditCommand ??=
            new LambdaCommand(OnShowDetailMixWithEditCommandExecuted, CanShowDetailMixWithEditCommandExecute);

        private bool CanShowDetailMixWithEditCommandExecute(object p) => true;

        private void OnShowDetailMixWithEditCommandExecuted(object p)
        {
            if (!(p is Mix mix))
                return;

            MessageBox.Show($"Дата: {mix.DateTime} номер: {mix.Number} форма: {mix.FormNumber}");
        }

        private ICommand _UpdateMixCommand;

        /// <summary> Обновить данные заливок за последние два часа </summary>
        public ICommand UpdateMixCommand => _UpdateMixCommand ??=
            new LambdaCommand(OnUpdateMixCommandExecuted, CanUpdateMixCommandExecute);

        private bool CanUpdateMixCommandExecute(object p) => true;

        private void OnUpdateMixCommandExecuted(object p)
        {
            OnPropertyChanged(nameof(TimeSpanCurrentShiftMixes));
            OnPropertyChanged(nameof(CountCurrentShiftMixes));
            OnPropertyChanged(nameof(CountNormalCurrentShiftMixes));
            LoadData();
        }

        #endregion

        #region Данные по заливкам за смену

        private ICommand _UpdateCurrentShiftMixesCommand;

        /// <summary> Обновить данные последней смены </summary>
        public ICommand UpdateCurrentShiftMixesCommand => _UpdateCurrentShiftMixesCommand ??=
            new LambdaCommand(OnUpdateCurrentShiftMixesCommandExecuted, CanUpdateCurrentShiftMixesCommandExecute);

        private bool CanUpdateCurrentShiftMixesCommandExecute(object p) => true;

        private void OnUpdateCurrentShiftMixesCommandExecuted(object p)
        {
            OnPropertyChanged(nameof(TimeSpanCurrentShiftMixes));
            OnPropertyChanged(nameof(CountCurrentShiftMixes));
            OnPropertyChanged(nameof(CountNormalCurrentShiftMixes));
            OnPropertyChanged(nameof(CurrentShiftMixes));
        }

        private ICommand _UpdatePreShiftMixesCommand;

        /// <summary> Обновить данные предидущей смены </summary>
        public ICommand UpdatePreShiftMixesCommand => _UpdatePreShiftMixesCommand ??=
            new LambdaCommand(OnUpdatePreShiftMixesCommandExecuted, CanUpdatePreShiftMixesCommandExecute);

        private bool CanUpdatePreShiftMixesCommandExecute(object p) => true;

        private void OnUpdatePreShiftMixesCommandExecuted(object p)
        {
            OnPropertyChanged(nameof(TimeBeginPreShiftMixes));
            OnPropertyChanged(nameof(CountPreShiftMixes));
            OnPropertyChanged(nameof(CountNormalPreShiftMixes));
            OnPropertyChanged(nameof(PreShiftMixes));
        }

        #endregion

        #region Данные по заливкам за смену за выбранную дату - дневная и ночная

        private ICommand _UpdateShiftMixesCommand;

        /// <summary> Обновить данные смены за выбранную дату </summary>
        public ICommand UpdateShiftMixesCommand => _UpdateShiftMixesCommand ??=
            new LambdaCommand(OnUpdateShiftMixesCommandExecuted, CanUpdateShiftMixesCommandExecute);

        private bool CanUpdateShiftMixesCommandExecute(object p) => true;

        private void OnUpdateShiftMixesCommandExecuted(object p)
        {
            OnPropertyChanged(nameof(ShiftDayMixes));
            OnPropertyChanged(nameof(ShiftNightMixes));
            OnPropertyChanged(nameof(CountShiftDayMixes));
            OnPropertyChanged(nameof(CountShiftNightMixes));
            OnPropertyChanged(nameof(CountNormalShiftDayMixes));
            OnPropertyChanged(nameof(CountNormalShiftNightMixes));
        }

        #endregion

        #region Архивные данные за выбранный диапазон дат
        
        private ICommand _UpdateFilteredArchiveMixesCommand;

        /// <summary> Обновить отфильтрованне архивные данные </summary>
        public ICommand UpdateFilteredArchiveMixesCommand => _UpdateFilteredArchiveMixesCommand ??=
            new LambdaCommand(OnUpdateFilteredArchiveMixesCommandExecuted, CanUpdateFilteredArchiveMixesCommandExecute);

        private bool CanUpdateFilteredArchiveMixesCommandExecute(object p) => true;

        private void OnUpdateFilteredArchiveMixesCommandExecuted(object p)
        {
            OnPropertyChanged(nameof(FilteredArchivesMixes));
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
        /// <summary> Получение времени начала работы смены </summary>
        private DateTime GetDateTimesFrom(DateTime date)
        {
            var dateFrom = date.Date.AddHours(8);
            if (date.Hour < 8)
                dateFrom = dateFrom.AddHours(-12);
            if (date.Hour >= 20)
                dateFrom = dateFrom.AddHours(12);
            return dateFrom;
        }

        #endregion

    }
}
