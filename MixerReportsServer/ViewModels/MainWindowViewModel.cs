﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using MixerReports.lib.Data.Base;
using MixerReports.lib.Interfaces;
using MixerReports.lib.Models;
using MixerReportsServer.Commands;
using MixerReportsServer.Models;
using MixerReportsServer.ViewModels.Base;
using Newtonsoft.Json;

namespace MixerReportsServer.ViewModels
{
    class MainWindowViewModel : ViewModel
    {
        //private readonly IRepository<Mix> _Mixes;
        private readonly Timer _timer;

        //private readonly ISharp7ReaderService _sharp7ReaderService;
        private readonly ISharp7MixReaderService _sharp7MixReaderService;

        private Mix _mix;

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

        #region Настройки

        private Settings _Settings;

        /// <summary> Настройки </summary>
        public Settings Settings
        {
            get => _Settings;
            set => Set(ref _Settings, value);
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

        public MainWindowViewModel(ISharp7MixReaderService sharp7MixReaderService)
        {
            //_sharp7ReaderService = sharp7ReaderService;
            _sharp7MixReaderService = sharp7MixReaderService;
            if (File.Exists("data.json"))
                Settings = JsonConvert.DeserializeObject<Settings>(System.IO.File.ReadAllText("data.json"));
            else
                Settings = new Settings();
            _timer = new Timer(3_000); //опрос каждые 3 секунды
            _timer.Elapsed += TimerOnElapsed;
            _timer.Start();
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

        private ICommand _ClosedCommand;

        /// <summary> Команда закрытие приложения </summary>
        public ICommand ClosedCommand => _ClosedCommand ??=
            new LambdaCommand(OnClosedCommandExecuted, CanClosedCommandExecute);

        private bool CanClosedCommandExecute(object p) => true;

        private void OnClosedCommandExecuted(object p)
        {
            System.IO.File.WriteAllText("data.json", JsonConvert.SerializeObject(Settings));
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

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Enabled = false;
            int seconds = 0;
            int error = -1;
            var mixGetted = false;
            try
            {
                if (Settings.Changed)
                {
                    //_sharp7ReaderService.SetSecondsToRead = Settings.SetSecondsToRead;
                    //_sharp7ReaderService.Address = Settings.Address;
                    //_sharp7ReaderService.AluminiumProp = Settings.AluminiumProp;
                    //_sharp7ReaderService.SecondsCorrect = Settings.SecondsCorrect;
                    //_sharp7ReaderService.SetSecondsToCorrect = Settings.SetSecondsToCorrect;
                    Settings.Changed = false;
                }
                mixGetted = _sharp7MixReaderService.TryNewMixTick(out seconds, out error, out _mix);
                //mixGetted = _sharp7ReaderService.GetMixOnTime(out seconds, out error, ref _mix);
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
                    //MixInfo = $"Прошло {seconds} из {_sharp7ReaderService.SetSecondsToRead} секунд заливки";
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
