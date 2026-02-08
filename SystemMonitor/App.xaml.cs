using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SystemMonitor.Services;
using SystemMonitor.ViewModels;

namespace SystemMonitor;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    /// <summary>
    /// Gets the service provider for dependency injection
    /// </summary>
    public IServiceProvider ServiceProvider => _serviceProvider!;

    /// <summary>
    /// Called when the application starts
    /// </summary>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Configure dependency injection
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();

        // Create and show main window
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    /// <summary>
    /// Configures all services for dependency injection
    /// </summary>
    private void ConfigureServices(IServiceCollection services)
    {
        // Configure logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // Register services as singletons (one instance for the application lifetime)
        services.AddSingleton<ICpuMonitorService, CpuMonitorService>();
        services.AddSingleton<IRamMonitorService, RamMonitorService>();
        services.AddSingleton<IGpuMonitorService, GpuMonitorService>();
        services.AddSingleton<IDiskMonitorService, DiskMonitorService>();
        services.AddSingleton<INetworkMonitorService, NetworkMonitorService>();
        services.AddSingleton<ISystemInfoService, SystemInfoService>();
        services.AddSingleton<IPerformanceRecordingService, PerformanceRecordingService>();
        services.AddSingleton<IReportGenerationService, ReportGenerationService>();

        // Register ViewModels
        services.AddSingleton<MainWindowViewModel>();

        // Register MainWindow
        services.AddSingleton<MainWindow>();
    }

    /// <summary>
    /// Called when the application exits
    /// </summary>
    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}

