using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WinFIM.Core.Interfaces;
using WinFIM.Core.Models;
using WinFIM.Core.Services;

namespace WinFIM.UI.ViewModels
{
    public partial class EventsViewModel : ObservableObject
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IExportService _exportService;
        private readonly IRevalidationService _revalidationService;

        [ObservableProperty]
        private ObservableCollection<FileEvent> _events = new();

        [ObservableProperty]
        private bool _isRevalidating = false;

        [ObservableProperty]
        private string _revalidationProgress = "";

        public EventsViewModel(IServiceScopeFactory scopeFactory, IExportService exportService, IRevalidationService revalidationService)
        {
            _scopeFactory = scopeFactory;
            _exportService = exportService;
            _revalidationService = revalidationService;
            LoadEventsAsync();
        }

        [RelayCommand]
        private async Task LoadEventsAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IWinFimDbContext>();
            var events = await db.FileEvents.OrderByDescending(e => e.DetectedAt).ToListAsync();
            Events = new ObservableCollection<FileEvent>(events);
        }

        [RelayCommand]
        private async Task RunRevalidationAsync()
        {
            IsRevalidating = true;
            try
            {
                await Task.Run(() => _revalidationService.RunRevalidationAsync(progress =>
                {
                    App.Current.Dispatcher.Invoke(() => RevalidationProgress = progress);
                }));
                await LoadEventsAsync();
            }
            finally
            {
                IsRevalidating = false;
            }
        }

        [RelayCommand]
        private async Task ExportCsvAsync()
        {
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var filePath = Path.Combine(desktop, $"WinFIM_Events_{DateTime.Now:yyyyMMdd_HHmmss}.csv");

            await _exportService.ExportEventsToCsvAsync(filePath, Events);
            
            MessageBox.Show($"Exportado para: {filePath}", "Exportação Concluída", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
    }
}
