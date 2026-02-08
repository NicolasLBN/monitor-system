using System.Diagnostics;
using Microsoft.Extensions.Logging;
using SystemMonitor.Models;

namespace SystemMonitor.Services;

/// <summary>
/// Implementation of RAM monitoring service using performance counters
/// </summary>
public class RamMonitorService : IRamMonitorService
{
    private readonly ILogger<RamMonitorService> _logger;
    private PerformanceCounter? _ramCounter;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the RamMonitorService class
    /// </summary>
    /// <param name="logger">Logger instance for logging errors and information</param>
    public RamMonitorService(ILogger<RamMonitorService> logger)
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
                _ramCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
                _ramCounter.NextValue(); // Initial read
                _logger.LogInformation("RAM performance counter initialized successfully");
            }
            else
            {
                _logger.LogWarning("RAM performance counters not available on non-Windows systems. Using fallback method.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing RAM performance counter");
        }
    }

    /// <summary>
    /// Gets the current RAM usage data asynchronously
    /// </summary>
    /// <returns>RAM usage data</returns>
    public async Task<RamUsageData> GetRamUsageAsync()
    {
        try
        {
            double ramPercent = 0;
            long totalMemory = 0;
            long usedMemory = 0;

            if (_ramCounter != null && OperatingSystem.IsWindows())
            {
                ramPercent = await Task.Run(() => _ramCounter.NextValue());
            }
            else
            {
                // Fallback estimation
                var process = Process.GetCurrentProcess();
                usedMemory = process.WorkingSet64;
                totalMemory = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
                ramPercent = totalMemory > 0 ? (double)usedMemory / totalMemory * 100 : 0;
            }

            // Get actual memory info
            var gcInfo = GC.GetGCMemoryInfo();
            totalMemory = gcInfo.TotalAvailableMemoryBytes;
            usedMemory = GC.GetTotalMemory(false);

            if (OperatingSystem.IsWindows() && _ramCounter != null)
            {
                usedMemory = (long)(totalMemory * ramPercent / 100);
            }

            return new RamUsageData
            {
                UsagePercent = ramPercent,
                UsedBytes = usedMemory,
                TotalBytes = totalMemory,
                Timestamp = DateTime.Now
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting RAM usage");
            return new RamUsageData { UsagePercent = 0, Timestamp = DateTime.Now };
        }
    }

    /// <summary>
    /// Disposes the RAM monitor service and releases performance counters
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _ramCounter?.Dispose();
            _disposed = true;
            _logger.LogInformation("RAM monitor service disposed");
        }
    }
}
