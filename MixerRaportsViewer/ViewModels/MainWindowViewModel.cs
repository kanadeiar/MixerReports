using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MixerRaportsViewer.Commands;
using MixerReports.lib.Interfaces;
using MixerReports.lib.Models;

namespace MixerRaportsViewer.ViewModels
{
    class MainWindowViewModel : Base.ViewModel
    {
        #region Данные

        private readonly IRepository<Mix> _Mixes;

        private readonly Timer _timer;

        #endregion

        #region Свойства

        #region Данные по заливкам текущей смены

        /// <summary> Заливки текущей смены </summary>
        public ObservableCollection<Mix> Mixes { get; } = new();

        #endregion

        #region Данные по сменам за выбранную дату - дневная и ночная

        private DateTime _ShiftSelectDateTime = DateTime.Today.AddDays(-1);

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

        private bool _SetOnlyBadShiftsMixes;

        /// <summary> Показывать только данные плохих заливок </summary>
        public bool SetOnlyBadShiftsMixes
        {
            get => _SetOnlyBadShiftsMixes;
            set
            {
                Set(ref _SetOnlyBadShiftsMixes, value);
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
                if (SetOnlyBadShiftsMixes)
                    mixs = mixs.Where(m => m.Normal == false).ToList();
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
                if (SetOnlyBadShiftsMixes)
                    mixs = mixs.Where(m => m.Normal == false).ToList();
                return mixs;
            }
        }

        #endregion

        #region Архивные данные за выбранный диапазон дат

        private DateTime _FilterArchivesBeginDateTime = DateTime.Today.AddDays(-1);

        /// <summary> Выбранная дата начала архивных данных </summary>
        public DateTime FilterArchivesBeginDateTime
        {
            get => _FilterArchivesBeginDateTime;
            set
            {
                Set(ref _FilterArchivesBeginDateTime, value);
                OnPropertyChanged(nameof(FilteredArchivesMixes));
                OnPropertyChanged(nameof(CountArchivesMixes));
                OnPropertyChanged(nameof(CountNormalArchivesMixes));
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
                OnPropertyChanged(nameof(CountArchivesMixes));
                OnPropertyChanged(nameof(CountNormalArchivesMixes));
            }
        }

        private bool _setOnlyBadArchivesMixes;

        /// <summary> Выбор - показывать только плохие заливки </summary>
        public bool SetOnlyBadArchivesMixes
        {
            get => _setOnlyBadArchivesMixes;
            set
            {
                Set(ref _setOnlyBadArchivesMixes, value);
                OnPropertyChanged(nameof(FilteredArchivesMixes));
                OnPropertyChanged(nameof(CountArchivesMixes));
                OnPropertyChanged(nameof(CountNormalArchivesMixes));
            }
        }

        /// <summary> Количество заливок дневной смены </summary>
        public int CountArchivesMixes
        {
            get
            {
                return _Mixes.GetAll()
                    .Count(m => m.DateTime >= FilterArchivesBeginDateTime && m.DateTime < FilterArchivesEndDateTime.AddHours(24));
            }
        }
        /// <summary> Количество нормальных заливок </summary>
        public int CountNormalArchivesMixes
        {
            get
            {
                return _Mixes.GetAll()
                    .Count(m => m.DateTime >= FilterArchivesBeginDateTime && m.DateTime < FilterArchivesEndDateTime.AddHours(24));
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
                if (SetOnlyBadArchivesMixes)
                    mixs = mixs.Where(m => m.Normal == false).ToList();
                return mixs;
            }
        }

        #endregion

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

        /// <summary> Текущее время </summary>
        public DateTime CurrentDateTime => DateTime.Now;

        #endregion

        #endregion

        public MainWindowViewModel(IRepository<Mix> mixes)
        {
            _Mixes = mixes;
            //автообновление 1 раз в секунду текущего времени
            _timer = new Timer(1_000);
            _timer.Elapsed += (s, a) => OnPropertyChanged(nameof(CurrentDateTime));
            _timer.Start();
        }

        #region Команды

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
            OnPropertyChanged(nameof(CountArchivesMixes));
            OnPropertyChanged(nameof(CountNormalArchivesMixes));
        }

        #endregion

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

        /// <summary> Загрузка данных из бд во вьюмодель </summary>
        private void LoadData()
        {
            try
            {
                var date = GetDateTimesFrom(DateTime.Now);
                Mixes.Clear();
                foreach (var mix in _Mixes.GetAll()
                    .Where(m => m.DateTime >= date)
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

        #endregion
    }
}
