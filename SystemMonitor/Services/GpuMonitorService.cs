using System.Diagnostics;
using Microsoft.Extensions.Logging;
using SystemMonitor.Models;

namespace SystemMonitor.Services;

/// <summary>
/// Implementation of GPU monitoring service using performance counters
/// </summary>
public class GpuMonitorService : IGpuMonitorService
{
    private readonly ILogger<GpuMonitorService> _logger;
    private PerformanceCounter? _gpuCounter;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the GpuMonitorService class
    /// </summary>
    /// <param name="logger">Logger instance for logging errors and information</param>
    public GpuMonitorService(ILogger<GpuMonitorService> logger)
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
                // Try to initialize GPU counter - may not be available on all systems
                var category = new PerformanceCounterCategory("GPU Engine");
                var instanceNames = category.GetInstanceNames();
                
                if (instanceNames.Length > 0)
                {
                    // Try different known counter names for GPU utilization
                    string[] counterNames = { "Utilization Percentage", "Running Time", "GPU Usage" };
                    
                    foreach (var counterName in counterNames)
                    {
                        try
                        {
                            _gpuCounter = new PerformanceCounter("GPU Engine", counterName, instanceNames[0]);
                            _gpuCounter.NextValue(); // Test read
                            _logger.LogInformation($"GPU performance counter initialized successfully with counter: {counterName}");
                            break;
                        }
                        catch
                        {
                            _gpuCounter = null;
                        }
                    }
                }
                
                if (_gpuCounter == null)
                {
                    _logger.LogWarning("GPU performance counters not available on this system");
                }
            }
            else
            {
                _logger.LogWarning("GPU performance counters not available on non-Windows systems");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing GPU performance counter");
        }
    }

    /// <summary>
    /// Gets the current GPU usage data asynchronously
    /// </summary>
    /// <returns>GPU usage data</returns>
    public async Task<GpuUsageData> GetGpuUsageAsync()
    {
        try
        {
            double gpuUsage = 0;

            if (_gpuCounter != null && OperatingSystem.IsWindows())
            {
                gpuUsage = await Task.Run(() => _gpuCounter.NextValue());
                // Ensure value is in 0-100 range (some counters may return values in different scales)
                gpuUsage = Math.Max(0, Math.Min(100, gpuUsage));
            }

            return new GpuUsageData
            {
                UsagePercent = gpuUsage,
                Timestamp = DateTime.Now
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting GPU usage");
            return new GpuUsageData { UsagePercent = 0, Timestamp = DateTime.Now };
        }
    }

    /// <summary>
    /// Disposes the GPU monitor service and releases performance counters
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _gpuCounter?.Dispose();
            _disposed = true;
            _logger.LogInformation("GPU monitor service disposed");
        }
    }
}
