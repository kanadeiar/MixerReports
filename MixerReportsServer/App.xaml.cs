using System;
using System.Configuration;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MixerReports.lib.Data;
using MixerReports.lib.Data.Base;
using MixerReports.lib.Interfaces;
using MixerReports.lib.Models;
using MixerReports.lib.Services;
using MixerReportsServer.ViewModels;

namespace MixerReportsServer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
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
        /// <summary> Инит сервисов </summary>
        /// <param name="services">сервисы приложения</param>
        private static void InitializeServices(IServiceCollection services)
        {
            services.AddDbContext<SPBSUMixerRaportsEntities>(c => c.UseSqlServer(GetDefaultConnectionString()));

            services.AddScoped<MainWindowViewModel>();

            services.AddScoped<IRepository<Mix>, MixRepository>();

            services.AddScoped<ISharp7ReaderService, Sharp7ReaderService>();
            //services.AddScoped<ISharp7ReaderService, DebugReaderService>();

        }

        private static string GetDefaultConnectionString()
        {
            AppSettingsReader ar = new AppSettingsReader();
            var password = ((string)ar.GetValue("password", typeof(string))).Decrypt();
            var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            return connectionString.Replace("{{password}}", password);
        }
    }
}
