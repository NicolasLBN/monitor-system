using System.Diagnostics;
using Microsoft.Extensions.Logging;
using SystemMonitor.Models;

namespace SystemMonitor.Services;

/// <summary>
/// Implementation of CPU monitoring service using performance counters
/// </summary>
public class CpuMonitorService : ICpuMonitorService
{
    private readonly ILogger<CpuMonitorService> _logger;
    private PerformanceCounter? _cpuCounter;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the CpuMonitorService class
    /// </summary>
    /// <param name="logger">Logger instance for logging errors and information</param>
    public CpuMonitorService(ILogger<CpuMonitorService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        Initialize();
    }

    private void Initialize()
    {
        try
        {
            if (OperatingSystem.IsWindows())
            {
                _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                _cpuCounter.NextValue(); // Initial read to initialize counter
                _logger.LogInformation("CPU performance counter initialized successfully");
            }
            else
            {
                _logger.LogWarning("CPU performance counters not available on non-Windows systems. Using fallback method.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing CPU performance counter");
        }
    }

    /// <summary>
    /// Gets the current CPU usage data asynchronously
    /// </summary>
    /// <returns>CPU usage data</returns>
    public async Task<CpuUsageData> GetCpuUsageAsync()
    {
        try
        {
            double cpuUsage = 0;

            if (_cpuCounter != null && OperatingSystem.IsWindows())
            {
                // Run CPU counter on background thread to avoid blocking UI
                cpuUsage = await Task.Run(() => _cpuCounter.NextValue());
            }
            else
            {
                // Fallback for non-Windows: use Process CPU time
                cpuUsage = await Task.Run(() => GetProcessCpuUsage());
            }

            return new CpuUsageData
            {
                UsagePercent = cpuUsage,
                Timestamp = DateTime.Now
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting CPU usage");
            return new CpuUsageData { UsagePercent = 0, Timestamp = DateTime.Now };
        }
    }

    private double GetProcessCpuUsage()
    {
        try
        {
            var process = Process.GetCurrentProcess();
            return Math.Min(100, process.TotalProcessorTime.TotalMilliseconds /
                          (Environment.TickCount * Environment.ProcessorCount) * 100);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating process CPU usage");
            return 0;
        }
    }

    /// <summary>
    /// Disposes the CPU monitor service and releases performance counters
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _cpuCounter?.Dispose();
            _disposed = true;
            _logger.LogInformation("CPU monitor service disposed");
        }
    }
}
