using SystemMonitor.Models;

namespace SystemMonitor.Services;

/// <summary>
/// Service for monitoring GPU usage
/// </summary>
public interface IGpuMonitorService : IDisposable
{
    /// <summary>
    /// Gets the current GPU usage data asynchronously
    /// </summary>
    /// <returns>GPU usage data</returns>
    Task<GpuUsageData> GetGpuUsageAsync();
}
