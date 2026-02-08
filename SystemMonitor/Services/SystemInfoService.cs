using Microsoft.Extensions.Logging;
using SystemMonitor.Models;

namespace SystemMonitor.Services;

/// <summary>
/// Implementation of system information service
/// </summary>
public class SystemInfoService : ISystemInfoService
{
    private readonly ILogger<SystemInfoService> _logger;

    /// <summary>
    /// Initializes a new instance of the SystemInfoService class
    /// </summary>
    /// <param name="logger">Logger instance for logging errors and information</param>
    public SystemInfoService(ILogger<SystemInfoService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets system information asynchronously
    /// </summary>
    /// <returns>System information</returns>
    public async Task<SystemInfo> GetSystemInfoAsync()
    {
        try
        {
            return await Task.Run(() =>
            {
                var uptime = TimeSpan.FromMilliseconds(Environment.TickCount64);

                return new SystemInfo
                {
                    OperatingSystem = Environment.OSVersion.VersionString,
                    ComputerName = Environment.MachineName,
                    ProcessorCount = Environment.ProcessorCount,
                    Uptime = uptime
                };
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting system information");
            return new SystemInfo();
        }
    }
}
