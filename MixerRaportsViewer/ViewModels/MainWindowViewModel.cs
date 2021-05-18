using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Win32;
using MixerRaportsViewer.Commands;
using MixerRaportsViewer.Raports;
using MixerReports.lib.Interfaces;
using MixerReports.lib.Models;
using MixerReports.Raports;

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
                CountShiftDayMixes = mixs.Count;
                CountNormalShiftDayMixes = mixs.Count(m => m.Normal);

                if (mixs.Count > 0)
                    DayAverageTemperature = mixs.Average(m => m.MixerTemperature);
                else
                    DayAverageTemperature = 0.0f;
                DaySumSetSandMud = mixs.Sum(m => m.SetSandMud);
                DaySumActSandMud = mixs.Sum(m => m.ActSandMud);
                DaySumSetRevertMud = mixs.Sum(m => m.SetRevertMud);
                DaySumActRevertMud = mixs.Sum(m => m.ActRevertMud);
                DaySumSetColdWater = mixs.Sum(m => m.SetColdWater);
                DaySumActColdWater = mixs.Sum(m => m.ActColdWater);
                DaySumSetHotWater = mixs.Sum(m => m.SetHotWater);
                DaySumActHotWater = mixs.Sum(m => m.ActHotWater);
                DaySumSetMixture1 = mixs.Sum(m => m.SetMixture1);
                DaySumActMixture1 = mixs.Sum(m => m.ActMixture1);
                DaySumSetMixture2 = mixs.Sum(m => m.SetMixture2);
                DaySumActMixture2 = mixs.Sum(m => m.ActMixture2);
                DaySumSetCement1 = mixs.Sum(m => m.SetCement1);
                DaySumActCement1 = mixs.Sum(m => m.ActCement1);
                DaySumSetCement2 = mixs.Sum(m => m.SetCement2);
                DaySumActCement2 = mixs.Sum(m => m.ActCement2);
                DaySumSetAluminium1 = mixs.Sum(m => m.SetAluminium1);
                DaySumActAluminium1 = mixs.Sum(m => m.ActAluminium1);
                DaySumSetAluminium2 = mixs.Sum(m => m.SetAluminium2);
                DaySumActAluminium2 = mixs.Sum(m => m.ActAluminium2);
                DayCountBadMixes = mixs.Count(m => m.Normal == false);
                DayCountUndersizedMixes = mixs.Count(m => m.Undersized);
                DayCountOvergroundMixes = mixs.Count(m => m.Overground);
                DayCountBoiledMixes = mixs.Count(m => m.Boiled);
                DayCountMudMixes = mixs.Count(m => m.IsMud);

                if (SetOnlyBadShiftsMixes)
                    mixs = mixs.Where(m => m.Normal == false).ToList();
                return mixs;
            }
        }
        private int _CountShiftDayMixes;
        /// <summary> Количество заливок дневной смены </summary>
        public int CountShiftDayMixes
        {
            get => _CountShiftDayMixes;
            set => Set(ref _CountShiftDayMixes, value);
        }
        private int _CountNormalShiftDayMixes;
        /// <summary> Количество нормальных заливок дневной смены </summary>
        public int CountNormalShiftDayMixes
        {
            get => _CountNormalShiftDayMixes;
            set => Set(ref _CountNormalShiftDayMixes, value);
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
                CountShiftNightMixes = mixs.Count;
                CountNormalShiftNightMixes = mixs.Count(m => m.Normal);

                if (mixs.Count > 0)
                    NightAverageTemperature = mixs.Average(m => m.MixerTemperature);
                else
                    NightAverageTemperature = 0.0f;
                NightSumSetSandMud = mixs.Sum(m => m.SetSandMud);
                NightSumActSandMud = mixs.Sum(m => m.ActSandMud);
                NightSumSetRevertMud = mixs.Sum(m => m.SetRevertMud);
                NightSumActRevertMud = mixs.Sum(m => m.ActRevertMud);
                NightSumSetColdWater = mixs.Sum(m => m.SetColdWater);
                NightSumActColdWater = mixs.Sum(m => m.ActColdWater);
                NightSumSetHotWater = mixs.Sum(m => m.SetHotWater);
                NightSumActHotWater = mixs.Sum(m => m.ActHotWater);
                NightSumSetMixture1 = mixs.Sum(m => m.SetMixture1);
                NightSumActMixture1 = mixs.Sum(m => m.ActMixture1);
                NightSumSetMixture2 = mixs.Sum(m => m.SetMixture2);
                NightSumActMixture2 = mixs.Sum(m => m.ActMixture2);
                NightSumSetCement1 = mixs.Sum(m => m.SetCement1);
                NightSumActCement1 = mixs.Sum(m => m.ActCement1);
                NightSumSetCement2 = mixs.Sum(m => m.SetCement2);
                NightSumActCement2 = mixs.Sum(m => m.ActCement2);
                NightSumSetAluminium1 = mixs.Sum(m => m.SetAluminium1);
                NightSumActAluminium1 = mixs.Sum(m => m.ActAluminium1);
                NightSumSetAluminium2 = mixs.Sum(m => m.SetAluminium2);
                NightSumActAluminium2 = mixs.Sum(m => m.ActAluminium2);
                NightCountBadMixes = mixs.Count(m => m.Normal == false);
                NightCountUndersizedMixes = mixs.Count(m => m.Undersized);
                NightCountOvergroundMixes = mixs.Count(m => m.Overground);
                NightCountBoiledMixes = mixs.Count(m => m.Boiled);
                NightCountMudMixes = mixs.Count(m => m.IsMud);

                if (SetOnlyBadShiftsMixes)
                    mixs = mixs.Where(m => m.Normal == false).ToList();
                return mixs;
            }
        }
        private int _CountShiftNightMixes;
        /// <summary> Количество заливок ночной смены </summary>
        public int CountShiftNightMixes
        {
            get => _CountShiftNightMixes;
            set => Set(ref _CountShiftNightMixes, value);
        }

        private int _CountNormalShiftNightMixes;

        /// <summary> Количество нормальных заливок ночной смены </summary>
        public int CountNormalShiftNightMixes
        {
            get => _CountNormalShiftNightMixes;
            set => Set(ref _CountNormalShiftNightMixes, value);
        }

        #region Статистические данные по дневной смене

        private float _DayAverageTemperature;
        /// <summary> Средняя температура </summary>
        public float DayAverageTemperature
        {
            get => _DayAverageTemperature;
            set => Set(ref _DayAverageTemperature, value);
        }
        private float _DaySumSetSandMud;
        /// <summary> Сумма заданий песчаный шлам </summary>
        public float DaySumSetSandMud
        {
            get => _DaySumSetSandMud;
            set => Set(ref _DaySumSetSandMud, value);
        }
        private float _DaySumActSandMud;
        /// <summary> Сумма факт песчаный шлам </summary>
        public float DaySumActSandMud
        {
            get => _DaySumActSandMud;
            set => Set(ref _DaySumActSandMud, value);
        }
        private float _DaySumSetRevertMud;
        /// <summary> Сумма заданий обратный шлам </summary>
        public float DaySumSetRevertMud
        {
            get => _DaySumSetRevertMud;
            set => Set(ref _DaySumSetRevertMud, value);
        }
        private float _DaySumActRevertMud;
        /// <summary> Сумма факт обратный шлам </summary>
        public float DaySumActRevertMud
        {
            get => _DaySumActRevertMud;
            set => Set(ref _DaySumActRevertMud, value);
        }
        private float _DaySumSetColdWater;
        /// <summary> Сумма заданий холодная вода </summary>
        public float DaySumSetColdWater
        {
            get => _DaySumSetColdWater;
            set => Set(ref _DaySumSetColdWater, value);
        }
        private float _DaySumActColdWater;
        /// <summary> Сумма факт холодная вода </summary>
        public float DaySumActColdWater
        {
            get => _DaySumActColdWater;
            set => Set(ref _DaySumActColdWater, value);
        }
        private float _DaySumSetHotWater;
        /// <summary> Сумма заданий холодная вода </summary>
        public float DaySumSetHotWater
        {
            get => _DaySumSetHotWater;
            set => Set(ref _DaySumSetHotWater, value);
        }
        private float _DaySumActHotWater;
        /// <summary> Сумма факт холодная вода </summary>
        public float DaySumActHotWater
        {
            get => _DaySumActHotWater;
            set => Set(ref _DaySumActHotWater, value);
        }
        private float _DaySumSetMixture1;
        /// <summary> Сумма заданий ИПВ1 </summary>
        public float DaySumSetMixture1
        {
            get => _DaySumSetMixture1;
            set => Set(ref _DaySumSetMixture1, value);
        }
        private float _DaySumActMixture1;
        /// <summary> Сумма факт ИПВ1 </summary>
        public float DaySumActMixture1
        {
            get => _DaySumActMixture1;
            set => Set(ref _DaySumActMixture1, value);
        }
        private float _DaySumSetMixture2;
        /// <summary> Сумма заданий ИПВ1 </summary>
        public float DaySumSetMixture2
        {
            get => _DaySumSetMixture2;
            set => Set(ref _DaySumSetMixture2, value);
        }
        private float _DaySumActMixture2;
        /// <summary> Сумма факт ИПВ1 </summary>
        public float DaySumActMixture2
        {
            get => _DaySumActMixture2;
            set => Set(ref _DaySumActMixture2, value);
        }
        private float _DaySumSetCement1;
        /// <summary> Сумма заданий цемент1 </summary>
        public float DaySumSetCement1
        {
            get => _DaySumSetCement1;
            set => Set(ref _DaySumSetCement1, value);
        }
        private float _DaySumActCement1;
        /// <summary> Сумма факт цемент1 </summary>
        public float DaySumActCement1
        {
            get => _DaySumActCement1;
            set => Set(ref _DaySumActCement1, value);
        }
        private float _DaySumSetCement2;
        /// <summary> Сумма заданий цемент2 </summary>
        public float DaySumSetCement2
        {
            get => _DaySumSetCement2;
            set => Set(ref _DaySumSetCement2, value);
        }
        private float _DaySumActCement2;
        /// <summary> Сумма факт цемент2 </summary>
        public float DaySumActCement2
        {
            get => _DaySumActCement2;
            set => Set(ref _DaySumActCement2, value);
        }
        private float _DaySumSetAluminium1;
        /// <summary> Сумма заданий алюминий1 </summary>
        public float DaySumSetAluminium1
        {
            get => _DaySumSetAluminium1;
            set => Set(ref _DaySumSetAluminium1, value);
        }
        private float _DaySumActAluminium1;
        /// <summary> Сумма факт алюминий1 </summary>
        public float DaySumActAluminium1
        {
            get => _DaySumActAluminium1;
            set => Set(ref _DaySumActAluminium1, value);
        }
        private float _DaySumSetAluminium2;
        /// <summary> Сумма заданий алюминий2 </summary>
        public float DaySumSetAluminium2
        {
            get => _DaySumSetAluminium2;
            set => Set(ref _DaySumSetAluminium2, value);
        }
        private float _DaySumActAluminium2;
        /// <summary> Сумма факт алюминий2 </summary>
        public float DaySumActAluminium2
        {
            get => _DaySumActAluminium2;
            set => Set(ref _DaySumActAluminium2, value);
        }
        private int _DayCountBadMixes;
        /// <summary> Количество плохих заливок </summary>
        public int DayCountBadMixes
        {
            get => _DayCountBadMixes;
            set => Set(ref _DayCountBadMixes, value);
        }
        private int _DayCountUndersizedMixes;
        /// <summary> Количество недоростков </summary>
        public int DayCountUndersizedMixes
        {
            get => _DayCountUndersizedMixes;
            set => Set(ref _DayCountUndersizedMixes, value);
        }
        private int _DayCountOvergroundMixes;
        /// <summary> Количество переростков </summary>
        public int DayCountOvergroundMixes
        {
            get => _DayCountOvergroundMixes;
            set => Set(ref _DayCountOvergroundMixes, value);
        }
        private int _DayCountBoiledMixes;
        /// <summary> Количество закипевших массивов </summary>
        public int DayCountBoiledMixes
        {
            get => _DayCountBoiledMixes;
            set => Set(ref _DayCountBoiledMixes, value);
        }
        private int _DayCountMudMixes;
        /// <summary> Количество скинутых в шлам массивов </summary>
        public int DayCountMudMixes
        {
            get => _DayCountMudMixes;
            set => Set(ref _DayCountMudMixes, value);
        }

        #endregion

        #region Статистические данные по ночной смене

        private float _NightAverageTemperature;
        /// <summary> Средняя температура </summary>
        public float NightAverageTemperature
        {
            get => _NightAverageTemperature;
            set => Set(ref _NightAverageTemperature, value);
        }
        private float _NightSumSetSandMud;
        /// <summary> Сумма заданий песчаный шлам </summary>
        public float NightSumSetSandMud
        {
            get => _NightSumSetSandMud;
            set => Set(ref _NightSumSetSandMud, value);
        }
        private float _NightSumActSandMud;
        /// <summary> Сумма факт песчаный шлам </summary>
        public float NightSumActSandMud
        {
            get => _NightSumActSandMud;
            set => Set(ref _NightSumActSandMud, value);
        }
        private float _NightSumSetRevertMud;
        /// <summary> Сумма заданий обратный шлам </summary>
        public float NightSumSetRevertMud
        {
            get => _NightSumSetRevertMud;
            set => Set(ref _NightSumSetRevertMud, value);
        }
        private float _NightSumActRevertMud;
        /// <summary> Сумма факт обратный шлам </summary>
        public float NightSumActRevertMud
        {
            get => _NightSumActRevertMud;
            set => Set(ref _NightSumActRevertMud, value);
        }
        private float _NightSumSetColdWater;
        /// <summary> Сумма заданий холодная вода </summary>
        public float NightSumSetColdWater
        {
            get => _NightSumSetColdWater;
            set => Set(ref _NightSumSetColdWater, value);
        }
        private float _NightSumActColdWater;
        /// <summary> Сумма факт холодная вода </summary>
        public float NightSumActColdWater
        {
            get => _NightSumActColdWater;
            set => Set(ref _NightSumActColdWater, value);
        }
        private float _NightSumSetHotWater;
        /// <summary> Сумма заданий холодная вода </summary>
        public float NightSumSetHotWater
        {
            get => _NightSumSetHotWater;
            set => Set(ref _NightSumSetHotWater, value);
        }
        private float _NightSumActHotWater;
        /// <summary> Сумма факт холодная вода </summary>
        public float NightSumActHotWater
        {
            get => _NightSumActHotWater;
            set => Set(ref _NightSumActHotWater, value);
        }
        private float _NightSumSetMixture1;
        /// <summary> Сумма заданий ИПВ1 </summary>
        public float NightSumSetMixture1
        {
            get => _NightSumSetMixture1;
            set => Set(ref _NightSumSetMixture1, value);
        }
        private float _NightSumActMixture1;
        /// <summary> Сумма факт ИПВ1 </summary>
        public float NightSumActMixture1
        {
            get => _NightSumActMixture1;
            set => Set(ref _NightSumActMixture1, value);
        }
        private float _NightSumSetMixture2;
        /// <summary> Сумма заданий ИПВ1 </summary>
        public float NightSumSetMixture2
        {
            get => _NightSumSetMixture2;
            set => Set(ref _NightSumSetMixture2, value);
        }
        private float _NightSumActMixture2;
        /// <summary> Сумма факт ИПВ1 </summary>
        public float NightSumActMixture2
        {
            get => _NightSumActMixture2;
            set => Set(ref _NightSumActMixture2, value);
        }
        private float _NightSumSetCement1;
        /// <summary> Сумма заданий цемент1 </summary>
        public float NightSumSetCement1
        {
            get => _NightSumSetCement1;
            set => Set(ref _NightSumSetCement1, value);
        }
        private float _NightSumActCement1;
        /// <summary> Сумма факт цемент1 </summary>
        public float NightSumActCement1
        {
            get => _NightSumActCement1;
            set => Set(ref _NightSumActCement1, value);
        }
        private float _NightSumSetCement2;
        /// <summary> Сумма заданий цемент2 </summary>
        public float NightSumSetCement2
        {
            get => _NightSumSetCement2;
            set => Set(ref _NightSumSetCement2, value);
        }
        private float _NightSumActCement2;
        /// <summary> Сумма факт цемент2 </summary>
        public float NightSumActCement2
        {
            get => _NightSumActCement2;
            set => Set(ref _NightSumActCement2, value);
        }
        private float _NightSumSetAluminium1;
        /// <summary> Сумма заданий алюминий1 </summary>
        public float NightSumSetAluminium1
        {
            get => _NightSumSetAluminium1;
            set => Set(ref _NightSumSetAluminium1, value);
        }
        private float _NightSumActAluminium1;
        /// <summary> Сумма факт алюминий1 </summary>
        public float NightSumActAluminium1
        {
            get => _NightSumActAluminium1;
            set => Set(ref _NightSumActAluminium1, value);
        }
        private float _NightSumSetAluminium2;
        /// <summary> Сумма заданий алюминий2 </summary>
        public float NightSumSetAluminium2
        {
            get => _NightSumSetAluminium2;
            set => Set(ref _NightSumSetAluminium2, value);
        }
        private float _NightSumActAluminium2;
        /// <summary> Сумма факт алюминий2 </summary>
        public float NightSumActAluminium2
        {
            get => _NightSumActAluminium2;
            set => Set(ref _NightSumActAluminium2, value);
        }
        private int _NightCountBadMixes;
        /// <summary> Количество плохих заливок </summary>
        public int NightCountBadMixes
        {
            get => _NightCountBadMixes;
            set => Set(ref _NightCountBadMixes, value);
        }
        private int _NightCountUndersizedMixes;
        /// <summary> Количество недоростков </summary>
        public int NightCountUndersizedMixes
        {
            get => _NightCountUndersizedMixes;
            set => Set(ref _NightCountUndersizedMixes, value);
        }
        private int _NightCountOvergroundMixes;
        /// <summary> Количество переростков </summary>
        public int NightCountOvergroundMixes
        {
            get => _NightCountOvergroundMixes;
            set => Set(ref _NightCountOvergroundMixes, value);
        }
        private int _NightCountBoiledMixes;
        /// <summary> Количество закипевших массивов </summary>
        public int NightCountBoiledMixes
        {
            get => _NightCountBoiledMixes;
            set => Set(ref _NightCountBoiledMixes, value);
        }
        private int _NightCountMudMixes;
        /// <summary> Количество скинутых в шлам массивов </summary>
        public int NightCountMudMixes
        {
            get => _NightCountMudMixes;
            set => Set(ref _NightCountMudMixes, value);
        }

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

        private bool _setOnlyBadArchivesMixes;
        /// <summary> Выбор - показывать только плохие заливки </summary>
        public bool SetOnlyBadArchivesMixes
        {
            get => _setOnlyBadArchivesMixes;
            set
            {
                Set(ref _setOnlyBadArchivesMixes, value);
                OnPropertyChanged(nameof(FilteredArchivesMixes));
            }
        }

        private bool _SetAroundBadArchives;
        /// <summary> Выбор - показывать смежные к плохим заливки </summary>
        public bool SetAroundBadArchives
        {
            get => _SetAroundBadArchives;
            set
            {
                Set(ref _SetAroundBadArchives, value);
                OnPropertyChanged(nameof(FilteredArchivesMixes));
            }
        }

        /// <summary> Отфильтрованные архивные данные </summary>
        public ICollection<Mix> FilteredArchivesMixes
        {
            get
            {
                var mixs = _Mixes.GetAll()
                    .Where(m => m.DateTime >= FilterArchivesBeginDateTime.AddHours(8) &&
                                m.DateTime < FilterArchivesEndDateTime.AddHours(24).AddHours(8))
                    .OrderBy(m => m.DateTime).ToList();
                CountArchivesMixes = mixs.Count;
                CountNormalArchivesMixes = mixs.Count(m => m.Normal);

                if (mixs.Count > 0)
                    ArchAverageTemperature = mixs.Average(m => m.MixerTemperature);
                else
                    ArchAverageTemperature = 0.0f;
                ArchSumSetSandMud = mixs.Sum(m => m.SetSandMud);
                ArchSumActSandMud = mixs.Sum(m => m.ActSandMud);
                ArchSumSetRevertMud = mixs.Sum(m => m.SetRevertMud);
                ArchSumActRevertMud = mixs.Sum(m => m.ActRevertMud);
                ArchSumSetColdWater = mixs.Sum(m => m.SetColdWater);
                ArchSumActColdWater = mixs.Sum(m => m.ActColdWater);
                ArchSumSetHotWater = mixs.Sum(m => m.SetHotWater);
                ArchSumActHotWater = mixs.Sum(m => m.ActHotWater);
                ArchSumSetMixture1 = mixs.Sum(m => m.SetMixture1);
                ArchSumActMixture1 = mixs.Sum(m => m.ActMixture1);
                ArchSumSetMixture2 = mixs.Sum(m => m.SetMixture2);
                ArchSumActMixture2 = mixs.Sum(m => m.ActMixture2);
                ArchSumSetCement1 = mixs.Sum(m => m.SetCement1);
                ArchSumActCement1 = mixs.Sum(m => m.ActCement1);
                ArchSumSetCement2 = mixs.Sum(m => m.SetCement2);
                ArchSumActCement2 = mixs.Sum(m => m.ActCement2);
                ArchSumSetAluminium1 = mixs.Sum(m => m.SetAluminium1);
                ArchSumActAluminium1 = mixs.Sum(m => m.ActAluminium1);
                ArchSumSetAluminium2 = mixs.Sum(m => m.SetAluminium2);
                ArchSumActAluminium2 = mixs.Sum(m => m.ActAluminium2);
                ArchCountBadMixes = mixs.Count(m => m.Normal == false);
                ArchCountUndersizedMixes = mixs.Count(m => m.Undersized);
                ArchCountOvergroundMixes = mixs.Count(m => m.Overground);
                ArchCountBoiledMixes = mixs.Count(m => m.Boiled);
                ArchCountMudMixes = mixs.Count(m => m.IsMud);

                if (SetOnlyBadArchivesMixes && !SetAroundBadArchives)
                    mixs = mixs.Where(m => m.Normal == false).ToList();
                if (SetAroundBadArchives)
                    mixs = mixs.Skip(1).SkipLast(1).Where((m, ind) =>
                        !m.Normal || !mixs.ElementAt(ind).Normal || !mixs.ElementAt(ind + 2).Normal).ToList();
                return mixs;
            }
        }
        private int _CountArchivesMixes;
        /// <summary> Количество заливок отфильтрованных архивных </summary>
        public int CountArchivesMixes
        {
            get => _CountArchivesMixes;
            set => Set(ref _CountArchivesMixes, value);
        }
        private int _CountNormalArchivesMixes;
        /// <summary> Количество нормальных заливок отфильтрованных архивных </summary>
        public int CountNormalArchivesMixes
        {
            get => _CountNormalArchivesMixes;
            set => Set(ref _CountNormalArchivesMixes, value);
        }

        #region Статистические данные по отфильтрованным архивным данным

        private float _ArchAverageTemperature;
        /// <summary> Средняя температура </summary>
        public float ArchAverageTemperature
        {
            get => _ArchAverageTemperature;
            set => Set(ref _ArchAverageTemperature, value);
        }
        private float _ArchSumSetSandMud;
        /// <summary> Сумма заданий песчаный шлам </summary>
        public float ArchSumSetSandMud
        {
            get => _ArchSumSetSandMud;
            set => Set(ref _ArchSumSetSandMud, value);
        }
        private float _ArchSumActSandMud;
        /// <summary> Сумма факт песчаный шлам </summary>
        public float ArchSumActSandMud
        {
            get => _ArchSumActSandMud;
            set => Set(ref _ArchSumActSandMud, value);
        }
        private float _ArchSumSetRevertMud;
        /// <summary> Сумма заданий обратный шлам </summary>
        public float ArchSumSetRevertMud
        {
            get => _ArchSumSetRevertMud;
            set => Set(ref _ArchSumSetRevertMud, value);
        }
        private float _ArchSumActRevertMud;
        /// <summary> Сумма факт обратный шлам </summary>
        public float ArchSumActRevertMud
        {
            get => _ArchSumActRevertMud;
            set => Set(ref _ArchSumActRevertMud, value);
        }
        private float _ArchSumSetColdWater;
        /// <summary> Сумма заданий холодная вода </summary>
        public float ArchSumSetColdWater
        {
            get => _ArchSumSetColdWater;
            set => Set(ref _ArchSumSetColdWater, value);
        }
        private float _ArchSumActColdWater;
        /// <summary> Сумма факт холодная вода </summary>
        public float ArchSumActColdWater
        {
            get => _ArchSumActColdWater;
            set => Set(ref _ArchSumActColdWater, value);
        }
        private float _ArchSumSetHotWater;
        /// <summary> Сумма заданий холодная вода </summary>
        public float ArchSumSetHotWater
        {
            get => _ArchSumSetHotWater;
            set => Set(ref _ArchSumSetHotWater, value);
        }
        private float _ArchSumActHotWater;
        /// <summary> Сумма факт холодная вода </summary>
        public float ArchSumActHotWater
        {
            get => _ArchSumActHotWater;
            set => Set(ref _ArchSumActHotWater, value);
        }
        private float _ArchSumSetMixture1;
        /// <summary> Сумма заданий ИПВ1 </summary>
        public float ArchSumSetMixture1
        {
            get => _ArchSumSetMixture1;
            set => Set(ref _ArchSumSetMixture1, value);
        }
        private float _ArchSumActMixture1;
        /// <summary> Сумма факт ИПВ1 </summary>
        public float ArchSumActMixture1
        {
            get => _ArchSumActMixture1;
            set => Set(ref _ArchSumActMixture1, value);
        }
        private float _ArchSumSetMixture2;
        /// <summary> Сумма заданий ИПВ1 </summary>
        public float ArchSumSetMixture2
        {
            get => _ArchSumSetMixture2;
            set => Set(ref _ArchSumSetMixture2, value);
        }
        private float _ArchSumActMixture2;
        /// <summary> Сумма факт ИПВ1 </summary>
        public float ArchSumActMixture2
        {
            get => _ArchSumActMixture2;
            set => Set(ref _ArchSumActMixture2, value);
        }
        private float _ArchSumSetCement1;
        /// <summary> Сумма заданий цемент1 </summary>
        public float ArchSumSetCement1
        {
            get => _ArchSumSetCement1;
            set => Set(ref _ArchSumSetCement1, value);
        }
        private float _ArchSumActCement1;
        /// <summary> Сумма факт цемент1 </summary>
        public float ArchSumActCement1
        {
            get => _ArchSumActCement1;
            set => Set(ref _ArchSumActCement1, value);
        }
        private float _ArchSumSetCement2;
        /// <summary> Сумма заданий цемент2 </summary>
        public float ArchSumSetCement2
        {
            get => _ArchSumSetCement2;
            set => Set(ref _ArchSumSetCement2, value);
        }
        private float _ArchSumActCement2;
        /// <summary> Сумма факт цемент2 </summary>
        public float ArchSumActCement2
        {
            get => _ArchSumActCement2;
            set => Set(ref _ArchSumActCement2, value);
        }
        private float _ArchSumSetAluminium1;
        /// <summary> Сумма заданий алюминий1 </summary>
        public float ArchSumSetAluminium1
        {
            get => _ArchSumSetAluminium1;
            set => Set(ref _ArchSumSetAluminium1, value);
        }
        private float _ArchSumActAluminium1;
        /// <summary> Сумма факт алюминий1 </summary>
        public float ArchSumActAluminium1
        {
            get => _ArchSumActAluminium1;
            set => Set(ref _ArchSumActAluminium1, value);
        }
        private float _ArchSumSetAluminium2;
        /// <summary> Сумма заданий алюминий2 </summary>
        public float ArchSumSetAluminium2
        {
            get => _ArchSumSetAluminium2;
            set => Set(ref _ArchSumSetAluminium2, value);
        }
        private float _ArchSumActAluminium2;
        /// <summary> Сумма факт алюминий2 </summary>
        public float ArchSumActAluminium2
        {
            get => _ArchSumActAluminium2;
            set => Set(ref _ArchSumActAluminium2, value);
        }
        private int _ArchCountBadMixes;
        /// <summary> Количество плохих заливок </summary>
        public int ArchCountBadMixes
        {
            get => _ArchCountBadMixes;
            set => Set(ref _ArchCountBadMixes, value);
        }
        private int _ArchCountUndersizedMixes;
        /// <summary> Количество недоростков </summary>
        public int ArchCountUndersizedMixes
        {
            get => _ArchCountUndersizedMixes;
            set => Set(ref _ArchCountUndersizedMixes, value);
        }
        private int _ArchCountOvergroundMixes;
        /// <summary> Количество переростков </summary>
        public int ArchCountOvergroundMixes
        {
            get => _ArchCountOvergroundMixes;
            set => Set(ref _ArchCountOvergroundMixes, value);
        }
        private int _ArchCountBoiledMixes;
        /// <summary> Количество закипевших массивов </summary>
        public int ArchCountBoiledMixes
        {
            get => _ArchCountBoiledMixes;
            set => Set(ref _ArchCountBoiledMixes, value);
        }
        private int _ArchCountMudMixes;
        /// <summary> Количество скинутых в шлам массивов </summary>
        public int ArchCountMudMixes
        {
            get => _ArchCountMudMixes;
            set => Set(ref _ArchCountMudMixes, value);
        }

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
        }

        private ICommand _GenerateRaportMixesCommand;

        /// <summary> Генерация отчета по заливкам </summary>
        public ICommand GenerateRaportMixesCommand => _GenerateRaportMixesCommand ??=
            new LambdaCommand(OnGenerateRaportMixesCommandExecuted, CanGenerateRaportMixesCommandExecute);

        private bool CanGenerateRaportMixesCommandExecute(object p) => true;

        private void OnGenerateRaportMixesCommandExecuted(object p)
        {
            var dialog = new SaveFileDialog
            {
                Title = "Сохранение отчета по заливкам выбранного дня в формате Excel",
                Filter = "Файлы Excel (*.xlsx)|*.xlsx|Все файлы (*.*)|*.*",
                FileName = $"Отчет БСУ {ShiftSelectDateTime:dd.MM.yyyy}",
                OverwritePrompt = true,
                InitialDirectory = Environment.CurrentDirectory,
            };
            if (dialog.ShowDialog() == false)
                return;
            MixRaportRed raport = new MixRaportRed();
            raport.DayMixes = ShiftDayMixes;
            raport.NightMixes = ShiftNightMixes;

            var fileName = dialog.FileName;
            try
            {
                raport.CreatePackage(fileName);
            }
            catch (IOException e)
            {
                MessageBox.Show($"Ошибка ввода-вывода при сохранении данных в файл {fileName}, ошибка: \n{e.Message}", "Ошибка сохранения файла", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            catch (Exception e)
            {
                MessageBox.Show($"Не удалось сохранить данные в файл {fileName}, ошибка: \n{e.Message}", "Ошибка сохранения файла", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MessageBox.Show($"Отчет по заливкам успешно сохранен в этом файле: \n{fileName}\n");
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

        private ICommand _GenerateFilteredArchiveMixesCommand;

        /// <summary> Генерация отчета по отфильтрованным архивным данным </summary>
        public ICommand GenerateFilteredArchiveMixesCommand => _GenerateFilteredArchiveMixesCommand ??=
            new LambdaCommand(OnGenerateFilteredArchiveMixesCommandExecuted, CanGenerateFilteredArchiveMixesCommandExecute);

        private bool CanGenerateFilteredArchiveMixesCommandExecute(object p) => true;

        private void OnGenerateFilteredArchiveMixesCommandExecuted(object p)
        {
            var dialog = new SaveFileDialog
            {
                Title = "Сохранение отчета по архивным данным заливок в формате Excel",
                Filter = "Файлы Excel (*.xlsx)|*.xlsx|Все файлы (*.*)|*.*",
                FileName = $"Отчет БСУ с {FilterArchivesBeginDateTime:dd.MM.yyyy} по {FilterArchivesEndDateTime:dd.MM.yyyy}",
                OverwritePrompt = true,
                InitialDirectory = Environment.CurrentDirectory,
            };
            if (dialog.ShowDialog() == false)
                return;
            MixRaportArchives raport = new MixRaportArchives();
            raport.Mixes = FilteredArchivesMixes;

            var fileName = dialog.FileName;
            try
            {
                raport.CreatePackage(fileName);
            }
            catch (IOException e)
            {
                MessageBox.Show($"Ошибка ввода-вывода при сохранении данных в файл {fileName}, ошибка: \n{e.Message}", "Ошибка сохранения файла", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            catch (Exception e)
            {
                MessageBox.Show($"Не удалось сохранить данные в файл {fileName}, ошибка: \n{e.Message}", "Ошибка сохранения файла", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MessageBox.Show($"Отчет по заливкам архивным успешно сохранен в этом файле: \n{fileName}\n");
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
