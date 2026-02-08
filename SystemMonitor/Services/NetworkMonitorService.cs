using Microsoft.Extensions.Logging;
using SystemMonitor.Models;
using System.Net.NetworkInformation;

namespace SystemMonitor.Services;

/// <summary>
/// Implementation of network monitoring service
/// </summary>
public class NetworkMonitorService : INetworkMonitorService
{
    private readonly ILogger<NetworkMonitorService> _logger;
    private long _previousBytesReceived = 0;
    private long _previousBytesSent = 0;
    private DateTime _lastCheck = DateTime.Now;

    /// <summary>
    /// Initializes a new instance of the NetworkMonitorService class
    /// </summary>
    /// <param name="logger">Logger instance for logging errors and information</param>
    public NetworkMonitorService(ILogger<NetworkMonitorService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets the current network speed data asynchronously
    /// </summary>
    /// <returns>Network speed data</returns>
    public async Task<NetworkSpeedData> GetNetworkSpeedAsync()
    {
        try
        {
            return await Task.Run(() =>
            {
                long totalBytesReceived = 0;
                long totalBytesSent = 0;

                var interfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (var ni in interfaces)
                {
                    if (ni.OperationalStatus == OperationalStatus.Up)
                    {
                        var stats = ni.GetIPv4Statistics();
                        totalBytesReceived += stats.BytesReceived;
                        totalBytesSent += stats.BytesSent;
                    }
                }

                var now = DateTime.Now;
                var timeDiff = (now - _lastCheck).TotalSeconds;

                double downloadSpeed = 0;
                double uploadSpeed = 0;

                if (timeDiff > 0 && _previousBytesReceived > 0)
                {
                    downloadSpeed = (totalBytesReceived - _previousBytesReceived) / timeDiff / 1024; // KB/s
                    uploadSpeed = (totalBytesSent - _previousBytesSent) / timeDiff / 1024; // KB/s
                }

                _previousBytesReceived = totalBytesReceived;
                _previousBytesSent = totalBytesSent;
                _lastCheck = now;

                return new NetworkSpeedData
                {
                    DownloadKbps = downloadSpeed,
                    UploadKbps = uploadSpeed,
                    Timestamp = now
                };
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting network speed");
            return new NetworkSpeedData { DownloadKbps = 0, UploadKbps = 0, Timestamp = DateTime.Now };
        }
    }
}
