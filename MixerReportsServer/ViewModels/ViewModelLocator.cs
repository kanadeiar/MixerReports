using Microsoft.Extensions.DependencyInjection;

namespace MixerReportsServer.ViewModels
{
    class ViewModelLocator
    {
        public MainWindowViewModel MainWindowViewModel => App.Services
            .GetRequiredService<MainWindowViewModel>();
    }
}
