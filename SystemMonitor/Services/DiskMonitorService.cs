using Microsoft.Extensions.Logging;
using SystemMonitor.Models;
using System.IO;

namespace SystemMonitor.Services;

/// <summary>
/// Implementation of disk monitoring service
/// </summary>
public class DiskMonitorService : IDiskMonitorService
{
    private readonly ILogger<DiskMonitorService> _logger;

    /// <summary>
    /// Initializes a new instance of the DiskMonitorService class
    /// </summary>
    /// <param name="logger">Logger instance for logging errors and information</param>
    public DiskMonitorService(ILogger<DiskMonitorService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets disk information for all available drives asynchronously
    /// </summary>
    /// <returns>Collection of disk information</returns>
    public async Task<IEnumerable<DiskInfo>> GetDiskInfoAsync()
    {
        try
        {
            return await Task.Run(() =>
            {
                var drives = DriveInfo.GetDrives().Where(d => d.IsReady).ToList();
                var diskInfos = new List<DiskInfo>();

                foreach (var drive in drives)
                {
                    try
                    {
                        diskInfos.Add(new DiskInfo
                        {
                            Name = drive.Name.TrimEnd('\\'),
                            TotalBytes = drive.TotalSize,
                            FreeBytes = drive.AvailableFreeSpace
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error reading disk info for drive {drive.Name}");
                    }
                }

                return diskInfos;
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting disk information");
            return Enumerable.Empty<DiskInfo>();
        }
    }
}
