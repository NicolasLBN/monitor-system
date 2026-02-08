using SystemMonitor.Models;

namespace SystemMonitor.Services;

/// <summary>
/// Service for monitoring network speed
/// </summary>
public interface INetworkMonitorService
{
    /// <summary>
    /// Gets the current network speed data asynchronously
    /// </summary>
    /// <returns>Network speed data</returns>
    Task<NetworkSpeedData> GetNetworkSpeedAsync();
}
