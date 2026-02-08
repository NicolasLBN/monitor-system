namespace SystemMonitor.Models;

/// <summary>
/// Represents CPU usage information
/// </summary>
public class CpuUsageData
{
    /// <summary>
    /// CPU usage percentage (0-100)
    /// </summary>
    public double UsagePercent { get; set; }

    /// <summary>
    /// Timestamp when the data was collected
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.Now;
}

/// <summary>
/// Represents RAM usage information
/// </summary>
public class RamUsageData
{
    /// <summary>
    /// RAM usage percentage (0-100)
    /// </summary>
    public double UsagePercent { get; set; }

    /// <summary>
    /// Used memory in bytes
    /// </summary>
    public long UsedBytes { get; set; }

    /// <summary>
    /// Total memory in bytes
    /// </summary>
    public long TotalBytes { get; set; }

    /// <summary>
    /// Timestamp when the data was collected
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.Now;

    /// <summary>
    /// Gets used memory in gigabytes
    /// </summary>
    public double UsedGB => UsedBytes / (1024.0 * 1024 * 1024);

    /// <summary>
    /// Gets total memory in gigabytes
    /// </summary>
    public double TotalGB => TotalBytes / (1024.0 * 1024 * 1024);
}

/// <summary>
/// Represents GPU usage information
/// </summary>
public class GpuUsageData
{
    /// <summary>
    /// GPU usage percentage (0-100)
    /// </summary>
    public double UsagePercent { get; set; }

    /// <summary>
    /// Timestamp when the data was collected
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.Now;
}

/// <summary>
/// Represents disk drive information
/// </summary>
public class DiskInfo
{
    /// <summary>
    /// Drive name (e.g., "C:", "D:")
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Total size in bytes
    /// </summary>
    public long TotalBytes { get; set; }

    /// <summary>
    /// Available free space in bytes
    /// </summary>
    public long FreeBytes { get; set; }

    /// <summary>
    /// Gets used space in bytes
    /// </summary>
    public long UsedBytes => TotalBytes - FreeBytes;

    /// <summary>
    /// Gets used space in gigabytes
    /// </summary>
    public double UsedGB => UsedBytes / (1024.0 * 1024 * 1024);

    /// <summary>
    /// Gets free space in gigabytes
    /// </summary>
    public double FreeGB => FreeBytes / (1024.0 * 1024 * 1024);

    /// <summary>
    /// Gets total space in gigabytes
    /// </summary>
    public double TotalGB => TotalBytes / (1024.0 * 1024 * 1024);
}

/// <summary>
/// Represents network speed information
/// </summary>
public class NetworkSpeedData
{
    /// <summary>
    /// Download speed in KB/s
    /// </summary>
    public double DownloadKbps { get; set; }

    /// <summary>
    /// Upload speed in KB/s
    /// </summary>
    public double UploadKbps { get; set; }

    /// <summary>
    /// Timestamp when the data was collected
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.Now;

    /// <summary>
    /// Gets formatted download speed string
    /// </summary>
    public string FormattedDownloadSpeed => FormatSpeed(DownloadKbps);

    /// <summary>
    /// Gets formatted upload speed string
    /// </summary>
    public string FormattedUploadSpeed => FormatSpeed(UploadKbps);

    private static string FormatSpeed(double kbps)
    {
        if (kbps < 1024)
            return $"{kbps:F2} KB/s";
        else
            return $"{kbps / 1024:F2} MB/s";
    }
}

/// <summary>
/// Represents system information
/// </summary>
public class SystemInfo
{
    /// <summary>
    /// Operating system version string
    /// </summary>
    public string OperatingSystem { get; set; } = string.Empty;

    /// <summary>
    /// Computer name
    /// </summary>
    public string ComputerName { get; set; } = string.Empty;

    /// <summary>
    /// Number of processor cores
    /// </summary>
    public int ProcessorCount { get; set; }

    /// <summary>
    /// System uptime
    /// </summary>
    public TimeSpan Uptime { get; set; }

    /// <summary>
    /// Gets formatted uptime string
    /// </summary>
    public string FormattedUptime => $"{Uptime.Days}d {Uptime.Hours}h {Uptime.Minutes}m";
}
