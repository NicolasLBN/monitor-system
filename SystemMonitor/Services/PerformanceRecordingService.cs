using Microsoft.Extensions.Logging;
using System.IO;

namespace SystemMonitor.Services;

/// <summary>
/// Implementation of performance recording service
/// </summary>
public class PerformanceRecordingService : IPerformanceRecordingService
{
    private readonly ILogger<PerformanceRecordingService> _logger;
    private string _recordingDataFile = string.Empty;
    private bool _isRecording;

    /// <summary>
    /// Initializes a new instance of the PerformanceRecordingService class
    /// </summary>
    /// <param name="logger">Logger instance for logging errors and information</param>
    public PerformanceRecordingService(ILogger<PerformanceRecordingService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets whether recording is currently active
    /// </summary>
    public bool IsRecording => _isRecording;

    /// <summary>
    /// Starts recording performance data
    /// </summary>
    /// <returns>Path to the recording data file</returns>
    public async Task<string> StartRecordingAsync()
    {
        try
        {
            _recordingDataFile = Path.Combine(Path.GetTempPath(), $"perf_data_{DateTime.Now:yyyyMMdd_HHmmss}.csv");

            // Create CSV file with headers
            await File.WriteAllTextAsync(_recordingDataFile, "Timestamp,CPU,RAM,Network Download,Network Upload\n");

            _isRecording = true;
            _logger.LogInformation($"Started recording to {_recordingDataFile}");

            return _recordingDataFile;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting performance recording");
            throw;
        }
    }

    /// <summary>
    /// Records current performance data point
    /// </summary>
    /// <param name="cpuUsage">CPU usage percentage</param>
    /// <param name="ramUsage">RAM usage percentage</param>
    /// <param name="downloadSpeed">Network download speed string</param>
    /// <param name="uploadSpeed">Network upload speed string</param>
    public async Task RecordDataPointAsync(double cpuUsage, double ramUsage, string downloadSpeed, string uploadSpeed)
    {
        try
        {
            if (!_isRecording || string.IsNullOrEmpty(_recordingDataFile))
                return;

            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var line = $"{timestamp},{cpuUsage:F2},{ramUsage:F2},{downloadSpeed},{uploadSpeed}\n";

            await File.AppendAllTextAsync(_recordingDataFile, line);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording data point");
        }
    }

    /// <summary>
    /// Stops recording and returns the data file path
    /// </summary>
    /// <returns>Path to the recorded data file</returns>
    public Task<string> StopRecordingAsync()
    {
        try
        {
            _isRecording = false;
            _logger.LogInformation($"Stopped recording to {_recordingDataFile}");

            var filePath = _recordingDataFile;
            _recordingDataFile = string.Empty;

            return Task.FromResult(filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping performance recording");
            throw;
        }
    }
}
