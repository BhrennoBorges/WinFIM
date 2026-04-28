using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WinFIM.Core.Interfaces;
using WinFIM.Core.Models;

namespace WinFIM.Core.Services
{
    public class RevalidationService : IRevalidationService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHashingService _hashingService;

        public RevalidationService(IServiceScopeFactory scopeFactory, IHashingService hashingService)
        {
            _scopeFactory = scopeFactory;
            _hashingService = hashingService;
        }

        public async Task RunRevalidationAsync(Action<string> progressCallback = null!)
        {
            progressCallback?.Invoke("Iniciando revalidação...");

            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IWinFimDbContext>();

            var activeDirectories = await dbContext.MonitoredDirectories
                .Where(d => d.IsActive)
                .Include(d => d.BaselineFiles)
                .ToListAsync();

            if (activeDirectories.Count == 0)
            {
                progressCallback?.Invoke("Nenhum diretório ativo para revalidação.");
                return;
            }

            var latestSnapshot = await dbContext.BaselineSnapshots
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync();

            if (latestSnapshot == null)
            {
                progressCallback?.Invoke("Nenhuma baseline encontrada para comparar.");
                return;
            }

            int eventsGenerated = 0;

            foreach (var dir in activeDirectories)
            {
                if (!Directory.Exists(dir.Path)) continue;

                progressCallback?.Invoke($"Revalidando diretório: {dir.Path}");

                var baselineFilesForDir = await dbContext.BaselineFiles
                    .Where(b => b.DirectoryId == dir.Id && b.SnapshotId == latestSnapshot.Id)
                    .ToDictionaryAsync(b => b.FullPath);

                try
                {
                    var currentFiles = Directory.GetFiles(dir.Path, "*.*", SearchOption.AllDirectories);
                    
                    // Check for Modified and Created
                    foreach (var filePath in currentFiles)
                    {
                        var fileInfo = new FileInfo(filePath);
                        var currentHash = await _hashingService.CalculateSha256Async(filePath);

                        if (baselineFilesForDir.TryGetValue(filePath, out var baselineFile))
                        {
                            if (baselineFile.Sha256Hash != currentHash)
                            {
                                // File was modified
                                dbContext.FileEvents.Add(new FileEvent
                                {
                                    DirectoryId = dir.Id,
                                    EventType = "MODIFIED",
                                    Severity = "High", // We treat hash mismatch as High severity during revalidation
                                    FullPath = filePath,
                                    DetectedAt = DateTime.Now,
                                    DetectionSource = "Revalidation",
                                    Details = $"Hash alterado. Antigo: {baselineFile.Sha256Hash}, Novo: {currentHash}"
                                });
                                eventsGenerated++;
                            }
                            // Remove from dictionary so we know which ones are missing (deleted)
                            baselineFilesForDir.Remove(filePath);
                        }
                        else
                        {
                            // File was created (not in baseline)
                            dbContext.FileEvents.Add(new FileEvent
                            {
                                DirectoryId = dir.Id,
                                EventType = "CREATED",
                                Severity = "Medium",
                                FullPath = filePath,
                                DetectedAt = DateTime.Now,
                                DetectionSource = "Revalidation",
                                Details = $"Arquivo não existia na baseline."
                            });
                            eventsGenerated++;
                        }
                    }

                    // Any remaining files in baselineFilesForDir were deleted
                    foreach (var deletedFile in baselineFilesForDir.Values)
                    {
                        dbContext.FileEvents.Add(new FileEvent
                        {
                            DirectoryId = dir.Id,
                            EventType = "DELETED",
                            Severity = "Medium",
                            FullPath = deletedFile.FullPath,
                            DetectedAt = DateTime.Now,
                            DetectionSource = "Revalidation",
                            Details = $"Arquivo presente na baseline não foi encontrado."
                        });
                        eventsGenerated++;
                    }
                }
                catch (Exception ex)
                {
                    progressCallback?.Invoke($"Erro ao revalidar {dir.Path}: {ex.Message}");
                }
            }

            if (eventsGenerated > 0)
            {
                await dbContext.SaveChangesAsync();
            }

            progressCallback?.Invoke($"Revalidação concluída. Eventos gerados: {eventsGenerated}.");
        }
    }
}
