using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using MixerReports.lib.Models.Base;

namespace MixerReports.lib.Models
{
    public class Mix : Entity
    {
        private int _Number;

        /// <summary> Номер </summary>
        public int Number
        {
            get => _Number;
            set => Set(ref _Number, value);
        }

        private DateTime _DateTime;

        /// <summary> Датавремя </summary>
        public DateTime DateTime
        {
            get => _DateTime;
            set => Set(ref _DateTime, value);
        }

        private int _FormNumber;

        /// <summary> Номер формы </summary>
        public int FormNumber
        {
            get => _FormNumber;
            set => Set(ref _FormNumber, value);
        }

        private int _RecipeNumber;

        /// <summary> Номер рецепта </summary>
        public int RecipeNumber
        {
            get => _RecipeNumber;
            set => Set(ref _RecipeNumber, value);
        }

        private float _MixerTemperature;

        /// <summary> Температура смесителя </summary>
        public float MixerTemperature
        {
            get => _MixerTemperature;
            set => Set(ref _MixerTemperature, value);
        }

        private float _SetRevertMud;

        /// <summary> Заданный вес обратный шлам </summary>
        public float SetRevertMud
        {
            get => _SetRevertMud;
            set => Set(ref _SetRevertMud, value);
        }

        private float _ActRevertMud;

        /// <summary> Факт вес обратный шлам </summary>
        public float ActRevertMud
        {
            get => _ActRevertMud;
            set => Set(ref _ActRevertMud, value);
        }

        private float _SetSandMud;

        /// <summary> Заданный вес песчанный шлам </summary>
        public float SetSandMud
        {
            get => _SetSandMud;
            set => Set(ref _SetSandMud, value);
        }

        private float _ActSandMud;

        /// <summary> Факт вес песчанный шлам </summary>
        public float ActSandMud
        {
            get => _ActSandMud;
            set => Set(ref _ActSandMud, value);
        }

        private float _setColdWater;

        /// <summary> Заданный вес холодная вода </summary>
        public float SetColdWater
        {
            get => _setColdWater;
            set => Set(ref _setColdWater, value);
        }

        private float _actColdWater;

        /// <summary> Факт вес холодная вода </summary>
        public float ActColdWater
        {
            get => _actColdWater;
            set => Set(ref _actColdWater, value);
        }

        private float _setHotWater;

        /// <summary> Заданный вес горячая вода </summary>
        public float SetHotWater
        {
            get => _setHotWater;
            set => Set(ref _setHotWater, value);
        }

        private float _ActHotWater;

        /// <summary> Факт вес горячая вода </summary>
        public float ActHotWater
        {
            get => _ActHotWater;
            set => Set(ref _ActHotWater, value);
        }

        private float _SetMixture1;

        /// <summary> Заданный вес ИПВ № 1 </summary>
        public float SetMixture1
        {
            get => _SetMixture1;
            set => Set(ref _SetMixture1, value);
        }

        private float _ActMixture1;

        /// <summary> Факт вес ИПВ № 1 </summary>
        public float ActMixture1
        {
            get => _ActMixture1;
            set => Set(ref _ActMixture1, value);
        }

        private float _SetMixture2;

        /// <summary> Заданный вес ИПВ № 2 </summary>
        public float SetMixture2
        {
            get => _SetMixture2;
            set => Set(ref _SetMixture2, value);
        }

        private float _ActMixture2;

        /// <summary> Факт вес ИПВ № 2 </summary>
        public float ActMixture2
        {
            get => _ActMixture2;
            set => Set(ref _ActMixture2, value);
        }

        private float _SetCement1;

        /// <summary> Заданный вес цемента № 1 </summary>
        public float SetCement1
        {
            get => _SetCement1;
            set => Set(ref _SetCement1, value);
        }

        private float _ActCement1;

        /// <summary> Факт вес цемента 1 </summary>
        public float ActCement1
        {
            get => _ActCement1;
            set => Set(ref _ActCement1, value);
        }

        private float _SetCement2;

        /// <summary> Заданный вес цемента 2 </summary>
        public float SetCement2
        {
            get => _SetCement2;
            set => Set(ref _SetCement2, value);
        }

        private float _ActCement2;

        /// <summary> Факт вес цемента 2 </summary>
        public float ActCement2
        {
            get => _ActCement2;
            set => Set(ref _ActCement2, value);
        }

        private float _SetAluminium1;

        /// <summary> Заданный вес алюминия 1 </summary>
        public float SetAluminium1
        {
            get => _SetAluminium1;
            set => Set(ref _SetAluminium1, value);
        }

        private float _ActAluminium1;

        /// <summary> Факт вес алюминия 1 </summary>
        public float ActAluminium1
        {
            get => _ActAluminium1;
            set => Set(ref _ActAluminium1, value);
        }

        private float _SetAluminium2;

        /// <summary> Заданный вес алюминия 2 </summary>
        public float SetAluminium2
        {
            get => _SetAluminium2;
            set => Set(ref _SetAluminium2, value);
        }

        private float _actAluminium2;

        /// <summary> Факт вес алюминия 2 </summary>
        public float ActAluminium2
        {
            get => _actAluminium2;
            set => Set(ref _actAluminium2, value);
        }

        private float _SandInMud;

        /// <summary> Песок в шламе </summary>
        public float SandInMud
        {
            get => _SandInMud;
            set => Set(ref _SandInMud, value);
        }

        private bool _Normal;

        /// <summary> Нормальная заливка </summary>
        public bool Normal
        {
            get => _Normal;
            set
            {
                Set(ref _Normal, value);
                OnPropertyChanged(nameof(NormalStr));
            }
        }

        [NotMapped]
        public string NormalStr => (Normal) ? "Норма" : "Ошибка";

        private bool _Undersized;

        /// <summary> Недоросток </summary>
        public bool Undersized
        {
            get => _Undersized;
            set => Set(ref _Undersized, value);
        }

        [NotMapped]
        public string UndersizedStr => (Undersized) ? "Недоросток" : String.Empty;

        private bool _Overground;

        /// <summary> Переросток </summary>
        public bool Overground
        {
            get => _Overground;
            set => Set(ref _Overground, value);
        }

        [NotMapped]
        public string OvergroundStr => (Overground) ? "Переросток" : string.Empty;

        private bool _Boiled;

        /// <summary> Закипевший </summary>
        public bool Boiled
        {
            get => _Boiled;
            set => Set(ref _Boiled, value);
        }

        [NotMapped]
        public string BoiledStr => (Boiled) ? "Закипел" : string.Empty;

        private bool _Other;

        /// <summary> Другое </summary>
        public bool Other
        {
            get => _Other;
            set => Set(ref _Other, value);
        }

        [NotMapped]
        public string OtherStr => (Other) ? "Другое" : string.Empty;

        /// <summary> Характеристики в виде строки </summary>
        [NotMapped]
        public string CharsStr
        {
            get
            {
                List<string> strs = new List<string>();
                if (Undersized) strs.Add(UndersizedStr);
                if (Overground) strs.Add(OvergroundStr);
                if (Boiled) strs.Add(BoiledStr);
                if (Other) strs.Add(OtherStr);
                if (strs.Count > 1)
                    return strs.Aggregate((m, n) => m + ',' + n);
                else if (strs.Count > 0)
                    return strs.First();
                else
                    return string.Empty;
            }
        }

        private string _Comment;

        /// <summary> Комментарий </summary>
        [MaxLength(250)]
        public string Comment
        {
            get => _Comment;
            set => Set(ref _Comment, value);
        }
    }

}
