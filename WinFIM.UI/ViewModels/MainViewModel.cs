using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace WinFIM.UI.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private object _currentViewModel;

        public MainViewModel()
        {
            _currentViewModel = App.AppHost!.Services.GetRequiredService<DashboardViewModel>();
        }

        [RelayCommand]
        private void NavigateToDashboard() => CurrentViewModel = App.AppHost!.Services.GetRequiredService<DashboardViewModel>();

        [RelayCommand]
        private void NavigateToDirectories() => CurrentViewModel = App.AppHost!.Services.GetRequiredService<DirectoriesViewModel>();

        [RelayCommand]
        private void NavigateToBaseline() => CurrentViewModel = App.AppHost!.Services.GetRequiredService<BaselineViewModel>();

        [RelayCommand]
        private void NavigateToEvents() => CurrentViewModel = App.AppHost!.Services.GetRequiredService<EventsViewModel>();
    }
}
