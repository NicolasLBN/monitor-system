using SystemMonitor.Models;

namespace SystemMonitor.Services;

/// <summary>
/// Service for monitoring RAM usage
/// </summary>
public interface IRamMonitorService : IDisposable
{
    /// <summary>
    /// Gets the current RAM usage data asynchronously
    /// </summary>
    /// <returns>RAM usage data</returns>
    Task<RamUsageData> GetRamUsageAsync();
}
