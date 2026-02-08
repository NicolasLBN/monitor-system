using SystemMonitor.Models;

namespace SystemMonitor.Services;

/// <summary>
/// Service for getting system information
/// </summary>
public interface ISystemInfoService
{
    /// <summary>
    /// Gets system information asynchronously
    /// </summary>
    /// <returns>System information</returns>
    Task<SystemInfo> GetSystemInfoAsync();
}
