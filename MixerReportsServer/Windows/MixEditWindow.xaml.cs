using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MixerReports.lib.Models;

namespace MixerReportsServer.Windows
{
    /// <summary>
    /// Логика взаимодействия для MixEditWindow.xaml
    /// </summary>
    public partial class MixEditWindow : Window
    {
        public string LocTitle { get; set; } = "Заголовок окна";

        #region Свойства зависимостей редактирования заливки

        public static readonly DependencyProperty NumberProperty = DependencyProperty.Register(
            "Number", typeof(int), typeof(MixEditWindow), new PropertyMetadata(default(int)));
        public int Number
        {
            get { return (int) GetValue(NumberProperty); }
            set { SetValue(NumberProperty, value); }
        }

        public static readonly DependencyProperty DateTimeProperty = DependencyProperty.Register(
            "DateTime", typeof(DateTime), typeof(MixEditWindow), new PropertyMetadata(default(DateTime)));
        public DateTime DateTime
        {
            get { return (DateTime) GetValue(DateTimeProperty); }
            set { SetValue(DateTimeProperty, value); }
        }

        public static readonly DependencyProperty FormNumberProperty = DependencyProperty.Register(
            "FormNumber", typeof(int), typeof(MixEditWindow), new PropertyMetadata(default(int)));
        public int FormNumber
        {
            get { return (int) GetValue(FormNumberProperty); }
            set { SetValue(FormNumberProperty, value); }
        }

        public static readonly DependencyProperty RecipeNumberProperty = DependencyProperty.Register(
            "RecipeNumber", typeof(int), typeof(MixEditWindow), new PropertyMetadata(default(int)));
        public int RecipeNumber
        {
            get { return (int) GetValue(RecipeNumberProperty); }
            set { SetValue(RecipeNumberProperty, value); }
        }

        public static readonly DependencyProperty MixerTemperatureProperty = DependencyProperty.Register(
            "MixerTemperature", typeof(float), typeof(MixEditWindow), new PropertyMetadata(default(float)));
        public float MixerTemperature
        {
            get { return (float) GetValue(MixerTemperatureProperty); }
            set { SetValue(MixerTemperatureProperty, value); }
        }

        #region Компоненты

        public static readonly DependencyProperty SetRevertMudProperty = DependencyProperty.Register(
            "SetRevertMud", typeof(float), typeof(MixEditWindow), new PropertyMetadata(default(float)));
        public float SetRevertMud
        {
            get { return (float) GetValue(SetRevertMudProperty); }
            set { SetValue(SetRevertMudProperty, value); }
        }

        public static readonly DependencyProperty ActRevertMudProperty = DependencyProperty.Register(
            "ActRevertMud", typeof(float), typeof(MixEditWindow), new PropertyMetadata(default(float)));
        public float ActRevertMud
        {
            get { return (float) GetValue(ActRevertMudProperty); }
            set { SetValue(ActRevertMudProperty, value); }
        }

        public static readonly DependencyProperty SetSandMudProperty = DependencyProperty.Register(
            "SetSandMud", typeof(float), typeof(MixEditWindow), new PropertyMetadata(default(float)));
        public float SetSandMud
        {
            get { return (float) GetValue(SetSandMudProperty); }
            set { SetValue(SetSandMudProperty, value); }
        }

        public static readonly DependencyProperty ActSandMudProperty = DependencyProperty.Register(
            "ActSandMud", typeof(float), typeof(MixEditWindow), new PropertyMetadata(default(float)));
        public float ActSandMud
        {
            get { return (float) GetValue(ActSandMudProperty); }
            set { SetValue(ActSandMudProperty, value); }
        }

        public static readonly DependencyProperty SetColdWaterProperty = DependencyProperty.Register(
            "SetColdWater", typeof(float), typeof(MixEditWindow), new PropertyMetadata(default(float)));
        public float SetColdWater
        {
            get { return (float) GetValue(SetColdWaterProperty); }
            set { SetValue(SetColdWaterProperty, value); }
        }

        public static readonly DependencyProperty ActColdWaterProperty = DependencyProperty.Register(
            "ActColdWater", typeof(float), typeof(MixEditWindow), new PropertyMetadata(default(float)));
        public float ActColdWater
        {
            get { return (float) GetValue(ActColdWaterProperty); }
            set { SetValue(ActColdWaterProperty, value); }
        }

        public static readonly DependencyProperty SetHotWaterProperty = DependencyProperty.Register(
            "SetHotWater", typeof(float), typeof(MixEditWindow), new PropertyMetadata(default(float)));
        public float SetHotWater
        {
            get { return (float) GetValue(SetHotWaterProperty); }
            set { SetValue(SetHotWaterProperty, value); }
        }

        public static readonly DependencyProperty ActHotWaterProperty = DependencyProperty.Register(
            "ActHotWater", typeof(float), typeof(MixEditWindow), new PropertyMetadata(default(float)));
        public float ActHotWater
        {
            get { return (float) GetValue(ActHotWaterProperty); }
            set { SetValue(ActHotWaterProperty, value); }
        }

        public static readonly DependencyProperty SetMixture1Property = DependencyProperty.Register(
            "SetMixture1", typeof(float), typeof(MixEditWindow), new PropertyMetadata(default(float)));
        public float SetMixture1
        {
            get { return (float) GetValue(SetMixture1Property); }
            set { SetValue(SetMixture1Property, value); }
        }

        public static readonly DependencyProperty ActMixture1Property = DependencyProperty.Register(
            "ActMixture1", typeof(float), typeof(MixEditWindow), new PropertyMetadata(default(float)));
        public float ActMixture1
        {
            get { return (float) GetValue(ActMixture1Property); }
            set { SetValue(ActMixture1Property, value); }
        }

        public static readonly DependencyProperty SetMixture2Property = DependencyProperty.Register(
            "SetMixture2", typeof(float), typeof(MixEditWindow), new PropertyMetadata(default(float)));
        public float SetMixture2
        {
            get { return (float) GetValue(SetMixture2Property); }
            set { SetValue(SetMixture2Property, value); }
        }

        public static readonly DependencyProperty ActMixture2Property = DependencyProperty.Register(
            "ActMixture2", typeof(float), typeof(MixEditWindow), new PropertyMetadata(default(float)));
        public float ActMixture2
        {
            get { return (float) GetValue(ActMixture2Property); }
            set { SetValue(ActMixture2Property, value); }
        }

        public static readonly DependencyProperty SetCement1Property = DependencyProperty.Register(
            "SetCement1", typeof(float), typeof(MixEditWindow), new PropertyMetadata(default(float)));
        public float SetCement1
        {
            get { return (float) GetValue(SetCement1Property); }
            set { SetValue(SetCement1Property, value); }
        }

        public static readonly DependencyProperty ActCement1Property = DependencyProperty.Register(
            "ActCement1", typeof(float), typeof(MixEditWindow), new PropertyMetadata(default(float)));
        public float ActCement1
        {
            get { return (float) GetValue(ActCement1Property); }
            set { SetValue(ActCement1Property, value); }
        }

        public static readonly DependencyProperty SetCement2Property = DependencyProperty.Register(
            "SetCement2", typeof(float), typeof(MixEditWindow), new PropertyMetadata(default(float)));
        public float SetCement2
        {
            get { return (float) GetValue(SetCement2Property); }
            set { SetValue(SetCement2Property, value); }
        }

        public static readonly DependencyProperty ActCement2Property = DependencyProperty.Register(
            "ActCement2", typeof(float), typeof(MixEditWindow), new PropertyMetadata(default(float)));
        public float ActCement2
        {
            get { return (float) GetValue(ActCement2Property); }
            set { SetValue(ActCement2Property, value); }
        }

        public static readonly DependencyProperty SetAluminium1Property = DependencyProperty.Register(
            "SetAluminium1", typeof(float), typeof(MixEditWindow), new PropertyMetadata(default(float)));
        public float SetAluminium1
        {
            get { return (float) GetValue(SetAluminium1Property); }
            set { SetValue(SetAluminium1Property, value); }
        }

        public static readonly DependencyProperty ActAluminium1Property = DependencyProperty.Register(
            "ActAluminium1", typeof(float), typeof(MixEditWindow), new PropertyMetadata(default(float)));
        public float ActAluminium1
        {
            get { return (float) GetValue(ActAluminium1Property); }
            set { SetValue(ActAluminium1Property, value); }
        }

        public static readonly DependencyProperty SetAluminium2Property = DependencyProperty.Register(
            "SetAluminium2", typeof(float), typeof(MixEditWindow), new PropertyMetadata(default(float)));
        public float SetAluminium2
        {
            get { return (float) GetValue(SetAluminium2Property); }
            set { SetValue(SetAluminium2Property, value); }
        }

        public static readonly DependencyProperty ActAluminium2Property = DependencyProperty.Register(
            "ActAluminium2", typeof(float), typeof(MixEditWindow), new PropertyMetadata(default(float)));
        public float ActAluminium2
        {
            get { return (float) GetValue(ActAluminium2Property); }
            set { SetValue(ActAluminium2Property, value); }
        }

        public static readonly DependencyProperty SandInMudProperty = DependencyProperty.Register(
            "SandInMud", typeof(float), typeof(MixEditWindow), new PropertyMetadata(default(float)));
        public float SandInMud
        {
            get { return (float) GetValue(SandInMudProperty); }
            set { SetValue(SandInMudProperty, value); }
        }

        public static readonly DependencyProperty DensitySandMudProperty = DependencyProperty.Register(
            "DensitySandMud", typeof(float), typeof(MixEditWindow), new PropertyMetadata(default(float)));
        public float DensitySandMud
        {
            get { return (float) GetValue(DensitySandMudProperty); }
            set { SetValue(DensitySandMudProperty, value); }
        }

        public static readonly DependencyProperty DensityRevertMudProperty = DependencyProperty.Register(
            "DensityRevertMud", typeof(float), typeof(MixEditWindow), new PropertyMetadata(default(float)));
        public float DensityRevertMud
        {
            get { return (float) GetValue(DensityRevertMudProperty); }
            set { SetValue(DensityRevertMudProperty, value); }
        }

        #endregion

        #region Характеристики

        public static readonly DependencyProperty NormalProperty = DependencyProperty.Register(
            "Normal", typeof(bool), typeof(MixEditWindow), new PropertyMetadata(default(bool)));
        public bool Normal
        {
            get { return (bool) GetValue(NormalProperty); }
            set { SetValue(NormalProperty, value); }
        }

        public static readonly DependencyProperty UndersizedProperty = DependencyProperty.Register(
            "Undersized", typeof(bool), typeof(MixEditWindow), new PropertyMetadata(default(bool)));
        public bool Undersized
        {
            get { return (bool) GetValue(UndersizedProperty); }
            set { SetValue(UndersizedProperty, value); }
        }

        public static readonly DependencyProperty OvergroundProperty = DependencyProperty.Register(
            "Overground", typeof(bool), typeof(MixEditWindow), new PropertyMetadata(default(bool)));
        public bool Overground
        {
            get { return (bool) GetValue(OvergroundProperty); }
            set { SetValue(OvergroundProperty, value); }
        }

        public static readonly DependencyProperty BoiledProperty = DependencyProperty.Register(
            "Boiled", typeof(bool), typeof(MixEditWindow), new PropertyMetadata(default(bool)));
        public bool Boiled
        {
            get { return (bool) GetValue(BoiledProperty); }
            set { SetValue(BoiledProperty, value); }
        }

        public static readonly DependencyProperty IsMudProperty = DependencyProperty.Register(
            "IsMud", typeof(bool), typeof(MixEditWindow), new PropertyMetadata(default(bool)));
        public bool IsMud
        {
            get { return (bool) GetValue(IsMudProperty); }
            set { SetValue(IsMudProperty, value); }
        }

        public static readonly DependencyProperty IsExperimentProperty = DependencyProperty.Register(
            "IsExperiment", typeof(bool), typeof(MixEditWindow), new PropertyMetadata(default(bool)));
        public bool IsExperiment
        {
            get { return (bool) GetValue(IsExperimentProperty); }
            set { SetValue(IsExperimentProperty, value); }
        }

        public static readonly DependencyProperty OtherProperty = DependencyProperty.Register(
            "Other", typeof(bool), typeof(MixEditWindow), new PropertyMetadata(default(bool)));
        public bool Other
        {
            get { return (bool) GetValue(OtherProperty); }
            set { SetValue(OtherProperty, value); }
        }

        public static readonly DependencyProperty CommentProperty = DependencyProperty.Register(
            "Comment", typeof(string), typeof(MixEditWindow), new PropertyMetadata(default(string)));
        public string Comment
        {
            get { return (string) GetValue(CommentProperty); }
            set { SetValue(CommentProperty, value); }
        }

        #endregion

        #endregion

        public MixEditWindow()
        {
            InitializeComponent();
        }
        /// <summary> Редактирование заливки </summary>
        /// <param name="Title">Заголовок</param>
        /// <param name="mix">Заливка</param>
        /// <returns>Нормальный результат выполнения</returns>
        public static bool ShowEdit(ref Mix mix, string Title = "Редактирование выбранной заливки")
        {
            var window = new MixEditWindow
            {
                LocTitle = Title,
                Owner = Application.Current.Windows.Cast<Window>()
                    .FirstOrDefault(win => win.IsActive),
            };
            #region Копирование данных из параметра в окно

            window.Number = mix.Number;
            window.DateTime = mix.DateTime;
            window.FormNumber = mix.FormNumber;
            window.RecipeNumber = mix.RecipeNumber;
            window.MixerTemperature = mix.MixerTemperature;
            window.SetRevertMud = mix.SetRevertMud;
            window.ActRevertMud = mix.ActRevertMud;
            window.SetSandMud = mix.SetSandMud;
            window.ActSandMud = mix.ActSandMud;
            window.SetColdWater = mix.SetColdWater;
            window.ActColdWater = mix.ActColdWater;
            window.SetHotWater = mix.SetHotWater;
            window.ActHotWater = mix.ActHotWater;
            window.SetMixture1 = mix.SetMixture1;
            window.ActMixture1 = mix.ActMixture1;
            window.SetMixture2 = mix.SetMixture2;
            window.ActMixture2 = mix.ActMixture2;
            window.SetCement1 = mix.SetCement1;
            window.ActCement1 = mix.ActCement1;
            window.SetCement2 = mix.SetCement2;
            window.ActCement2 = mix.ActCement2;
            window.SetAluminium1 = mix.SetAluminium1;
            window.ActAluminium1 = mix.ActAluminium1;
            window.SetAluminium2 = mix.SetAluminium2;
            window.ActAluminium2 = mix.ActAluminium2;
            window.SandInMud = mix.SandInMud;
            window.DensitySandMud = mix.DensitySandMud;
            window.DensityRevertMud = mix.DensityRevertMud;
            window.Normal = mix.Normal;
            window.Undersized = mix.Undersized;
            window.Overground = mix.Overground;
            window.Boiled = mix.Boiled;
            window.IsExperiment = mix.IsExperiment;
            window.IsMud = mix.IsMud;
            window.Other = mix.Other;
            window.Comment = mix.Comment;

            #endregion
            if (window.ShowDialog() != true) return false;
            #region Копирование данных из окна в параметр

            mix.Number = window.Number;
            mix.DateTime = window.DateTime;
            mix.FormNumber = window.FormNumber;
            mix.RecipeNumber = window.RecipeNumber;
            mix.MixerTemperature = window.MixerTemperature;
            mix.SetRevertMud = window.SetRevertMud;
            mix.ActRevertMud = window.ActRevertMud;
            mix.SetSandMud = window.SetSandMud;
            mix.ActSandMud = window.ActSandMud;
            mix.SetColdWater = window.SetColdWater;
            mix.ActColdWater = window.ActColdWater;
            mix.SetHotWater = window.SetHotWater;
            mix.ActHotWater = window.ActHotWater;
            mix.SetMixture1 = window.SetMixture1;
            mix.ActMixture1 = window.ActMixture1;
            mix.SetMixture2 = window.SetMixture2;
            mix.ActMixture2 = window.ActMixture2;
            mix.SetCement1 = window.SetCement1;
            mix.ActCement1 = window.ActCement1;
            mix.SetCement2 = window.SetCement2;
            mix.ActCement2 = window.ActCement2;
            mix.SetAluminium1 = window.SetAluminium1;
            mix.ActAluminium1 = window.ActAluminium1;
            mix.SetAluminium2 = window.SetAluminium2;
            mix.ActAluminium2 = window.ActAluminium2;
            mix.SandInMud = window.SandInMud;
            mix.DensitySandMud = window.DensitySandMud;
            mix.DensityRevertMud = window.DensityRevertMud;
            mix.Normal = window.Normal;
            mix.Undersized = window.Undersized;
            mix.Overground = window.Overground;
            mix.Boiled = window.Boiled;
            mix.IsExperiment = window.IsExperiment;
            mix.IsMud = window.IsMud;
            mix.Other = window.Other;
            mix.Comment = window.Comment;

            #endregion
            return true;
        }
        /// <summary> Создание новой заливки </summary>
        /// <param name="mix">Заливка</param>
        /// <returns>Нормальный результат выполнения</returns>
        public static bool Create(out Mix mix)
        {
            mix = new Mix();
            mix.DateTime = DateTime.Now;
            return ShowEdit(ref mix, "Создание новой заливки");
        }
        private void WindowButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = !((Button)e.OriginalSource).IsCancel;
            Close();
        }
    }
}
