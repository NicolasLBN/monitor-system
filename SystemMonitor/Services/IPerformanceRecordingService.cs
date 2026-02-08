namespace SystemMonitor.Services;

/// <summary>
/// Service for recording performance data
/// </summary>
public interface IPerformanceRecordingService
{
    /// <summary>
    /// Starts recording performance data
    /// </summary>
    /// <returns>Path to the recording data file</returns>
    Task<string> StartRecordingAsync();

    /// <summary>
    /// Records current performance data point
    /// </summary>
    /// <param name="cpuUsage">CPU usage percentage</param>
    /// <param name="ramUsage">RAM usage percentage</param>
    /// <param name="downloadSpeed">Network download speed string</param>
    /// <param name="uploadSpeed">Network upload speed string</param>
    Task RecordDataPointAsync(double cpuUsage, double ramUsage, string downloadSpeed, string uploadSpeed);

    /// <summary>
    /// Stops recording and returns the data file path
    /// </summary>
    /// <returns>Path to the recorded data file</returns>
    Task<string> StopRecordingAsync();

    /// <summary>
    /// Gets whether recording is currently active
    /// </summary>
    bool IsRecording { get; }
}
