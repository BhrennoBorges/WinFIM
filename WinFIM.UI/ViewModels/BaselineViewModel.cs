using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using WinFIM.Core.Services;

namespace WinFIM.UI.ViewModels
{
    public partial class BaselineViewModel : ObservableObject
    {
        private readonly IBaselineService _baselineService;

        [ObservableProperty]
        private string _progressText = "Pronto para iniciar.";

        [ObservableProperty]
        private bool _isProcessing = false;

        public BaselineViewModel(IBaselineService baselineService)
        {
            _baselineService = baselineService;
        }

        [RelayCommand]
        private async Task CreateBaselineAsync()
        {
            IsProcessing = true;
            try
            {
                await Task.Run(() => _baselineService.CreateBaselineAsync(progress =>
                {
                    App.Current.Dispatcher.Invoke(() => ProgressText = progress);
                }));
            }
            finally
            {
                IsProcessing = false;
            }
        }
    }
}
