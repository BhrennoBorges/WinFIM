using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using WinFIM.Core.Models;

namespace WinFIM.Core.Interfaces
{
    public interface IWinFimDbContext
    {
        DbSet<MonitoredDirectory> MonitoredDirectories { get; set; }
        DbSet<BaselineSnapshot> BaselineSnapshots { get; set; }
        DbSet<BaselineFile> BaselineFiles { get; set; }
        DbSet<FileEvent> FileEvents { get; set; }
        DbSet<AppSetting> AppSettings { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
