using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WinFIM.Core.Interfaces;
using WinFIM.Core.Models;

namespace WinFIM.Core.Services
{
    public class FileMonitorService : IFileMonitorService, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ConcurrentDictionary<int, FileSystemWatcher> _watchers = new();

        public FileMonitorService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public void ConfigureDirectories(IEnumerable<MonitoredDirectory> directories)
        {
            StopMonitoring();

            foreach (var dir in directories.Where(d => d.IsActive))
            {
                if (!Directory.Exists(dir.Path)) continue;

                var watcher = new FileSystemWatcher
                {
                    Path = dir.Path,
                    NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite | NotifyFilters.Size,
                    IncludeSubdirectories = true
                };

                watcher.Created += (s, e) => OnChanged(e, "CREATED", dir.Id);
                watcher.Changed += (s, e) => OnChanged(e, "MODIFIED", dir.Id);
                watcher.Deleted += (s, e) => OnChanged(e, "DELETED", dir.Id);
                watcher.Renamed += (s, e) => OnRenamed(e, dir.Id);

                _watchers[dir.Id] = watcher;
            }
        }

        public void StartMonitoring()
        {
            foreach (var watcher in _watchers.Values)
            {
                watcher.EnableRaisingEvents = true;
            }
        }

        public void StopMonitoring()
        {
            foreach (var watcher in _watchers.Values)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }
            _watchers.Clear();
        }

        private async void OnChanged(FileSystemEventArgs e, string eventType, int directoryId)
        {
            await SaveEventAsync(new FileEvent
            {
                DirectoryId = directoryId,
                EventType = eventType,
                FullPath = e.FullPath,
                DetectedAt = DateTime.Now,
                DetectionSource = "Watcher",
                Severity = DetermineSeverity(e.FullPath, eventType)
            });
        }

        private async void OnRenamed(RenamedEventArgs e, int directoryId)
        {
            await SaveEventAsync(new FileEvent
            {
                DirectoryId = directoryId,
                EventType = "RENAMED",
                FullPath = e.FullPath,
                OldPath = e.OldFullPath,
                DetectedAt = DateTime.Now,
                DetectionSource = "Watcher",
                Severity = DetermineSeverity(e.FullPath, "RENAMED")
            });
        }

        private string DetermineSeverity(string path, string eventType)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            if (ext == ".exe" || ext == ".dll" || ext == ".sys" || ext == ".bat" || ext == ".ps1")
            {
                return "High";
            }
            if (eventType == "DELETED" && (ext == ".docx" || ext == ".pdf" || ext == ".xlsx"))
            {
                return "High";
            }
            return "Medium";
        }

        private async System.Threading.Tasks.Task SaveEventAsync(FileEvent fileEvent)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<IWinFimDbContext>();
                
                dbContext.FileEvents.Add(fileEvent);
                await dbContext.SaveChangesAsync();
            }
            catch
            {
                // In a real app we'd log this. MVP simply drops if DB fails.
            }
        }

        public void Dispose()
        {
            StopMonitoring();
        }
    }
}
