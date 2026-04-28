using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WinFIM.Core.Interfaces;
using WinFIM.Core.Models;

namespace WinFIM.Core.Services
{
    public class BaselineService : IBaselineService
    {
        private readonly IWinFimDbContext _dbContext;
        private readonly IHashingService _hashingService;

        public BaselineService(IWinFimDbContext dbContext, IHashingService hashingService)
        {
            _dbContext = dbContext;
            _hashingService = hashingService;
        }

        public async Task CreateBaselineAsync(Action<string> progressCallback = null!)
        {
            progressCallback?.Invoke("Iniciando criação de baseline...");

            var activeDirectories = await _dbContext.MonitoredDirectories
                .Where(d => d.IsActive)
                .ToListAsync();

            if (activeDirectories.Count == 0)
            {
                progressCallback?.Invoke("Nenhum diretório ativo para monitoramento.");
                return;
            }

            var snapshot = new BaselineSnapshot
            {
                CreatedAt = DateTime.Now,
                Status = "In Progress",
                Notes = "Baseline manual"
            };

            _dbContext.BaselineSnapshots.Add(snapshot);
            await _dbContext.SaveChangesAsync(); // Save to get the ID

            int totalFilesProcessed = 0;

            foreach (var dir in activeDirectories)
            {
                if (!Directory.Exists(dir.Path))
                {
                    progressCallback?.Invoke($"Diretório não encontrado: {dir.Path}");
                    continue;
                }

                progressCallback?.Invoke($"Processando diretório: {dir.Path}");

                try
                {
                    var files = Directory.GetFiles(dir.Path, "*.*", SearchOption.AllDirectories);
                    
                    foreach (var filePath in files)
                    {
                        var fileInfo = new FileInfo(filePath);
                        var hash = await _hashingService.CalculateSha256Async(filePath);

                        var baselineFile = new BaselineFile
                        {
                            SnapshotId = snapshot.Id,
                            DirectoryId = dir.Id,
                            FullPath = fileInfo.FullName,
                            FileName = fileInfo.Name,
                            Extension = fileInfo.Extension,
                            SizeBytes = fileInfo.Length,
                            LastModifiedAt = fileInfo.LastWriteTime,
                            Sha256Hash = hash
                        };

                        _dbContext.BaselineFiles.Add(baselineFile);
                        totalFilesProcessed++;

                        if (totalFilesProcessed % 50 == 0)
                        {
                            progressCallback?.Invoke($"Processados {totalFilesProcessed} arquivos...");
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    progressCallback?.Invoke($"Acesso negado ao processar: {dir.Path}");
                }
                catch (Exception ex)
                {
                    progressCallback?.Invoke($"Erro ao processar {dir.Path}: {ex.Message}");
                }
            }

            snapshot.Status = "Completed";
            await _dbContext.SaveChangesAsync();

            progressCallback?.Invoke($"Baseline concluída. Total de arquivos: {totalFilesProcessed}.");
        }
    }
}
