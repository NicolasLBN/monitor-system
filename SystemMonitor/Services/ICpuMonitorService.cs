using SystemMonitor.Models;

namespace SystemMonitor.Services;

/// <summary>
/// Service for monitoring CPU usage
/// </summary>
public interface ICpuMonitorService : IDisposable
{
    /// <summary>
    /// Gets the current CPU usage data asynchronously
    /// </summary>
    /// <returns>CPU usage data</returns>
    Task<CpuUsageData> GetCpuUsageAsync();
}
