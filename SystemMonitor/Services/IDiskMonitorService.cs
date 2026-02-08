using SystemMonitor.Models;

namespace SystemMonitor.Services;

/// <summary>
/// Service for monitoring disk usage
/// </summary>
public interface IDiskMonitorService
{
    /// <summary>
    /// Gets disk information for all available drives asynchronously
    /// </summary>
    /// <returns>Collection of disk information</returns>
    Task<IEnumerable<DiskInfo>> GetDiskInfoAsync();
}
