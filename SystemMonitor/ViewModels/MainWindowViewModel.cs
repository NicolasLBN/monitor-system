using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Microsoft.Extensions.Logging;
using SystemMonitor.Commands;
using SystemMonitor.Models;
using SystemMonitor.Services;

namespace SystemMonitor.ViewModels;

/// <summary>
/// ViewModel for the main window, orchestrating all monitoring services
/// </summary>
public class MainWindowViewModel : ViewModelBase, IDisposable
{
    private const int MaxHistoryPoints = 30;
    private const int DiskUpdateIntervalSeconds = 60;

    private readonly ICpuMonitorService _cpuMonitorService;
    private readonly IRamMonitorService _ramMonitorService;
    private readonly IGpuMonitorService _gpuMonitorService;
    private readonly IDiskMonitorService _diskMonitorService;
    private readonly INetworkMonitorService _networkMonitorService;
    private readonly ISystemInfoService _systemInfoService;
    private readonly IPerformanceRecordingService _performanceRecordingService;
    private readonly IReportGenerationService _reportGenerationService;
    private readonly ILogger<MainWindowViewModel> _logger;

    private readonly DispatcherTimer _updateTimer;
    private DateTime _lastDiskUpdate = DateTime.MinValue;
    private readonly List<double> _ramHistory = new();
    private readonly List<string> _ramLabels = new();
    private readonly List<double> _gpuHistory = new();
    
    private bool _disposed;

    #region Properties

    private double _cpuUsage;
    /// <summary>
    /// Current CPU usage percentage
    /// </summary>
    public double CpuUsage
    {
        get => _cpuUsage;
        set => SetProperty(ref _cpuUsage, value);
    }

    private string _cpuUsageText = "0%";
    /// <summary>
    /// Formatted CPU usage text
    /// </summary>
    public string CpuUsageText
    {
        get => _cpuUsageText;
        set => SetProperty(ref _cpuUsageText, value);
    }

    private string _ramUsedText = "Used: 0 GB";
    /// <summary>
    /// RAM used text
    /// </summary>
    public string RamUsedText
    {
        get => _ramUsedText;
        set => SetProperty(ref _ramUsedText, value);
    }

    private string _ramTotalText = "Total: 0 GB";
    /// <summary>
    /// RAM total text
    /// </summary>
    public string RamTotalText
    {
        get => _ramTotalText;
        set => SetProperty(ref _ramTotalText, value);
    }

    private string _networkDownloadText = "Download: 0 KB/s";
    /// <summary>
    /// Network download speed text
    /// </summary>
    public string NetworkDownloadText
    {
        get => _networkDownloadText;
        set => SetProperty(ref _networkDownloadText, value);
    }

    private string _networkUploadText = "Upload: 0 KB/s";
    /// <summary>
    /// Network upload speed text
    /// </summary>
    public string NetworkUploadText
    {
        get => _networkUploadText;
        set => SetProperty(ref _networkUploadText, value);
    }

    private string _osText = "OS: Loading...";
    /// <summary>
    /// Operating system text
    /// </summary>
    public string OsText
    {
        get => _osText;
        set => SetProperty(ref _osText, value);
    }

    private string _computerNameText = "Computer: Loading...";
    /// <summary>
    /// Computer name text
    /// </summary>
    public string ComputerNameText
    {
        get => _computerNameText;
        set => SetProperty(ref _computerNameText, value);
    }

    private string _processorText = "Processor: Loading...";
    /// <summary>
    /// Processor info text
    /// </summary>
    public string ProcessorText
    {
        get => _processorText;
        set => SetProperty(ref _processorText, value);
    }

    private string _uptimeText = "Uptime: Loading...";
    /// <summary>
    /// System uptime text
    /// </summary>
    public string UptimeText
    {
        get => _uptimeText;
        set => SetProperty(ref _uptimeText, value);
    }

    private string _diskInfoText = "Disks: 0";
    /// <summary>
    /// Disk information text
    /// </summary>
    public string DiskInfoText
    {
        get => _diskInfoText;
        set => SetProperty(ref _diskInfoText, value);
    }

    private string _recordingStatusText = "Ready to record";
    /// <summary>
    /// Recording status text
    /// </summary>
    public string RecordingStatusText
    {
        get => _recordingStatusText;
        set => SetProperty(ref _recordingStatusText, value);
    }

    private Brush _recordingStatusBrush = new SolidColorBrush(Color.FromRgb(128, 128, 128));
    /// <summary>
    /// Recording status text brush
    /// </summary>
    public Brush RecordingStatusBrush
    {
        get => _recordingStatusBrush;
        set => SetProperty(ref _recordingStatusBrush, value);
    }

    private bool _isStartRecordEnabled = true;
    /// <summary>
    /// Whether start recording button is enabled
    /// </summary>
    public bool IsStartRecordEnabled
    {
        get => _isStartRecordEnabled;
        set => SetProperty(ref _isStartRecordEnabled, value);
    }

    private bool _isStopRecordEnabled = false;
    /// <summary>
    /// Whether stop recording button is enabled
    /// </summary>
    public bool IsStopRecordEnabled
    {
        get => _isStopRecordEnabled;
        set => SetProperty(ref _isStopRecordEnabled, value);
    }

    /// <summary>
    /// RAM usage chart series
    /// </summary>
    public SeriesCollection RamSeries { get; set; } = null!;

    /// <summary>
    /// RAM chart labels
    /// </summary>
    public ObservableCollection<string> RamLabels { get; set; } = null!;

    /// <summary>
    /// Disk usage chart series
    /// </summary>
    public SeriesCollection DiskSeries { get; set; } = null!;

    /// <summary>
    /// Disk chart labels
    /// </summary>
    public ObservableCollection<string> DiskLabels { get; set; } = null!;

    /// <summary>
    /// GPU usage chart series
    /// </summary>
    public SeriesCollection GpuSeries { get; set; } = null!;

    /// <summary>
    /// GPU chart labels
    /// </summary>
    public ObservableCollection<string> GpuLabels { get; set; } = null!;

    #endregion

    #region Commands

    /// <summary>
    /// Command to start recording performance data
    /// </summary>
    public ICommand StartRecordCommand { get; }

    /// <summary>
    /// Command to stop recording and generate report
    /// </summary>
    public ICommand StopRecordCommand { get; }

    #endregion

    /// <summary>
    /// Initializes a new instance of the MainWindowViewModel class
    /// </summary>
    public MainWindowViewModel(
        ICpuMonitorService cpuMonitorService,
        IRamMonitorService ramMonitorService,
        IGpuMonitorService gpuMonitorService,
        IDiskMonitorService diskMonitorService,
        INetworkMonitorService networkMonitorService,
        ISystemInfoService systemInfoService,
        IPerformanceRecordingService performanceRecordingService,
        IReportGenerationService reportGenerationService,
        ILogger<MainWindowViewModel> logger)
    {
        _cpuMonitorService = cpuMonitorService ?? throw new ArgumentNullException(nameof(cpuMonitorService));
        _ramMonitorService = ramMonitorService ?? throw new ArgumentNullException(nameof(ramMonitorService));
        _gpuMonitorService = gpuMonitorService ?? throw new ArgumentNullException(nameof(gpuMonitorService));
        _diskMonitorService = diskMonitorService ?? throw new ArgumentNullException(nameof(diskMonitorService));
        _networkMonitorService = networkMonitorService ?? throw new ArgumentNullException(nameof(networkMonitorService));
        _systemInfoService = systemInfoService ?? throw new ArgumentNullException(nameof(systemInfoService));
        _performanceRecordingService = performanceRecordingService ?? throw new ArgumentNullException(nameof(performanceRecordingService));
        _reportGenerationService = reportGenerationService ?? throw new ArgumentNullException(nameof(reportGenerationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Initialize charts
        InitializeCharts();

        // Initialize commands
        StartRecordCommand = new RelayCommand(async _ => await StartRecordingAsync(), _ => IsStartRecordEnabled);
        StopRecordCommand = new RelayCommand(async _ => await StopRecordingAsync(), _ => IsStopRecordEnabled);

        // Initialize system info
        Task.Run(async () =>
        {
            try
            {
                await InitializeSystemInfoAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in InitializeSystemInfoAsync");
            }
        });

        // Start update timer
        _updateTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _updateTimer.Tick += async (s, e) => await UpdateTimerTickAsync();
        _updateTimer.Start();

        _logger.LogInformation("MainWindowViewModel initialized successfully");
    }

    private void InitializeCharts()
    {
        // Initialize RAM chart
        RamSeries = new SeriesCollection
        {
            new LineSeries
            {
                Title = "RAM %",
                Values = new ChartValues<double>(),
                Fill = new SolidColorBrush(Color.FromArgb(50, 206, 145, 120)),
                Stroke = new SolidColorBrush(Color.FromRgb(206, 145, 120)),
                StrokeThickness = 2,
                PointGeometry = null
            }
        };
        RamLabels = new ObservableCollection<string>();

        // Initialize Disk chart
        DiskSeries = new SeriesCollection
        {
            new ColumnSeries
            {
                Title = "Used",
                Values = new ChartValues<double>(),
                Fill = new SolidColorBrush(Color.FromRgb(197, 134, 192))
            },
            new ColumnSeries
            {
                Title = "Free",
                Values = new ChartValues<double>(),
                Fill = new SolidColorBrush(Color.FromRgb(100, 100, 100))
            }
        };
        DiskLabels = new ObservableCollection<string>();

        // Initialize GPU chart
        GpuSeries = new SeriesCollection
        {
            new LineSeries
            {
                Title = "GPU %",
                Values = new ChartValues<double>(),
                Fill = new SolidColorBrush(Color.FromArgb(50, 106, 174, 213)),
                Stroke = new SolidColorBrush(Color.FromRgb(106, 174, 213)),
                StrokeThickness = 2,
                PointGeometry = null
            }
        };
        GpuLabels = new ObservableCollection<string>();
    }

    private async Task InitializeSystemInfoAsync()
    {
        try
        {
            var systemInfo = await _systemInfoService.GetSystemInfoAsync();
            OsText = $"OS: {systemInfo.OperatingSystem}";
            ComputerNameText = $"Computer: {systemInfo.ComputerName}";
            ProcessorText = $"Processor: {systemInfo.ProcessorCount} cores";

            await UpdateDiskInfoAsync();
            _lastDiskUpdate = DateTime.Now;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing system info");
        }
    }

    private async Task UpdateTimerTickAsync()
    {
        try
        {
            // Update all metrics
            await UpdateCpuUsageAsync();
            await UpdateRamUsageAsync();
            await UpdateGpuUsageAsync();
            await UpdateNetworkSpeedAsync();
            await UpdateUptimeAsync();

            // Update disk info every minute
            var now = DateTime.Now;
            if ((now - _lastDiskUpdate).TotalSeconds >= DiskUpdateIntervalSeconds)
            {
                await UpdateDiskInfoAsync();
                _lastDiskUpdate = now;
            }

            // Record data if recording is active
            if (_performanceRecordingService.IsRecording)
            {
                await RecordPerformanceDataAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in update timer tick");
        }
    }

    private async Task UpdateCpuUsageAsync()
    {
        try
        {
            var cpuData = await _cpuMonitorService.GetCpuUsageAsync();
            CpuUsage = cpuData.UsagePercent;
            CpuUsageText = $"{cpuData.UsagePercent:F2}%";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating CPU usage");
        }
    }

    private async Task UpdateRamUsageAsync()
    {
        try
        {
            var ramData = await _ramMonitorService.GetRamUsageAsync();

            // Update chart
            _ramHistory.Add(ramData.UsagePercent);
            var ramLabel = DateTime.Now.ToString("HH:mm:ss");
            _ramLabels.Add(ramLabel);

            var series = RamSeries[0] as LineSeries;
            if (series != null)
            {
                series.Values.Add(ramData.UsagePercent);

                if (series.Values.Count > MaxHistoryPoints)
                {
                    series.Values.RemoveAt(0);
                    _ramHistory.RemoveAt(0);
                }
            }

            if (_ramLabels.Count > MaxHistoryPoints)
            {
                _ramLabels.RemoveAt(0);
            }

            RamLabels.Clear();
            foreach (var label in _ramLabels)
            {
                RamLabels.Add(label);
            }

            RamUsedText = $"Used: {ramData.UsedGB:F2} GB";
            RamTotalText = $"Total: {ramData.TotalGB:F2} GB";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating RAM usage");
        }
    }

    private async Task UpdateGpuUsageAsync()
    {
        try
        {
            var gpuData = await _gpuMonitorService.GetGpuUsageAsync();

            // Update chart
            _gpuHistory.Add(gpuData.UsagePercent);

            var series = GpuSeries[0] as LineSeries;
            if (series != null)
            {
                series.Values.Add(gpuData.UsagePercent);

                if (series.Values.Count > MaxHistoryPoints)
                {
                    series.Values.RemoveAt(0);
                    _gpuHistory.RemoveAt(0);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating GPU usage");
        }
    }

    private async Task UpdateDiskInfoAsync()
    {
        try
        {
            var disks = await _diskMonitorService.GetDiskInfoAsync();
            var diskList = disks.ToList();

            var usedValues = new ChartValues<double>();
            var freeValues = new ChartValues<double>();
            var labels = new List<string>();

            foreach (var disk in diskList)
            {
                usedValues.Add(disk.UsedGB);
                freeValues.Add(disk.FreeGB);
                labels.Add(disk.Name);
            }

            (DiskSeries[0] as ColumnSeries)!.Values = usedValues;
            (DiskSeries[1] as ColumnSeries)!.Values = freeValues;
            
            DiskLabels.Clear();
            foreach (var label in labels)
            {
                DiskLabels.Add(label);
            }

            DiskInfoText = $"Disks: {diskList.Count} | Total: {diskList.Sum(d => d.TotalGB):F0} GB";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating disk info");
        }
    }

    private async Task UpdateNetworkSpeedAsync()
    {
        try
        {
            var networkData = await _networkMonitorService.GetNetworkSpeedAsync();
            NetworkDownloadText = $"Download: {networkData.FormattedDownloadSpeed}";
            NetworkUploadText = $"Upload: {networkData.FormattedUploadSpeed}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating network speed");
        }
    }

    private async Task UpdateUptimeAsync()
    {
        try
        {
            var systemInfo = await _systemInfoService.GetSystemInfoAsync();
            UptimeText = $"Uptime: {systemInfo.FormattedUptime}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating uptime");
        }
    }

    private async Task StartRecordingAsync()
    {
        try
        {
            var filePath = await _performanceRecordingService.StartRecordingAsync();

            IsStartRecordEnabled = false;
            IsStopRecordEnabled = true;
            RecordingStatusText = "Recording...";
            RecordingStatusBrush = new SolidColorBrush(Color.FromRgb(78, 201, 176));

            MessageBox.Show($"Recording started. Data will be saved to:\n{filePath}",
                          "Recording Started", MessageBoxButton.OK, MessageBoxImage.Information);

            _logger.LogInformation("Recording started");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting recording");
            MessageBox.Show($"Error starting recording: {ex.Message}",
                          "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task StopRecordingAsync()
    {
        try
        {
            IsStartRecordEnabled = true;
            IsStopRecordEnabled = false;
            RecordingStatusText = "Generating PDF...";

            var dataFilePath = await _performanceRecordingService.StopRecordingAsync();
            var outputPdf = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                                        $"SystemMonitor_Report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

            var success = await _reportGenerationService.GenerateReportAsync(dataFilePath, outputPdf);

            if (success)
            {
                RecordingStatusText = "PDF generated successfully!";
                RecordingStatusBrush = new SolidColorBrush(Color.FromRgb(78, 201, 176));

                var result = MessageBox.Show($"PDF report generated successfully!\n\nLocation: {outputPdf}\n\nWould you like to open it?",
                              "Success", MessageBoxButton.YesNo, MessageBoxImage.Information);

                if (result == MessageBoxResult.Yes)
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(outputPdf) { UseShellExecute = true });
                }
            }
            else
            {
                RecordingStatusText = "PDF generation failed";
                RecordingStatusBrush = new SolidColorBrush(Color.FromRgb(244, 135, 113));
                MessageBox.Show("PDF generation failed. Check logs for details.",
                              "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            await Task.Delay(3000);
            RecordingStatusText = "Ready to record";
            RecordingStatusBrush = new SolidColorBrush(Color.FromRgb(128, 128, 128));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping recording");
            MessageBox.Show($"Error stopping recording: {ex.Message}",
                          "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            RecordingStatusText = "Ready to record";
            RecordingStatusBrush = new SolidColorBrush(Color.FromRgb(128, 128, 128));
        }
    }

    private async Task RecordPerformanceDataAsync()
    {
        try
        {
            var cpu = CpuUsage;
            var ram = _ramHistory.Count > 0 ? _ramHistory[^1] : 0;
            var networkDown = NetworkDownloadText.Replace("Download: ", "");
            var networkUp = NetworkUploadText.Replace("Upload: ", "");

            await _performanceRecordingService.RecordDataPointAsync(cpu, ram, networkDown, networkUp);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording performance data");
        }
    }

    /// <summary>
    /// Disposes the ViewModel and all services
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _updateTimer?.Stop();
            _cpuMonitorService?.Dispose();
            _ramMonitorService?.Dispose();
            _gpuMonitorService?.Dispose();
            _disposed = true;
            _logger.LogInformation("MainWindowViewModel disposed");
        }
    }
}
