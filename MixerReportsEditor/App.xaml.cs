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
using MixerReportsEditor.ViewModel;

namespace MixerReportsEditor
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
        /// <summary> Сервисы </summary>
        public static IServiceProvider Services => __Services ??= GetServices().BuildServiceProvider();

        private static string __DefaultConnectionString;
        /// <summary> Строка подключения к базе данных </summary>
        public static string DefaultConnectionString => __DefaultConnectionString ??= GetDefaultConnectionString();
        private static void InitializeServices(IServiceCollection services)
        {
            services.AddDbContext<SPBSUMixerRaportsEntities>(c => c.UseSqlServer(GetDefaultConnectionString(), o => o.EnableRetryOnFailure()));
            
            services.AddScoped<MainWindowViewModel>();

            services.AddScoped<IRepository<Mix>, MixRepository>();

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
