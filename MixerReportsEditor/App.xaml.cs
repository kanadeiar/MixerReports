using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
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
        private static void InitializeServices(IServiceCollection services)
        {
            services.AddScoped<MainWindowViewModel>();
        }
    }
}
