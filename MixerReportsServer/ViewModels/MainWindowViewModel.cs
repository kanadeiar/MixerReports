using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Win32;
using MixerRaports.dbf.Interfaces;
using MixerReports.lib.Data.Base;
using MixerReports.lib.Interfaces;
using MixerReports.lib.Models;
using MixerReportsServer.Commands;
using MixerReportsServer.ViewModels.Base;
using MixerReportsServer.Windows;

namespace MixerReportsServer.ViewModels
{
    class MainWindowViewModel : ViewModel
    {
        #region Поля

        private readonly Timer _timer;

        private readonly ISharp7MixReaderService _sharp7MixReaderService;

        private readonly IDBFConverterService _dbfConverterService;

        private Mix _mix;

        #endregion

        #region Свойства

        #region Заливки

        /// <summary> Заливки </summary>
        public ObservableCollection<Mix> Mixes { get; } = new ();

        /// <summary> Последние заливки за два часа </summary>
        public ICollection<Mix> LastMixes =>
            Mixes.Where(m => m.DateTime >= DateTime.Now.AddHours(-2))
                .OrderByDescending(m => m.DateTime)
                .ToList();
        /// <summary> Заливки за текущюю смену </summary>
        public ICollection<Mix> NowShiftMixes =>
            Mixes.Where(m => m.DateTime >= GetDateTimesFrom())
                .OrderByDescending(m => m.DateTime)
                .ToList();

        #endregion

        #region Связь с базой данных

        private string _Log;

        /// <summary> Лог работы </summary>
        public string Log
        {
            get => _Log;
            set => Set(ref _Log, value);
        }

        private string _MixInfo;

        /// <summary> Текст информация по заливке </summary>
        public string MixInfo
        {
            get => _MixInfo;
            set => Set(ref _MixInfo, value);
        }

        private bool _ConnectToPLC;

        /// <summary> Инфа про соединение с ПЛК </summary>
        public bool ConnectToPLC
        {
            get => _ConnectToPLC;
            set
            {
                Set(ref _ConnectToPLC, value);
                OnPropertyChanged(nameof(ConnectToPLCStr));
            }
        }
        public string ConnectToPLCStr => (ConnectToPLC) ? "Соединение с ПЛК установлено" : "Соединение с ПЛК потеряно";

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

        #region Редактирование заливок

        private DateTime _FilterArchivesBeginDateTime = DateTime.Today.AddDays(-1);

        /// <summary> Выбранная дата начала архивных данных </summary>
        public DateTime FilterArchivesBeginDateTime
        {
            get => _FilterArchivesBeginDateTime;
            set
            {
                Set(ref _FilterArchivesBeginDateTime, value);
                LoadEditData();
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
                LoadEditData();
            }
        }

        /// <summary> Заливки доступные для редактирования </summary>
        public ObservableCollection<Mix> EditMixes { get; } = new();

        private Mix _selectedEditMix;

        /// <summary> Выделенная заливка в списке </summary>
        public Mix SelectedEditMix
        {
            get => _selectedEditMix;
            set => Set(ref _selectedEditMix, value);
        }

        #endregion

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

        public MainWindowViewModel(ISharp7MixReaderService sharp7MixReaderService, IDBFConverterService dbfConverterService)
        {
            _sharp7MixReaderService = sharp7MixReaderService;
            _dbfConverterService = dbfConverterService;
            _timer = new Timer(3_000); //опрос каждые 3 секунды
            _timer.Elapsed += TimerOnElapsed;
            _timer.Start();
        }

        #region Команды

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

        #region Импорт данных из DBF файла

        private ICommand _ImportDataFromDBFCommand;

        /// <summary> Импорт данных из DBF файлов в базу данных </summary>
        public ICommand ImportDataFromDBFCommand => _ImportDataFromDBFCommand ??=
            new LambdaCommand(OnImportDataFromDBFCommandExecuted, CanImportDataFromDBFCommandExecute);
        private bool CanImportDataFromDBFCommandExecute(object p) => true;
        private void OnImportDataFromDBFCommandExecuted(object p)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Выбор один файл или множество для чтения DBF",
                InitialDirectory = Environment.CurrentDirectory,
                Filter = "Файлы DBF (*.dbf)|*.dbf|Все файлы (*.*)|*.*",
                Multiselect = true,
            };
            if (dialog.ShowDialog() != true)
                return;
            var dbfFiles = dialog.FileNames;
            if (dbfFiles.Length <= 0)
            {
                MessageBox.Show($"Не выбраны файлы для чтения", "Ошибка выбора файлов", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var list = new List<Mix>();

            try
            {
                foreach (var fileName in dbfFiles)
                {
                    if (File.Exists(fileName))
                    {
                        list.AddRange(_dbfConverterService.GetMixesFromDBF(fileName));
                    }
                }
                MessageBox.Show($"Успешно удалось прочитать из файлов информацию о {list.Count} заливок", "Импорт данных по заливкам в базу данных", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Не удалось прочитать информацию из файлов о заливках, ошибка:\n{e.Message}", "Ошибка импорта данных", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var options = new DbContextOptionsBuilder<SPBSUMixerRaportsEntities>()
                    .UseSqlServer(App.DefaultConnectionString, o => o.EnableRetryOnFailure())
                    .ConfigureWarnings(w => w.Throw(RelationalEventId.BoolWithDefaultWarning)).Options;
                using (var db = new SPBSUMixerRaportsEntities(options))
                {
                    var mindate = list.Min(l => l.DateTime);
                    var maxdate = list.Max(l => l.DateTime);
                    if (db.Mixes.Any(m => m.DateTime >= mindate && m.DateTime <= maxdate))
                    {
                        if (MessageBox.Show("В базе данных уже имеются данные по заливкам за диапазон времени заливок из DBF файла. Новые данные из файла будут помещены рядом с имеющимися и возможно будут дублироваться. Действительно добавить заливки?", "Добавление новых данных в базу данных", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                            return;
                    }

                    db.Mixes.AddRange(list);
                    db.SaveChanges();
                }
                OnPropertyChanged(nameof(LastMixes));
                OnPropertyChanged(nameof(NowShiftMixes));
                AddToLog($"{DateTime.Now:dd.MM.yyyy HH:mm:ss}, Успешно добавлены заливки импортом из DBF файла в количестве {list.Count} штук.");
                ConnectToDataBase = true;
            }
            catch (DbUpdateException ex)
            {
                AddToLog($"{DateTime.Now} Ошибка обновления данных в базе данных {ex.Message}, Подробности: {ex?.InnerException?.Message} Данные: {PrintDatas(list[0])} ... {list.Count} штук.");
                ConnectToDataBase = false;
            }
            catch (RetryLimitExceededException ex)
            {
                AddToLog($"{DateTime.Now} Превышение лимита попыток подключения к базе данных {ex.Message}, Подробности: {ex?.InnerException?.Message} Данные: {PrintDatas(list[0])} ... {list.Count} штук.");
                ConnectToDataBase = false;
            }
            catch (Exception ex)
            {
                AddToLog($"{DateTime.Now} Ошибка связи с базой данных {ex.Message}, Подробности: {ex?.InnerException?.Message} Данные: {PrintDatas(list[0])} ... {list.Count} штук.");
                ConnectToDataBase = false;
            }
        }

        #endregion

        #region Редактирование заливок

        private ICommand _UpdateEditMixesCommand;

        /// <summary> Команда обновить заливки доступные для редактирования </summary>
        public ICommand UpdateEditMixesCommand => _UpdateEditMixesCommand ??=
            new LambdaCommand(OnUpdateEditMixesCommandExecuted, CanUpdateEditMixesCommandExecute);

        private bool CanUpdateEditMixesCommandExecute(object p) => true;

        private void OnUpdateEditMixesCommandExecuted(object p)
        {
            LoadEditData();
        }

        private ICommand _AddNewMixCommand;

        /// <summary> Команда добавления новой заливки </summary>
        public ICommand AddNewMixCommand => _AddNewMixCommand ??=
            new LambdaCommand(OnAddNewMixCommandExecuted, CanAddNewMixCommandExecute);

        private bool CanAddNewMixCommandExecute(object p) => true;

        private void OnAddNewMixCommandExecuted(object p)
        {
            if (!MixEditWindow.Create(out Mix mix))
                return;
            var options = new DbContextOptionsBuilder<SPBSUMixerRaportsEntities>()
                .UseSqlServer(App.DefaultConnectionString, o => o.EnableRetryOnFailure())
                .ConfigureWarnings(w => w.Throw(RelationalEventId.BoolWithDefaultWarning)).Options;
            try
            {
                using (var db = new SPBSUMixerRaportsEntities(options))
                {
                    db.Mixes.Add(mix);
                    db.SaveChanges();
                    EditMixes.Add(mix);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось добавить новую заливку в базу данных, ошибка: {ex.Message}, внутренняя: {ex?.InnerException?.Message}", "Ошибка добавления новой заливки в базу данных", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private ICommand _EditSelectedMixCommand;

        /// <summary> Команда редактирования выбранной заливки </summary>
        public ICommand EditSelectedMixCommand => _EditSelectedMixCommand ??=
            new LambdaCommand(OnEditSelectedMixCommandExecuted, CanEditSelectedMixCommandExecute);

        private bool CanEditSelectedMixCommandExecute(object p) => p is Mix;

        private void OnEditSelectedMixCommandExecuted(object p)
        {
            if (!(p is Mix mix))
                return;
            if (!MixEditWindow.ShowEdit(ref mix))
                return;
            var options = new DbContextOptionsBuilder<SPBSUMixerRaportsEntities>()
                .UseSqlServer(App.DefaultConnectionString, o => o.EnableRetryOnFailure())
                .ConfigureWarnings(w => w.Throw(RelationalEventId.BoolWithDefaultWarning)).Options;
            try
            {
                using (var db = new SPBSUMixerRaportsEntities(options))
                {
                    db.Mixes.Update(mix);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось изменить выбранную заливку в базе данных, ошибка: {ex.Message}, внутренняя: {ex?.InnerException?.Message}", "Ошибка изменения выбранной заливки в базе данных", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private ICommand _DeleteSelectedMixCommand;

        /// <summary> Команда удаления выбранной заливки из базы данных </summary>
        public ICommand DeleteSelectedMixCommand => _DeleteSelectedMixCommand ??=
            new LambdaCommand(OnDeleteSelectedMixCommandExecuted, CanDeleteSelectedMixCommandExecute);

        private bool CanDeleteSelectedMixCommandExecute(object p) => p is Mix;

        private void OnDeleteSelectedMixCommandExecuted(object p)
        {
            if (!(p is Mix mix))
                return;
            if (MessageBox.Show("Действительно удалить выделенную заливку?", "Удаление заливки из базы данных", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                return;
            var options = new DbContextOptionsBuilder<SPBSUMixerRaportsEntities>()
                .UseSqlServer(App.DefaultConnectionString, o => o.EnableRetryOnFailure())
                .ConfigureWarnings(w => w.Throw(RelationalEventId.BoolWithDefaultWarning)).Options;
            try
            {
                using (var db = new SPBSUMixerRaportsEntities(options))
                {
                    db.Mixes.Remove(mix);
                    db.SaveChanges();
                    EditMixes.Remove(mix);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось удалить выбранную заливку в базе данных, ошибка: {ex.Message}, внутренняя: {ex?.InnerException?.Message}", "Ошибка удаления выбранной заливки в базе данных", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

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

        #region Для таймера

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Enabled = false;
            int seconds = 0;
            int error = -1;
            var mixGetted = false;
            try
            {
                mixGetted = _sharp7MixReaderService.TryNewMixTick(out seconds, out error, out _mix);
                if (mixGetted)
                {
                    ConnectToPLC = true;
                }
                if (error != 0)
                {
                    AddToLog($"{DateTime.Now} Ошибка чтения контроллера, код ошибки: {error}");
                    ConnectToPLC = false;
                }
                else
                {
                    MixInfo = $"Прошло {seconds} секунд заливки";
                    ConnectToPLC = true;
                }
            }
            catch (Exception ex)
            {
                AddToLog($"{DateTime.Now} Ошибка чтения контроллера заливки {ex.Message}");
                ConnectToPLC = false;
            }
            if (mixGetted && _mix != null)
            {
                try
                {
                    _mix.Number = NowShiftMixes
                        .OrderByDescending(r => r.Number)
                        .FirstOrDefault()?.Number + 1 ?? 1;
                    var options = new DbContextOptionsBuilder<SPBSUMixerRaportsEntities>()
                        .UseSqlServer(App.DefaultConnectionString, o => o.EnableRetryOnFailure())
                        .ConfigureWarnings(w => w.Throw(RelationalEventId.BoolWithDefaultWarning)).Options;
                    using (var db = new SPBSUMixerRaportsEntities(options))
                    {
                        db.Mixes.Add(_mix);
                        db.SaveChanges();
                    }
                    Mixes.Add(_mix);
                    OnPropertyChanged(nameof(LastMixes));
                    OnPropertyChanged(nameof(NowShiftMixes));
                    AddToLog(
                        $"{DateTime.Now:dd.MM.yyyy HH:mm:ss}, Заливка: {_mix.DateTime:dd.MM.yyyy HH:mm:ss}, номер заливки: {_mix.Number}, номер формы: {_mix.FormNumber}, температура: {_mix.MixerTemperature}, норма: {_mix.NormalStr} ");
                    ConnectToDataBase = true;
                }
                catch (ArgumentNullException ex)
                {
                    AddToLog(
                        $"{DateTime.Now} Ошибка отсутствия аргумента при доступе к базе данных {ex.Message}, Подробности: {ex?.InnerException?.Message}, Данные: {PrintDatas(_mix)}");
                    ConnectToDataBase = false;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    AddToLog(
                        $"{DateTime.Now} Ошибка конкурентного доступа к базе данных в базе данных {ex.Message}, Подробности: {ex?.InnerException?.Message}, Данные: {PrintDatas(_mix)}");
                    ConnectToDataBase = false;
                }
                catch (DbUpdateException ex)
                {
                    AddToLog(
                        $"{DateTime.Now} Ошибка обновления данных в базе данных {ex.Message}, Подробности: {ex?.InnerException?.Message}, Данные: {PrintDatas(_mix)}");
                    ConnectToDataBase = false;
                }
                catch (RetryLimitExceededException ex)
                {
                    AddToLog(
                        $"{DateTime.Now} Превышение лимита попыток подключения к базе данных {ex.Message}, Подробности: {ex?.InnerException?.Message}, Данные: {PrintDatas(_mix)}");
                    ConnectToDataBase = false;
                }
                catch (Exception ex)
                {
                    AddToLog($"{DateTime.Now} Ошибка связи с базой данных {ex.Message}, Подробности: {ex?.InnerException?.Message}, Данные: {PrintDatas(_mix)}");
                    ConnectToDataBase = false;
                }
            }

            _timer.Enabled = true;
        }

        #endregion

        /// <summary> Загрузка данных из бд в вьюмодель </summary>
        private void LoadData()
        {
            var options = new DbContextOptionsBuilder<SPBSUMixerRaportsEntities>()
                .UseSqlServer(App.DefaultConnectionString, o => o.EnableRetryOnFailure())
                .ConfigureWarnings(w => w.Throw(RelationalEventId.BoolWithDefaultWarning)).Options;
            Mixes.Clear();
            using (var db = new SPBSUMixerRaportsEntities(options))
            {
                db.Database.Migrate();
                foreach (var mix in db.Mixes.Where(m => m.DateTime >= DateTime.Now.AddDays(- 7)))
                {
                    Mixes.Add(mix);
                }
            }
            OnPropertyChanged(nameof(LastMixes));
            OnPropertyChanged(nameof(NowShiftMixes));
            ConnectToDataBase = true;
        }
        /// <summary> Загрузка данных из бд во вьюмодель для редактирования </summary>
        private void LoadEditData()
        {
            var options = new DbContextOptionsBuilder<SPBSUMixerRaportsEntities>()
                .UseSqlServer(App.DefaultConnectionString, o => o.EnableRetryOnFailure())
                .ConfigureWarnings(w => w.Throw(RelationalEventId.BoolWithDefaultWarning)).Options;
            EditMixes.Clear();
            using (var db = new SPBSUMixerRaportsEntities(options))
            {
                foreach (var mix in db.Mixes.Where(m => m.DateTime >= FilterArchivesBeginDateTime.AddHours(8) && m.DateTime <= FilterArchivesEndDateTime.AddHours(24).AddHours(8))) 
                {
                    EditMixes.Add(mix);
                }
            }
            ConnectToDataBase = true;
        }
        private DateTime GetDateTimesFrom()
        {
            var dateFrom = DateTime.Today.AddHours(8);
            if (DateTime.Now.Hour < 8)
                dateFrom = dateFrom.AddHours(-12);
            if (DateTime.Now.Hour >= 20)
                dateFrom = dateFrom.AddHours(12);
            return dateFrom;
        }
        private void AddToLog(string text)
        {
            Log += text + "\n";
            using var fileLog = File.AppendText("log.txt");
            fileLog.Write(text + "\n");
        }
        private static string PrintDatas(Mix row)
        {
            return $" {row.DateTime:g} {row.Number} Форма №{row.FormNumber} {row.MixerTemperature}, " +
                      $"обратный шлам: {row.SetRevertMud} {row.ActRevertMud} песчаный шлам: {row.SetSandMud} {row.ActSandMud} \n" +
                      $"холодная вода: {row.SetColdWater} {row.ActColdWater} горячая вода {row.SetHotWater} {row.ActHotWater} " +
                      $"ипв1: {row.SetMixture1} {row.ActMixture1} ипв2: {row.SetMixture2} {row.ActMixture2} \n" +
                      $"цемент1: {row.SetCement1} {row.ActCement1} цемент2: {row.SetCement2} {row.ActCement2} " +
                      $"алюминий1: {row.SetAluminium1} {row.ActAluminium1} алюминий2: {row.SetAluminium2} {row.ActAluminium2} \n" +
                      $"песок в шламе: {row.SandInMud} плотность песка: {row.DensitySandMud} плотность обратки: {row.DensityRevertMud}";
        }
        #endregion
    }
}
