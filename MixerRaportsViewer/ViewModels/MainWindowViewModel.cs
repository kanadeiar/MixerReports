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
using MixerRaportsViewer.Raports;
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
        public ObservableCollection<Mix> CurrentShiftMixes { get; } = new();
        /// <summary> Время сколько длиться эта смена </summary>
        public string TimeSpanCurrentShiftMixes
        {
            get
            {
                var span = DateTime.Now - GetDateTimesFrom(DateTime.Now);
                return $"{span.Hours} час. {span.Minutes} мин. {span.Seconds} сек.";
            }
        }
        private int _CountCurrentShiftMixes;
        /// <summary> Количество всех заливок </summary>
        public int CountCurrentShiftMixes
        {
            get => _CountCurrentShiftMixes;
            set => Set(ref _CountCurrentShiftMixes, value);
        }
        private int _CountCurrentNormalShiftMixes;
        /// <summary> Количество нормальных заливок </summary>
        public int CountCurrentNormalShiftMixes
        {
            get => _CountCurrentNormalShiftMixes;
            set => Set(ref _CountCurrentNormalShiftMixes, value);
        }

        /// <summary> Заливки предидущей смены </summary>
        public ObservableCollection<Mix> PreShiftMixes { get; } = new ();
        /// <summary> Время и дата начала предидущей смены </summary>
        public string TimeBeginPreShiftMixes
        {
            get
            {
                var date = GetDateTimesFrom(DateTime.Now).AddHours(-12);
                return $"{date:HH:mm:ss dd.MM.yyyy}";
            }
        }
        private int _CountPreShiftMixes;
        /// <summary> Количество всех заливок предидущей смены </summary>
        public int CountPreShiftMixes
        {
            get => _CountPreShiftMixes;
            set => Set(ref _CountPreShiftMixes, value);
        }
        private int _CountPreNormalShiftMixes;
        /// <summary> Количество нормальных заливок предидущей смены </summary>
        public int CountPreNormalShiftMixes
        {
            get => _CountPreNormalShiftMixes;
            set => Set(ref _CountPreNormalShiftMixes, value);
        }

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

        #region Статистические данные по сменам выбранной даты

        /// <summary> Средняя температура </summary>
        public float CurrAverageTemperature => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(12)).Average(m => m.MixerTemperature);
        /// <summary> Сумма заданий песчаный шлам </summary>
        public float CurrSumSetSandMud => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(12)).Sum(m => m.SetSandMud);
        /// <summary> Сумма факт песчаный шлам </summary>
        public float CurrSumActSandMud => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(12)).Sum(m => m.ActSandMud);
        /// <summary> Сумма заданий обратный шлам </summary>
        public float CurrSumSetRevertMud => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(12)).Sum(m => m.SetRevertMud);
        /// <summary> Сумма факт обратный шлам </summary>
        public float CurrSumActRevertMud => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(12)).Sum(m => m.ActRevertMud);
        /// <summary> Сумма заданий холодная вода </summary>
        public float CurrSumSetColdWater => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(12)).Sum(m => m.SetColdWater);
        /// <summary> Сумма факт холодная вода </summary>
        public float CurrSumActColdWater => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(12)).Sum(m => m.ActColdWater);
        /// <summary> Сумма заданий горячая вода </summary>
        public float CurrSumSetHotWater => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(12)).Sum(m => m.SetHotWater);
        /// <summary> Сумма факт горячая вода </summary>
        public float CurrSumActHotWater => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(12)).Sum(m => m.ActHotWater);
        /// <summary> Сумма заданий ИПВ1 </summary>
        public float CurrSumSetMixture1 => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(12)).Sum(m => m.SetMixture1);
        /// <summary> Сумма факт ИПВ1 </summary>
        public float CurrSumActMixture1 => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(12)).Sum(m => m.ActMixture1);
        /// <summary> Сумма заданий ИПВ2 </summary>
        public float CurrSumSetMixture2 => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(12)).Sum(m => m.SetMixture2);
        /// <summary> Сумма факт ИПВ2 </summary>
        public float CurrSumActMixture2 => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(12)).Sum(m => m.ActMixture2);
        /// <summary> Сумма заданий цемент1 </summary>
        public float CurrSumSetCement1 => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(12)).Sum(m => m.SetCement1);
        /// <summary> Сумма факт цемент1 </summary>
        public float CurrSumActCement1 => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(12)).Sum(m => m.ActCement1);
        /// <summary> Сумма заданий цемент2 </summary>
        public float CurrSumSetCement2 => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(12)).Sum(m => m.SetCement2);
        /// <summary> Сумма факт цемент2 </summary>
        public float CurrSumActCement2 => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(12)).Sum(m => m.ActCement2);
        /// <summary> Сумма заданий алюминий1 </summary>
        public float CurrSumSetAluminium1 => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(12)).Sum(m => m.SetAluminium1);
        /// <summary> Сумма факт алюминий1 </summary>
        public float CurrSumActAluminium1 => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(12)).Sum(m => m.ActAluminium1);
        /// <summary> Сумма заданий алюминий2 </summary>
        public float CurrSumSetAluminium2 => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(12)).Sum(m => m.SetAluminium2);
        /// <summary> Сумма факт алюминий2 </summary>
        public float CurrSumActAluminium2 => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(12)).Sum(m => m.ActAluminium2);
        /// <summary> Количество плохих заливок </summary>
        public int CurrCountBadMixes => _Mixes.GetAll()
            .Count(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(12) && m.Normal == false);
        /// <summary> Количество недоростков </summary>
        public int CurrCountUndersizedMixes => _Mixes.GetAll()
            .Count(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(12) && m.Undersized);
        /// <summary> Количество переростков </summary>
        public int CurrCountOvergroundMixes => _Mixes.GetAll()
            .Count(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(12) && m.Overground);
        /// <summary> Количество закипевших массивов </summary>
        public int CurrCountBoiledMixes => _Mixes.GetAll()
            .Count(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(12) && m.Boiled);
        /// <summary> Количество скинутых в шлам массивов </summary>
        public int CurrCountMudMixes => _Mixes.GetAll()
            .Count(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(12) && m.IsMud);


        /// <summary> Средняя температура </summary>
        public float PreAverageTemperature => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8).AddHours(12) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(24)).Average(m => m.MixerTemperature);
        /// <summary> Сумма заданий песчаный шлам </summary>
        public float PreSumSetSandMud => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8).AddHours(12) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(24)).Sum(m => m.SetSandMud);
        /// <summary> Сумма факт песчаный шлам </summary>
        public float PreSumActSandMud => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8).AddHours(12) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(24)).Sum(m => m.ActSandMud);
        /// <summary> Сумма заданий обратный шлам </summary>
        public float PreSumSetRevertMud => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8).AddHours(12) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(24)).Sum(m => m.SetRevertMud);
        /// <summary> Сумма факт обратный шлам </summary>
        public float PreSumActRevertMud => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8).AddHours(12) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(24)).Sum(m => m.ActRevertMud);
        /// <summary> Сумма заданий холодная вода </summary>
        public float PreSumSetColdWater => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8).AddHours(12) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(24)).Sum(m => m.SetColdWater);
        /// <summary> Сумма факт холодная вода </summary>
        public float PreSumActColdWater => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8).AddHours(12) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(24)).Sum(m => m.ActColdWater);
        /// <summary> Сумма заданий горячая вода </summary>
        public float PreSumSetHotWater => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8).AddHours(12) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(24)).Sum(m => m.SetHotWater);
        /// <summary> Сумма факт горячая вода </summary>
        public float PreSumActHotWater => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8).AddHours(12) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(24)).Sum(m => m.ActHotWater);
        /// <summary> Сумма заданий ИПВ1 </summary>
        public float PreSumSetMixture1 => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8).AddHours(12) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(24)).Sum(m => m.SetMixture1);
        /// <summary> Сумма факт ИПВ1 </summary>
        public float PreSumActMixture1 => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8).AddHours(12) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(24)).Sum(m => m.ActMixture1);
        /// <summary> Сумма заданий ИПВ2 </summary>
        public float PreSumSetMixture2 => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8).AddHours(12) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(24)).Sum(m => m.SetMixture2);
        /// <summary> Сумма факт ИПВ2 </summary>
        public float PreSumActMixture2 => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8).AddHours(12) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(24)).Sum(m => m.ActMixture2);
        /// <summary> Сумма заданий цемент1 </summary>
        public float PreSumSetCement1 => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8).AddHours(12) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(24)).Sum(m => m.SetCement1);
        /// <summary> Сумма факт цемент1 </summary>
        public float PreSumActCement1 => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8).AddHours(12) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(24)).Sum(m => m.ActCement1);
        /// <summary> Сумма заданий цемент2 </summary>
        public float PreSumSetCement2 => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8).AddHours(12) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(24)).Sum(m => m.SetCement2);
        /// <summary> Сумма факт цемент2 </summary>
        public float PreSumActCement2 => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8).AddHours(12) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(24)).Sum(m => m.ActCement2);
        /// <summary> Сумма заданий алюминий1 </summary>
        public float PreSumSetAluminium1 => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8).AddHours(12) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(24)).Sum(m => m.SetAluminium1);
        /// <summary> Сумма факт алюминий1 </summary>
        public float PreSumActAluminium1 => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8).AddHours(12) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(24)).Sum(m => m.ActAluminium1);
        /// <summary> Сумма заданий алюминий2 </summary>
        public float PreSumSetAluminium2 => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8).AddHours(12) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(24)).Sum(m => m.SetAluminium2);
        /// <summary> Сумма факт алюминий2 </summary>
        public float PreSumActAluminium2 => _Mixes.GetAll()
            .Where(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8).AddHours(12) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(24)).Sum(m => m.ActAluminium2);
        /// <summary> Количество плохих заливок </summary>
        public int PreCountBadMixes => _Mixes.GetAll()
            .Count(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8).AddHours(12) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(24) && m.Normal == false);
        /// <summary> Количество недоростков </summary>
        public int PreCountUndersizedMixes => _Mixes.GetAll()
            .Count(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8).AddHours(12) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(24) && m.Undersized);
        /// <summary> Количество переростков </summary>
        public int PreCountOvergroundMixes => _Mixes.GetAll()
            .Count(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8).AddHours(12) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(24) && m.Overground);
        /// <summary> Количество закипевших массивов </summary>
        public int PreCountBoiledMixes => _Mixes.GetAll()
            .Count(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8).AddHours(12) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(24) && m.Boiled);
        /// <summary> Количество скинутых в шлам массивов </summary>
        public int PreCountMudMixes => _Mixes.GetAll()
            .Count(m => m.DateTime >= ShiftSelectDateTime.Date.AddHours(8).AddHours(12) && m.DateTime < ShiftSelectDateTime.Date.AddHours(8).AddHours(24) && m.IsMud);

        #endregion



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
                    .Count(m => m.DateTime >= FilterArchivesBeginDateTime.AddHours(8) && m.DateTime < FilterArchivesEndDateTime.AddHours(24).AddHours(8));
            }
        }
        /// <summary> Количество нормальных заливок </summary>
        public int CountNormalArchivesMixes
        {
            get
            {
                return _Mixes.GetAll()
                    .Count(m => m.DateTime >= FilterArchivesBeginDateTime.AddHours(8) && m.DateTime < FilterArchivesEndDateTime.AddHours(24).AddHours(8) && m.Normal);
            }
        }

        /// <summary> Отфильтрованные архивные данные </summary>
        public ICollection<Mix> FilteredArchivesMixes
        {
            get
            {
                var mixs = _Mixes.GetAll()
                    .Where(m => m.DateTime >= FilterArchivesBeginDateTime.AddHours(8) && m.DateTime < FilterArchivesEndDateTime.AddHours(24).AddHours(8))
                    .OrderBy(m => m.DateTime).ToList();
                if (SetOnlyBadArchivesMixes)
                    mixs = mixs.Where(m => m.Normal == false).ToList();
                return mixs;
            }
        }

        #region Статистические данные по архивным фильтрованным заливкам

        /// <summary> Средняя температура </summary>
        public float ArchAverageTemperature => _Mixes.GetAll()
            .Where(m => m.DateTime >= FilterArchivesBeginDateTime.AddHours(8) && m.DateTime < FilterArchivesEndDateTime.AddHours(24).AddHours(8)).Average(m => m.MixerTemperature);
        /// <summary> Сумма заданий песчаный шлам </summary>
        public float ArchSumSetSandMud => _Mixes.GetAll()
            .Where(m => m.DateTime >= FilterArchivesBeginDateTime.AddHours(8) && m.DateTime < FilterArchivesEndDateTime.AddHours(24).AddHours(8)).Sum(m => m.SetSandMud);
        /// <summary> Сумма факт песчаный шлам </summary>
        public float ArchSumActSandMud => _Mixes.GetAll()
            .Where(m => m.DateTime >= FilterArchivesBeginDateTime.AddHours(8) && m.DateTime < FilterArchivesEndDateTime.AddHours(24).AddHours(8)).Sum(m => m.ActSandMud);
        /// <summary> Сумма заданий обратный шлам </summary>
        public float ArchSumSetRevertMud => _Mixes.GetAll()
            .Where(m => m.DateTime >= FilterArchivesBeginDateTime.AddHours(8) && m.DateTime < FilterArchivesEndDateTime.AddHours(24).AddHours(8)).Sum(m => m.SetRevertMud);
        /// <summary> Сумма факт обратный шлам </summary>
        public float ArchSumActRevertMud => _Mixes.GetAll()
            .Where(m => m.DateTime >= FilterArchivesBeginDateTime.AddHours(8) && m.DateTime < FilterArchivesEndDateTime.AddHours(24).AddHours(8)).Sum(m => m.ActRevertMud);
        /// <summary> Сумма заданий холодная вода </summary>
        public float ArchSumSetColdWater => _Mixes.GetAll()
            .Where(m => m.DateTime >= FilterArchivesBeginDateTime.AddHours(8) && m.DateTime < FilterArchivesEndDateTime.AddHours(24).AddHours(8)).Sum(m => m.SetColdWater);
        /// <summary> Сумма факт холодная вода </summary>
        public float ArchSumActColdWater => _Mixes.GetAll()
            .Where(m => m.DateTime >= FilterArchivesBeginDateTime.AddHours(8) && m.DateTime < FilterArchivesEndDateTime.AddHours(24).AddHours(8)).Sum(m => m.ActColdWater);
        /// <summary> Сумма заданий горячая вода </summary>
        public float ArchSumSetHotWater => _Mixes.GetAll()
            .Where(m => m.DateTime >= FilterArchivesBeginDateTime.AddHours(8) && m.DateTime < FilterArchivesEndDateTime.AddHours(24).AddHours(8)).Sum(m => m.SetHotWater);
        /// <summary> Сумма факт горячая вода </summary>
        public float ArchSumActHotWater => _Mixes.GetAll()
            .Where(m => m.DateTime >= FilterArchivesBeginDateTime.AddHours(8) && m.DateTime < FilterArchivesEndDateTime.AddHours(24).AddHours(8)).Sum(m => m.ActHotWater);
        /// <summary> Сумма заданий ИПВ1 </summary>
        public float ArchSumSetMixture1 => _Mixes.GetAll()
            .Where(m => m.DateTime >= FilterArchivesBeginDateTime.AddHours(8) && m.DateTime < FilterArchivesEndDateTime.AddHours(24).AddHours(8)).Sum(m => m.SetMixture1);
        /// <summary> Сумма факт ИПВ1 </summary>
        public float ArchSumActMixture1 => _Mixes.GetAll()
            .Where(m => m.DateTime >= FilterArchivesBeginDateTime.AddHours(8) && m.DateTime < FilterArchivesEndDateTime.AddHours(24).AddHours(8)).Sum(m => m.ActMixture1);
        /// <summary> Сумма заданий ИПВ2 </summary>
        public float ArchSumSetMixture2 => _Mixes.GetAll()
            .Where(m => m.DateTime >= FilterArchivesBeginDateTime.AddHours(8) && m.DateTime < FilterArchivesEndDateTime.AddHours(24).AddHours(8)).Sum(m => m.SetMixture2);
        /// <summary> Сумма факт ИПВ2 </summary>
        public float ArchSumActMixture2 => _Mixes.GetAll()
            .Where(m => m.DateTime >= FilterArchivesBeginDateTime.AddHours(8) && m.DateTime < FilterArchivesEndDateTime.AddHours(24).AddHours(8)).Sum(m => m.ActMixture2);
        /// <summary> Сумма заданий цемент1 </summary>
        public float ArchSumSetCement1 => _Mixes.GetAll()
            .Where(m => m.DateTime >= FilterArchivesBeginDateTime.AddHours(8) && m.DateTime < FilterArchivesEndDateTime.AddHours(24).AddHours(8)).Sum(m => m.SetCement1);
        /// <summary> Сумма факт цемент1 </summary>
        public float ArchSumActCement1 => _Mixes.GetAll()
            .Where(m => m.DateTime >= FilterArchivesBeginDateTime.AddHours(8) && m.DateTime < FilterArchivesEndDateTime.AddHours(24).AddHours(8)).Sum(m => m.ActCement1);
        /// <summary> Сумма заданий цемент2 </summary>
        public float ArchSumSetCement2 => _Mixes.GetAll()
            .Where(m => m.DateTime >= FilterArchivesBeginDateTime.AddHours(8) && m.DateTime < FilterArchivesEndDateTime.AddHours(24).AddHours(8)).Sum(m => m.SetCement2);
        /// <summary> Сумма факт цемент2 </summary>
        public float ArchSumActCement2 => _Mixes.GetAll()
            .Where(m => m.DateTime >= FilterArchivesBeginDateTime.AddHours(8) && m.DateTime < FilterArchivesEndDateTime.AddHours(24).AddHours(8)).Sum(m => m.ActCement2);
        /// <summary> Сумма заданий алюминий1 </summary>
        public float ArchSumSetAluminium1 => _Mixes.GetAll()
            .Where(m => m.DateTime >= FilterArchivesBeginDateTime.AddHours(8) && m.DateTime < FilterArchivesEndDateTime.AddHours(24).AddHours(8)).Sum(m => m.SetAluminium1);
        /// <summary> Сумма факт алюминий1 </summary>
        public float ArchSumActAluminium1 => _Mixes.GetAll()
            .Where(m => m.DateTime >= FilterArchivesBeginDateTime.AddHours(8) && m.DateTime < FilterArchivesEndDateTime.AddHours(24).AddHours(8)).Sum(m => m.ActAluminium1);
        /// <summary> Сумма заданий алюминий2 </summary>
        public float ArchSumSetAluminium2 => _Mixes.GetAll()
            .Where(m => m.DateTime >= FilterArchivesBeginDateTime.AddHours(8) && m.DateTime < FilterArchivesEndDateTime.AddHours(24).AddHours(8)).Sum(m => m.SetAluminium2);
        /// <summary> Сумма факт алюминий2 </summary>
        public float ArchSumActAluminium2 => _Mixes.GetAll()
            .Where(m => m.DateTime >= FilterArchivesBeginDateTime.AddHours(8) && m.DateTime < FilterArchivesEndDateTime.AddHours(24).AddHours(8)).Sum(m => m.ActAluminium2);
        /// <summary> Количество плохих заливок </summary>
        public int ArchCountBadMixes => _Mixes.GetAll()
            .Count(m => m.DateTime >= FilterArchivesBeginDateTime.AddHours(8) &&
                        m.DateTime < FilterArchivesEndDateTime.AddHours(24).AddHours(8) && m.Normal == false);
        /// <summary> Количество недоростков </summary>
        public int ArchCountUndersizedMixes => _Mixes.GetAll()
            .Count(m => m.DateTime >= FilterArchivesBeginDateTime.AddHours(8) &&
                        m.DateTime < FilterArchivesEndDateTime.AddHours(24).AddHours(8) && m.Undersized);
        /// <summary> Количество переростков </summary>
        public int ArchCountOvergroundMixes => _Mixes.GetAll()
            .Count(m => m.DateTime >= FilterArchivesBeginDateTime.AddHours(8) &&
                        m.DateTime < FilterArchivesEndDateTime.AddHours(24).AddHours(8) && m.Overground);
        /// <summary> Количество закипевших массивов </summary>
        public int ArchCountBoiledMixes => _Mixes.GetAll()
            .Count(m => m.DateTime >= FilterArchivesBeginDateTime.AddHours(8) &&
                        m.DateTime < FilterArchivesEndDateTime.AddHours(24).AddHours(8) && m.Boiled);
        /// <summary> Количество скинутых в шлам массивов </summary>
        public int ArchCountMudMixes => _Mixes.GetAll()
            .Count(m => m.DateTime >= FilterArchivesBeginDateTime.AddHours(8) &&
                        m.DateTime < FilterArchivesEndDateTime.AddHours(24).AddHours(8) && m.IsMud);

        #endregion

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
            OnPropertyChanged(nameof(TimeSpanCurrentShiftMixes));
            OnPropertyChanged(nameof(CountCurrentShiftMixes));
            OnPropertyChanged(nameof(CountCurrentNormalShiftMixes));
            OnPropertyChanged(nameof(TimeBeginPreShiftMixes));
            OnPropertyChanged(nameof(CountPreShiftMixes));
            OnPropertyChanged(nameof(CountPreNormalShiftMixes));
        }

        #endregion

        #region Данные по заливкам текущей смены

        private ICommand _UpdateCurrentShiftMixesCommand;

        /// <summary> Обновить данные текущей смены </summary>
        public ICommand UpdateCurrentShiftMixesCommand => _UpdateCurrentShiftMixesCommand ??=
            new LambdaCommand(OnUpdateCurrentShiftMixesCommandExecuted, CanUpdateCurrentShiftMixesCommandExecute);

        private bool CanUpdateCurrentShiftMixesCommandExecute(object p) => true;

        private void OnUpdateCurrentShiftMixesCommandExecuted(object p)
        {
            LoadData();
            OnPropertyChanged(nameof(TimeSpanCurrentShiftMixes));
        }

        private ICommand _UpdatePreShiftMixesCommand;

        /// <summary> Обновить данные предидущей смены </summary>
        public ICommand UpdatePreShiftMixesCommand => _UpdatePreShiftMixesCommand ??=
            new LambdaCommand(OnUpdatePreShiftMixesCommandExecuted, CanUpdatePreShiftMixesCommandExecute);

        private bool CanUpdatePreShiftMixesCommandExecute(object p) => true;

        private void OnUpdatePreShiftMixesCommandExecuted(object p)
        {
            LoadData();
            OnPropertyChanged(nameof(TimeBeginPreShiftMixes));
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

        #region Статистические данные по сменам выбранной даты

        private ICommand _UpdateCurrentSumDatasCommand;

        /// <summary> Обновление данных дневной смены выбранной даты </summary>
        public ICommand UpdateCurrentSumDatasCommand => _UpdateCurrentSumDatasCommand ??=
            new LambdaCommand(OnUpdateCurrentSumDatasCommandExecuted, CanUpdateCurrentSumDatasCommandExecute);

        private bool CanUpdateCurrentSumDatasCommandExecute(object p) => true;

        private void OnUpdateCurrentSumDatasCommandExecuted(object p)
        {
            OnPropertyChanged(nameof(CurrAverageTemperature));
            OnPropertyChanged(nameof(CurrSumSetSandMud));
            OnPropertyChanged(nameof(CurrSumActSandMud));
            OnPropertyChanged(nameof(CurrSumSetRevertMud));
            OnPropertyChanged(nameof(CurrSumActRevertMud));
            OnPropertyChanged(nameof(CurrSumSetColdWater));
            OnPropertyChanged(nameof(CurrSumActColdWater));
            OnPropertyChanged(nameof(CurrSumSetHotWater));
            OnPropertyChanged(nameof(CurrSumActHotWater));
            OnPropertyChanged(nameof(CurrSumSetMixture1));
            OnPropertyChanged(nameof(CurrSumActMixture1));
            OnPropertyChanged(nameof(CurrSumSetMixture2));
            OnPropertyChanged(nameof(CurrSumActMixture2));
            OnPropertyChanged(nameof(CurrSumSetCement1));
            OnPropertyChanged(nameof(CurrSumActCement1));
            OnPropertyChanged(nameof(CurrSumSetCement2));
            OnPropertyChanged(nameof(CurrSumActCement2));
            OnPropertyChanged(nameof(CurrSumSetAluminium1));
            OnPropertyChanged(nameof(CurrSumActAluminium1));
            OnPropertyChanged(nameof(CurrSumSetAluminium2));
            OnPropertyChanged(nameof(CurrSumActAluminium2));
            OnPropertyChanged(nameof(CurrCountBadMixes));
            OnPropertyChanged(nameof(CurrCountUndersizedMixes));
            OnPropertyChanged(nameof(CurrCountOvergroundMixes));
            OnPropertyChanged(nameof(CurrCountBoiledMixes));
            OnPropertyChanged(nameof(CurrCountMudMixes));
        }

        private ICommand _UpdatePreSumDatasCommand;

        /// <summary> Обновление данных ночной смены выбранной даты </summary>
        public ICommand UpdatePreSumDatasCommand => _UpdatePreSumDatasCommand ??=
            new LambdaCommand(OnUpdatePreSumDatasCommandExecuted, CanUpdatePreSumDatasCommandExecute);

        private bool CanUpdatePreSumDatasCommandExecute(object p) => true;

        private void OnUpdatePreSumDatasCommandExecuted(object p)
        {
            OnPropertyChanged(nameof(PreAverageTemperature));
            OnPropertyChanged(nameof(PreSumSetSandMud));
            OnPropertyChanged(nameof(PreSumActSandMud));
            OnPropertyChanged(nameof(PreSumSetRevertMud));
            OnPropertyChanged(nameof(PreSumActRevertMud));
            OnPropertyChanged(nameof(PreSumSetColdWater));
            OnPropertyChanged(nameof(PreSumActColdWater));
            OnPropertyChanged(nameof(PreSumSetHotWater));
            OnPropertyChanged(nameof(PreSumActHotWater));
            OnPropertyChanged(nameof(PreSumSetMixture1));
            OnPropertyChanged(nameof(PreSumActMixture1));
            OnPropertyChanged(nameof(PreSumSetMixture2));
            OnPropertyChanged(nameof(PreSumActMixture2));
            OnPropertyChanged(nameof(PreSumSetCement1));
            OnPropertyChanged(nameof(PreSumActCement1));
            OnPropertyChanged(nameof(PreSumSetCement2));
            OnPropertyChanged(nameof(PreSumActCement2));
            OnPropertyChanged(nameof(PreSumSetAluminium1));
            OnPropertyChanged(nameof(PreSumActAluminium1));
            OnPropertyChanged(nameof(PreSumSetAluminium2));
            OnPropertyChanged(nameof(PreSumActAluminium2));
            OnPropertyChanged(nameof(PreCountBadMixes));
            OnPropertyChanged(nameof(PreCountUndersizedMixes));
            OnPropertyChanged(nameof(PreCountOvergroundMixes));
            OnPropertyChanged(nameof(PreCountBoiledMixes));
            OnPropertyChanged(nameof(PreCountMudMixes));
        }

        #endregion

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

        #region Статистические данные по архивным отфильтрованным данным

        private ICommand _UpdateArchivesSumDatasCommand;

        /// <summary> Обновление статистических данных отфильтрованных заливок </summary>
        public ICommand UpdateArchivesSumDatasCommand => _UpdateArchivesSumDatasCommand ??=
            new LambdaCommand(OnUpdateArchivesSumDatasCommandExecuted, CanUpdateArchivesSumDatasCommandExecute);

        private bool CanUpdateArchivesSumDatasCommandExecute(object p) => true;

        private void OnUpdateArchivesSumDatasCommandExecuted(object p)
        {
            OnPropertyChanged(nameof(ArchAverageTemperature));
            OnPropertyChanged(nameof(ArchSumSetSandMud));
            OnPropertyChanged(nameof(ArchSumActSandMud));
            OnPropertyChanged(nameof(ArchSumSetRevertMud));
            OnPropertyChanged(nameof(ArchSumActRevertMud));
            OnPropertyChanged(nameof(ArchSumSetColdWater));
            OnPropertyChanged(nameof(ArchSumActColdWater));
            OnPropertyChanged(nameof(ArchSumSetHotWater));
            OnPropertyChanged(nameof(ArchSumActHotWater));
            OnPropertyChanged(nameof(ArchSumSetMixture1));
            OnPropertyChanged(nameof(ArchSumActMixture1));
            OnPropertyChanged(nameof(ArchSumSetMixture2));
            OnPropertyChanged(nameof(ArchSumActMixture2));
            OnPropertyChanged(nameof(ArchSumSetCement1));
            OnPropertyChanged(nameof(ArchSumActCement1));
            OnPropertyChanged(nameof(ArchSumSetCement2));
            OnPropertyChanged(nameof(ArchSumActCement2));
            OnPropertyChanged(nameof(ArchSumSetAluminium1));
            OnPropertyChanged(nameof(ArchSumActAluminium1));
            OnPropertyChanged(nameof(ArchSumSetAluminium2));
            OnPropertyChanged(nameof(ArchSumActAluminium2));
            OnPropertyChanged(nameof(ArchCountBadMixes));
            OnPropertyChanged(nameof(ArchCountUndersizedMixes));
            OnPropertyChanged(nameof(ArchCountOvergroundMixes));
            OnPropertyChanged(nameof(ArchCountBoiledMixes));
            OnPropertyChanged(nameof(ArchCountMudMixes));
        }

        #endregion

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

        private ICommand _TestCommand;

        /// <summary> Test </summary>
        public ICommand TestCommand => _TestCommand ??=
            new LambdaCommand(OnTestCommandExecuted, CanTestCommandExecute);

        private bool CanTestCommandExecute(object p) => true;

        private void OnTestCommandExecuted(object p)
        {
            MixRaport raport = new MixRaport();
            
            raport.CreatePackage("test.xlsx");

            MessageBox.Show("Файл создан!");
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
                CurrentShiftMixes.Clear();
                foreach (var mix in _Mixes.GetAll()
                    .Where(m => m.DateTime >= date)
                    .OrderByDescending(m => m.DateTime))
                {
                    CurrentShiftMixes.Add(mix);
                }
                CountCurrentShiftMixes = CurrentShiftMixes.Count;
                CountCurrentNormalShiftMixes = CurrentShiftMixes.Count(m => m.Normal);

                PreShiftMixes.Clear();
                foreach (var mix in _Mixes.GetAll()
                    .Where(m => m.DateTime >= date.AddHours(-12) && m.DateTime < date)
                    .OrderByDescending(m => m.DateTime))
                {
                    PreShiftMixes.Add(mix);
                }

                CountPreShiftMixes = PreShiftMixes.Count;
                CountPreNormalShiftMixes = PreShiftMixes.Count(m => m.Normal);

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
