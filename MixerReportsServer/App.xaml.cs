﻿using System;
using System.Configuration;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using MixerRaports.dbf.Interfaces;
using MixerRaports.dbf.Services;
using MixerReports.lib.Interfaces;
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

        private static string __DefaultConnectionString;
        public static string DefaultConnectionString => __DefaultConnectionString ??= GetDefaultConnectionString();

        /// <summary> Инит сервисов </summary>
        /// <param name="services">сервисы приложения</param>
        private static void InitializeServices(IServiceCollection services)
        {
            //services.AddDbContext<SPBSUMixerRaportsEntities>(
            //    c => c.UseSqlServer(GetDefaultConnectionString(), o => o.EnableRetryOnFailure()).ConfigureWarnings(w => w.Throw(RelationalEventId.BoolWithDefaultWarning)));
            
            services.AddScoped<MainWindowViewModel>();

            //services.AddScoped<IRepository<Mix>, MixRepository>();
//#if DEBUG
//            services.AddScoped<ISharp7ReaderService, DebugReaderService>();
//#else
//            services.AddScoped<ISharp7ReaderService, Sharp7ReaderService>();
//#endif
            services.AddScoped<ISharp7MixReaderService>(s =>
            {
                GetAppSettings(out string address, out int aluminiumProp, out int secondsCorrect);
                return new Sharp7MixReaderService(address, aluminiumProp, secondsCorrect);
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
