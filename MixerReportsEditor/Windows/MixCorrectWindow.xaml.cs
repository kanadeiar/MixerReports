using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MixerReports.lib.Models;

namespace MixerReportsEditor.Windows
{
    /// <summary>
    /// Логика взаимодействия для MixCorrectWindow.xaml
    /// </summary>
    public partial class MixCorrectWindow : Window
    {
        public Mix Mix { get; set; }
        public MixCorrectWindow()
        {
            InitializeComponent();
        }
        /// <summary> Корректирование данных по заливке </summary>
        /// <param name="FormNumber">Номер формы</param>
        /// <param name="RecipeNumber">Номер рецепта</param>
        /// <param name="Normal">Норма</param>
        /// <param name="Undersized">Недоросток</param>
        /// <param name="Overground">Переросток</param>
        /// <param name="Boiled">Вскипел</param>
        /// <param name="IsMud">Шлам</param>
        /// <param name="IsExperiment">Эксперимент</param>
        /// <param name="Other">Другое</param>
        /// <param name="Comment">Комментарий</param>
        public static bool Correct(ref int FormNumber, ref int RecipeNumber, ref bool Normal, ref bool Undersized, ref bool Overground, 
            ref bool Boiled, ref bool IsMud, ref bool IsExperiment, ref bool Other, ref string Comment, string Title, int Number, DateTime DateTime, float MixerTemperature,
            float SetSandMud, float ActSandMud, float SetRevertMud, float ActRevertMud)
        {
            var window = new MixCorrectWindow
            {
                Title = Title,
                Mix = new Mix
                {
                    FormNumber = FormNumber,
                    RecipeNumber = RecipeNumber,
                    Normal = Normal,
                    Undersized = Undersized,
                    Overground = Overground,
                    Boiled = Boiled,
                    IsMud = IsMud,
                    IsExperiment = IsExperiment,
                    Other = Other,
                    Comment = Comment,
                    Number = Number,
                    DateTime = DateTime,
                    MixerTemperature = MixerTemperature,
                    SetSandMud = SetSandMud,
                    ActSandMud = ActSandMud,
                    SetRevertMud = SetRevertMud,
                    ActRevertMud = ActRevertMud,
                },
                Owner = Application.Current.Windows.Cast<Window>()
                    .FirstOrDefault(win => win.IsActive),
            };
            window.DockPanelCorrectMix.DataContext = window.Mix;
            if (window.ShowDialog() != true) return false;
            FormNumber = window.Mix.FormNumber;
            RecipeNumber = window.Mix.RecipeNumber;
            Normal = window.Mix.Normal;
            Undersized = window.Mix.Undersized;
            Overground = window.Mix.Overground;
            Boiled = window.Mix.Boiled;
            IsMud = window.Mix.IsMud;
            IsExperiment = window.Mix.IsExperiment;
            Other = window.Mix.Other;
            Comment = window.Mix.Comment;
            return true;
        }

        private void WindowButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = !((Button)e.OriginalSource).IsCancel;
            Close();
        }
    }
}
