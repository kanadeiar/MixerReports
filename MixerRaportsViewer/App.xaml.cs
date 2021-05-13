using System;
using System.Configuration;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using MixerRaportsViewer.ViewModels;
using MixerReports.lib.Data;
using MixerReports.lib.Data.Base;
using MixerReports.lib.Interfaces;
using MixerReports.lib.Models;
using MixerReports.lib.Services;

namespace MixerRaportsViewer
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
        
        private static void InitializeServices(IServiceCollection services)
        {
            services.AddDbContext<SPBSUMixerRaportsEntities>(
                c => c.UseSqlServer(DefaultConnectionString, o => o.EnableRetryOnFailure())
                    .ConfigureWarnings(w => w.Throw(RelationalEventId.BoolWithDefaultWarning)));

            services.AddScoped<MainWindowViewModel>();

            services.AddScoped<IRepository<Mix>, MixRepository>();
        }

        private static string __DefaultConnectionString;
        /// <summary> Строка подключения к базе данных </summary>
        public static string DefaultConnectionString => __DefaultConnectionString ??= GetDefaultConnectionString();

        private static string GetDefaultConnectionString()
        {
            AppSettingsReader ar = new AppSettingsReader();
            var password = ((string)ar.GetValue("password", typeof(string))).Decrypt();
            var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            return connectionString.Replace("{{password}}", password);
        }
    }
}
