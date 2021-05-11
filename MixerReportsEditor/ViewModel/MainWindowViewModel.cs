using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MixerReports.lib.Interfaces;
using MixerReports.lib.Models;
using MixerReportsEditor.Commands;
using MixerReportsEditor.Windows;

namespace MixerReportsEditor.ViewModel
{
    class MainWindowViewModel : Base.ViewModel
    {
        #region Data

        private readonly IRepository<Mix> _Mixes;
        private readonly Timer _timer;

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

        public MainWindowViewModel(IRepository<Mix> mixes)
        {
            _Mixes = mixes;
            //автообновление 1 раз в минуту только самых передних данных
            _timer = new Timer(60_000);
            _timer.Elapsed += (s, a) => Application.Current.Dispatcher.Invoke(UpdateFirstData);
            _timer.Start();
        }

        /// <summary> Текущее время </summary>
        public DateTime CurrentDateTime => DateTime.Now;

        #region Commands
        
        #region Данные все

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
            var formNumber = mix.FormNumber;
            var recipeNumber = mix.RecipeNumber;
            var normal = mix.Normal;
            var undersized = mix.Undersized;
            var overground = mix.Overground;
            var boiled = mix.Boiled;
            var isMud = mix.IsMud;
            var isExperiment = mix.IsExperiment;
            var other = mix.Other;
            var comment = mix.Comment;
            if (!MixCorrectWindow.Correct(
                ref formNumber, 
                ref recipeNumber,
                ref normal,
                ref undersized,
                ref overground,
                ref boiled,
                ref isMud,
                ref isExperiment,
                ref other,
                ref comment,
                "Уточнение данных по заливке",
                mix.Number,
                mix.DateTime,
                mix.MixerTemperature,
                mix.SetSandMud,
                mix.ActSandMud,
                mix.SetRevertMud,
                mix.ActRevertMud))
                return;
            mix.FormNumber = formNumber;
            mix.RecipeNumber = recipeNumber;
            mix.Normal = normal;
            mix.Undersized = undersized;
            mix.Overground = overground;
            mix.Boiled = boiled;
            mix.IsMud = isMud;
            mix.IsExperiment = isExperiment;
            mix.Other = other;
            mix.Comment = comment;
            _Mixes.Update(mix);
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

        #region Support Methods

        /// <summary> Загрузка данных из бд во вьюмодель </summary>
        private void LoadData()
        {
            try
            {
                Mixes.Clear();
                foreach (var mix in _Mixes.GetAll()
                    .Where(m => m.DateTime >= DateTime.Now.AddHours(-2))
                    .OrderByDescending(m => m.DateTime))
                {
                    Mixes.Add(mix);
                }
                ConnectToDataBase = true;
            }
            catch (ArgumentNullException ex)
            {
                MessageBox.Show($"{DateTime.Now} Ошибка отсутствия аргумента при доступе к базе данных {ex.Message}, Подробности: {ex?.InnerException?.Message}", "Ошибка связи с базой данных", MessageBoxButton.OK, MessageBoxImage.Error);
                ConnectToDataBase = false;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                MessageBox.Show($"{DateTime.Now} Ошибка конкурентного доступа к базе данных в базе данных {ex.Message}, Подробности: {ex?.InnerException?.Message}", "Ошибка связи с базой данных", MessageBoxButton.OK, MessageBoxImage.Error);
                ConnectToDataBase = false;
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"{DateTime.Now} Ошибка обновления данных в базе данных {ex.Message}, Подробности: {ex?.InnerException?.Message}", "Ошибка связи с базой данных", MessageBoxButton.OK, MessageBoxImage.Error);
                ConnectToDataBase = false;
            }
            catch (RetryLimitExceededException ex)
            {
                MessageBox.Show($"{DateTime.Now} Превышение лимита попыток подключения к базе данных {ex.Message}, Подробности: {ex?.InnerException?.Message}", "Ошибка связи с базой данных", MessageBoxButton.OK, MessageBoxImage.Error);
                ConnectToDataBase = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{DateTime.Now} Неизвестная ошибка связи с базой данных {ex.Message}, Подробности: {ex?.InnerException?.Message}", "Ошибка связи с базой данных", MessageBoxButton.OK, MessageBoxImage.Error);
                ConnectToDataBase = false;
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
        /// <summary> Обновление только самой важной передней части данных </summary>
        private void UpdateFirstData()
        {
            OnPropertyChanged(nameof(TimeSpanCurrentShiftMixes));
            OnPropertyChanged(nameof(CountCurrentShiftMixes));
            OnPropertyChanged(nameof(CountNormalCurrentShiftMixes));
            OnPropertyChanged(nameof(CurrentDateTime));
            LoadData();
        }

        #endregion

    }
}
