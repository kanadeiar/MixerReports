using Microsoft.Extensions.DependencyInjection;

namespace MixerReportsEditor.ViewModel
{
    class ViewModelLocator
    {
        public MainWindowViewModel MainWindowViewModel => App.Services
            .GetRequiredService<MainWindowViewModel>();
    }
}
