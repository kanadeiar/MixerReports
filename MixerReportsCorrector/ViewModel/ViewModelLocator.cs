using Microsoft.Extensions.DependencyInjection;

namespace MixerReportsCorrector.ViewModel
{
    class ViewModelLocator
    {
        public MainWindowViewModel MainWindowViewModel => App.Services
            .GetRequiredService<MainWindowViewModel>();
    }
}
