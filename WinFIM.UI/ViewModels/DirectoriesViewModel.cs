using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WinFIM.Core.Interfaces;
using WinFIM.Core.Models;
using WinFIM.Core.Services;

namespace WinFIM.UI.ViewModels
{
    public partial class DirectoriesViewModel : ObservableObject
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IFileMonitorService _monitorService;

        [ObservableProperty]
        private ObservableCollection<MonitoredDirectory> _directories = new();

        [ObservableProperty]
        private string _newDirectoryPath = string.Empty;

        public DirectoriesViewModel(IServiceScopeFactory scopeFactory, IFileMonitorService monitorService)
        {
            _scopeFactory = scopeFactory;
            _monitorService = monitorService;
            LoadDirectoriesAsync();
        }

        private async void LoadDirectoriesAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IWinFimDbContext>();
            var dirs = await db.MonitoredDirectories.ToListAsync();
            Directories = new ObservableCollection<MonitoredDirectory>(dirs);
            
            // Reconfigure watcher
            _monitorService.ConfigureDirectories(dirs);
            _monitorService.StartMonitoring();
        }

        [RelayCommand]
        private async Task AddDirectoryAsync()
        {
            if (string.IsNullOrWhiteSpace(NewDirectoryPath) || !Directory.Exists(NewDirectoryPath))
                return; // Simple validation for MVP

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IWinFimDbContext>();

            var exists = await db.MonitoredDirectories.AnyAsync(d => d.Path == NewDirectoryPath);
            if (!exists)
            {
                var newDir = new MonitoredDirectory
                {
                    Path = NewDirectoryPath,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                db.MonitoredDirectories.Add(newDir);
                await db.SaveChangesAsync();

                Directories.Add(newDir);
                NewDirectoryPath = string.Empty;
                
                _monitorService.ConfigureDirectories(Directories);
                _monitorService.StartMonitoring();
            }
        }

        [RelayCommand]
        private async Task ToggleActiveAsync(MonitoredDirectory directory)
        {
            if (directory == null) return;

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IWinFimDbContext>();
            
            var dbDir = await db.MonitoredDirectories.FindAsync(directory.Id);
            if (dbDir != null)
            {
                dbDir.IsActive = !dbDir.IsActive;
                dbDir.UpdatedAt = DateTime.Now;
                await db.SaveChangesAsync();
                
                directory.IsActive = dbDir.IsActive; // Update UI
                
                _monitorService.ConfigureDirectories(Directories);
                _monitorService.StartMonitoring();
            }
        }

        [RelayCommand]
        private async Task RemoveDirectoryAsync(MonitoredDirectory directory)
        {
            if (directory == null) return;

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IWinFimDbContext>();

            var dbDir = await db.MonitoredDirectories.FindAsync(directory.Id);
            if (dbDir != null)
            {
                db.MonitoredDirectories.Remove(dbDir);
                await db.SaveChangesAsync();

                Directories.Remove(directory);
                
                _monitorService.ConfigureDirectories(Directories);
                _monitorService.StartMonitoring();
            }
        }
    }
}
