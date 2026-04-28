using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using WinFIM.Core.Interfaces;

namespace WinFIM.UI.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly IServiceScopeFactory _scopeFactory;

        [ObservableProperty]
        private int _totalDirectories;

        [ObservableProperty]
        private int _totalEvents;

        [ObservableProperty]
        private string _latestSnapshotStatus = "Nenhuma baseline encontrada";

        public DashboardViewModel(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            LoadStatsAsync();
        }

        private async void LoadStatsAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IWinFimDbContext>();

            TotalDirectories = await db.MonitoredDirectories.CountAsync(d => d.IsActive);
            TotalEvents = await db.FileEvents.CountAsync();

            var latest = await db.BaselineSnapshots.OrderByDescending(s => s.CreatedAt).FirstOrDefaultAsync();
            if (latest != null)
            {
                LatestSnapshotStatus = $"Baseline mais recente: {latest.CreatedAt:dd/MM/yyyy HH:mm} ({latest.Status})";
            }
        }
    }
}
