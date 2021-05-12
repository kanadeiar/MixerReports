using Microsoft.Extensions.DependencyInjection;

namespace MixerRaportsViewer.ViewModels
{
    class ViewModelLocator
    {
        public MainWindowViewModel MainWindowViewModel => App.Services
            .GetRequiredService<MainWindowViewModel>();
    }
}
