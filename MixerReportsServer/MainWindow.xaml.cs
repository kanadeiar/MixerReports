using System.Windows;

namespace MixerReportsServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Действительно закрыть приложение?\nВедь данные по заливкам больше не будут записываться в базу данных!\nОбязательно оставляйте одно запущенное приложение в БСУ при заливках для формирования отчетности по заливкам!", "Завершение работы приложения", MessageBoxButton.YesNo, MessageBoxImage.Hand, MessageBoxResult.No) == MessageBoxResult.No)
                e.Cancel = true;
        }
    }
}
