using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WinFIM.Core.Interfaces;
using WinFIM.Core.Services;
using WinFIM.Data.Context;
using WinFIM.UI.ViewModels;

namespace WinFIM.UI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IHost? AppHost { get; private set; }

    public App()
    {
        AppHost = Host.CreateDefaultBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                // DbContext
                services.AddDbContext<WinFimDbContext>();
                services.AddScoped<IWinFimDbContext>(provider => provider.GetRequiredService<WinFimDbContext>());

                // Services
                services.AddSingleton<IHashingService, HashingService>();
                services.AddSingleton<IBaselineService, BaselineService>();
                services.AddSingleton<IFileMonitorService, FileMonitorService>();
                services.AddSingleton<IRevalidationService, RevalidationService>();
                services.AddSingleton<IExportService, ExportService>();

                // ViewModels
                services.AddSingleton<MainViewModel>();
                services.AddTransient<DashboardViewModel>();
                services.AddTransient<DirectoriesViewModel>();
                services.AddTransient<BaselineViewModel>();
                services.AddTransient<EventsViewModel>();

                // Views
                services.AddSingleton<MainWindow>();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await AppHost!.StartAsync();

        // Migrate DB automatically
        using (var scope = AppHost.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<WinFimDbContext>();
            db.Database.EnsureCreated(); // Simplification for MVP. Use proper migrations in prod.
        }

        var mainWindow = AppHost.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost!.StopAsync();
        base.OnExit(e);
    }
}

