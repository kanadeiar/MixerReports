using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using MixerReportsCorrector.ViewModel;

namespace MixerReportsCorrector
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
            services.AddScoped<MainWindowViewModel>();

        }

    }
}
