using System;
using System.Configuration;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using MixerRaports.dbf.Interfaces;
using MixerRaports.dbf.Services;
using MixerReports.lib.Interfaces;
using MixerReports.lib.Services;
using MixerReportsServer.ViewModels;
using Sharp7;

namespace MixerReportsServer
{
    /// <summary> Interaction logic for App.xaml </summary>
    public partial class App : Application
    {
        private static IServiceProvider __Services;
        private static IServiceCollection GetServices()
        {
            var services = new ServiceCollection();
            InitializeServices(services);
            return services;
        }
        public static IServiceProvider Services => __Services ??= GetServices().BuildServiceProvider();

        private static string __DefaultConnectionString;
        public static string DefaultConnectionString => __DefaultConnectionString ??= GetDefaultConnectionString();

        private static void InitializeServices(IServiceCollection services)
        {
            services.AddScoped<MainWindowViewModel>();

            services.AddScoped<ISharp7MixReaderService>(s =>
            {
                GetAppSettings(out string address, out int aluminiumProp, out int secondsCorrect);
                return new Sharp7EasyMixReaderService(new S7Client{ConnTimeout = 5_000, RecvTimeout = 5_000}, address, aluminiumProp, secondsCorrect);
            });

            services.AddScoped<IDBFConverterService, DBFConverterService>();
        }

        #region Вспомогательное

        private static string GetDefaultConnectionString()
        {
            AppSettingsReader ar = new AppSettingsReader();
            var password = ((string)ar.GetValue("password", typeof(string))).Decrypt();
            var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            return connectionString.Replace("{{password}}", password);
        }
        private static void GetAppSettings(out string address, out int aluminiumProp, out int secondsCorrect)
        {
            AppSettingsReader ar = new AppSettingsReader();
            address = (string)ar.GetValue("address", typeof(string));
            aluminiumProp = (int)ar.GetValue("aluminiumprop", typeof(int));
            secondsCorrect = (int)ar.GetValue("secondscorrect", typeof(int));
        }

        #endregion
    }
}
