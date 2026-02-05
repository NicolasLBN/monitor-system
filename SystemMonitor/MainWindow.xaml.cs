using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Diagnostics;
using System.IO;
using System.Windows.Threading;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;

namespace SystemMonitor;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private DispatcherTimer _updateTimer = null!;
    private PerformanceCounter? _cpuCounter;
    private PerformanceCounter? _ramCounter;
    private PerformanceCounter? _gpuCounter;
    private List<double> _ramHistory = new List<double>();
    private List<string> _ramLabels = new List<string>();
    private List<double> _gpuHistory = new List<double>();
    private const int MaxHistoryPoints = 30;
    
    private long _previousBytesReceived = 0;
    private long _previousBytesSent = 0;
    private DateTime _lastNetworkCheck = DateTime.Now;
    
    private DateTime _lastDiskUpdate = DateTime.MinValue;
    private const int DiskUpdateIntervalSeconds = 60;
    
    private bool _isRecording = false;
    private string _recordingDataFile = "";
    
    public SeriesCollection RamSeries { get; set; } = null!;
    public List<string> RamLabels { get; set; } = null!;
    public SeriesCollection DiskSeries { get; set; } = null!;
    public List<string> DiskLabels { get; set; } = null!;
    public SeriesCollection GpuSeries { get; set; } = null!;
    public List<string> GpuLabels { get; set; } = null!;

    public MainWindow()
    {
        InitializeComponent();
        
        InitializeCharts();
        InitializePerformanceCounters();
        InitializeSystemInfo();
        
        _updateTimer = new DispatcherTimer();
        _updateTimer.Interval = TimeSpan.FromSeconds(1);
        _updateTimer.Tick += UpdateTimer_Tick;
        _updateTimer.Start();
        
        DataContext = this;
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
        
        RamLabels = new List<string>();
        
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
        
        DiskLabels = new List<string>();
        
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
        
        GpuLabels = new List<string>();
    }

    private void InitializePerformanceCounters()
    {
        try
        {
            if (OperatingSystem.IsWindows())
            {
                _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                _ramCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
                
                // Try to initialize GPU counter - may not be available on all systems
                try
                {
                    // Try to find GPU performance counters
                    // Common GPU counter categories: "GPU Engine", "GPU Adapter Memory", etc.
                    var category = new PerformanceCounterCategory("GPU Engine");
                    var instanceNames = category.GetInstanceNames();
                    if (instanceNames.Length > 0)
                    {
                        // Use the first GPU instance found
                        _gpuCounter = new PerformanceCounter("GPU Engine", "Utilization Percentage", instanceNames[0]);
                    }
                }
                catch
                {
                    // GPU counters not available - will use fallback
                    Debug.WriteLine("GPU performance counters not available");
                }
                
                // Initial read to initialize counters
                _cpuCounter.NextValue();
                _ramCounter.NextValue();
                if (_gpuCounter != null)
                {
                    _gpuCounter.NextValue();
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error initializing performance counters: {ex.Message}\nSome features may not work on non-Windows systems.", 
                          "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void InitializeSystemInfo()
    {
        try
        {
            OsText.Text = $"OS: {Environment.OSVersion.VersionString}";
            ComputerNameText.Text = $"Computer: {Environment.MachineName}";
            ProcessorText.Text = $"Processor: {Environment.ProcessorCount} cores";
            
            UpdateDiskInfo();
            _lastDiskUpdate = DateTime.Now;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading system info: {ex.Message}");
        }
    }

    private void UpdateTimer_Tick(object? sender, EventArgs e)
    {
        UpdateCpuUsage();
        UpdateRamUsage();
        UpdateGpuUsage();
        
        // Only update disk info every minute
        var now = DateTime.Now;
        if ((now - _lastDiskUpdate).TotalSeconds >= DiskUpdateIntervalSeconds)
        {
            UpdateDiskInfo();
            _lastDiskUpdate = now;
        }
        
        UpdateNetworkSpeed();
        UpdateUptime();
        
        if (_isRecording)
        {
            RecordPerformanceData();
        }
    }

    private void UpdateCpuUsage()
    {
        try
        {
            double cpuUsage = 0;
            
            if (_cpuCounter != null && OperatingSystem.IsWindows())
            {
                cpuUsage = _cpuCounter.NextValue();
            }
            else
            {
                // Fallback for non-Windows: use Process CPU time
                cpuUsage = GetProcessCpuUsage();
            }
            
            CpuGauge.Value = cpuUsage;
            CpuPercentText.Text = $"{cpuUsage:F2}%";
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating CPU: {ex.Message}");
        }
    }

    private double GetProcessCpuUsage()
    {
        // Simple CPU usage estimation based on process CPU time
        // This is a fallback for non-Windows systems
        try
        {
            var process = Process.GetCurrentProcess();
            return Math.Min(100, process.TotalProcessorTime.TotalMilliseconds / 
                          (Environment.TickCount * Environment.ProcessorCount) * 100);
        }
        catch
        {
            return 0;
        }
    }

    private void UpdateRamUsage()
    {
        try
        {
            double ramPercent = 0;
            long totalMemory = 0;
            long usedMemory = 0;
            
            if (_ramCounter != null && OperatingSystem.IsWindows())
            {
                ramPercent = _ramCounter.NextValue();
            }
            else
            {
                // Fallback estimation
                var process = Process.GetCurrentProcess();
                usedMemory = process.WorkingSet64;
                totalMemory = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
                ramPercent = totalMemory > 0 ? (double)usedMemory / totalMemory * 100 : 0;
            }
            
            // Get actual memory info
            var gcInfo = GC.GetGCMemoryInfo();
            totalMemory = gcInfo.TotalAvailableMemoryBytes;
            usedMemory = GC.GetTotalMemory(false);
            
            if (OperatingSystem.IsWindows() && _ramCounter != null)
            {
                usedMemory = (long)(totalMemory * ramPercent / 100);
            }
            
            // Update chart
            _ramHistory.Add(ramPercent);
            if (_ramHistory.Count > MaxHistoryPoints)
            {
                _ramHistory.RemoveAt(0);
                _ramLabels.RemoveAt(0);
            }
            
            _ramLabels.Add(DateTime.Now.ToString("HH:mm:ss"));
            
            var series = RamSeries[0] as LineSeries;
            if (series != null)
            {
                series.Values.Clear();
                foreach (var value in _ramHistory)
                {
                    series.Values.Add(value);
                }
            }
            
            RamUsedText.Text = $"Used: {usedMemory / (1024.0 * 1024 * 1024):F2} GB";
            RamTotalText.Text = $"Total: {totalMemory / (1024.0 * 1024 * 1024):F2} GB";
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating RAM: {ex.Message}");
        }
    }

    private void UpdateGpuUsage()
    {
        try
        {
            double gpuUsage = 0;
            
            if (_gpuCounter != null && OperatingSystem.IsWindows())
            {
                gpuUsage = _gpuCounter.NextValue();
            }
            else
            {
                // Fallback: GPU data not available
                gpuUsage = 0;
            }
            
            // Update chart
            _gpuHistory.Add(gpuUsage);
            if (_gpuHistory.Count > MaxHistoryPoints)
            {
                _gpuHistory.RemoveAt(0);
            }
            
            var series = GpuSeries[0] as LineSeries;
            if (series != null)
            {
                series.Values.Clear();
                foreach (var value in _gpuHistory)
                {
                    series.Values.Add(value);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating GPU: {ex.Message}");
        }
    }

    private void UpdateDiskInfo()
    {
        try
        {
            var drives = DriveInfo.GetDrives().Where(d => d.IsReady).ToArray();
            
            var usedValues = new ChartValues<double>();
            var freeValues = new ChartValues<double>();
            var labels = new List<string>();
            
            foreach (var drive in drives)
            {
                double usedGB = (drive.TotalSize - drive.AvailableFreeSpace) / (1024.0 * 1024 * 1024);
                double freeGB = drive.AvailableFreeSpace / (1024.0 * 1024 * 1024);
                
                usedValues.Add(usedGB);
                freeValues.Add(freeGB);
                labels.Add(drive.Name.TrimEnd('\\'));
            }
            
            (DiskSeries[0] as ColumnSeries)!.Values = usedValues;
            (DiskSeries[1] as ColumnSeries)!.Values = freeValues;
            DiskLabels.Clear();
            DiskLabels.AddRange(labels);
            
            DiskInfoText.Text = $"Disks: {drives.Length} | Total: {drives.Sum(d => d.TotalSize) / (1024.0 * 1024 * 1024):F0} GB";
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating disk info: {ex.Message}");
        }
    }

    private void UpdateNetworkSpeed()
    {
        try
        {
            long totalBytesReceived = 0;
            long totalBytesSent = 0;
            
            var interfaces = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
            foreach (var ni in interfaces)
            {
                if (ni.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up)
                {
                    var stats = ni.GetIPv4Statistics();
                    totalBytesReceived += stats.BytesReceived;
                    totalBytesSent += stats.BytesSent;
                }
            }
            
            var now = DateTime.Now;
            var timeDiff = (now - _lastNetworkCheck).TotalSeconds;
            
            if (timeDiff > 0 && _previousBytesReceived > 0)
            {
                double downloadSpeed = (totalBytesReceived - _previousBytesReceived) / timeDiff / 1024; // KB/s
                double uploadSpeed = (totalBytesSent - _previousBytesSent) / timeDiff / 1024; // KB/s
                
                NetworkDownloadText.Text = $"Download: {FormatSpeed(downloadSpeed)}";
                NetworkUploadText.Text = $"Upload: {FormatSpeed(uploadSpeed)}";
            }
            
            _previousBytesReceived = totalBytesReceived;
            _previousBytesSent = totalBytesSent;
            _lastNetworkCheck = now;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating network: {ex.Message}");
        }
    }

    private string FormatSpeed(double kbps)
    {
        if (kbps < 1024)
            return $"{kbps:F2} KB/s";
        else
            return $"{kbps / 1024:F2} MB/s";
    }

    private void UpdateUptime()
    {
        try
        {
            var uptime = TimeSpan.FromMilliseconds(Environment.TickCount64);
            UptimeText.Text = $"Uptime: {uptime.Days}d {uptime.Hours}h {uptime.Minutes}m";
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating uptime: {ex.Message}");
        }
    }

    private void StartRecordButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _isRecording = true;
            _recordingDataFile = Path.Combine(Path.GetTempPath(), $"perf_data_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            
            // Create CSV file with headers
            File.WriteAllText(_recordingDataFile, "Timestamp,CPU,RAM,Network Download,Network Upload\n");
            
            StartRecordButton.IsEnabled = false;
            StopRecordButton.IsEnabled = true;
            RecordingStatusText.Text = "Recording...";
            RecordingStatusText.Foreground = new SolidColorBrush(Color.FromRgb(78, 201, 176));
            
            MessageBox.Show($"Recording started. Data will be saved to:\n{_recordingDataFile}", 
                          "Recording Started", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error starting recording: {ex.Message}", 
                          "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            _isRecording = false;
        }
    }

    private void StopRecordButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _isRecording = false;
            StartRecordButton.IsEnabled = true;
            StopRecordButton.IsEnabled = false;
            RecordingStatusText.Text = "Generating PDF...";
            
            // Call Python script to generate PDF
            GeneratePdfReport();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error stopping recording: {ex.Message}", 
                          "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            RecordingStatusText.Text = "Ready to record";
            RecordingStatusText.Foreground = new SolidColorBrush(Color.FromRgb(128, 128, 128));
        }
    }

    private void RecordPerformanceData()
    {
        try
        {
            if (string.IsNullOrEmpty(_recordingDataFile))
                return;
                
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var cpu = CpuGauge.Value;
            var ram = _ramHistory.Count > 0 ? _ramHistory[_ramHistory.Count - 1] : 0;
            var networkDown = NetworkDownloadText.Text.Replace("Download: ", "");
            var networkUp = NetworkUploadText.Text.Replace("Upload: ", "");
            
            var line = $"{timestamp},{cpu:F2},{ram:F2},{networkDown},{networkUp}\n";
            File.AppendAllText(_recordingDataFile, line);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error recording data: {ex.Message}");
        }
    }

    private void GeneratePdfReport()
    {
        try
        {
            var scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "generate_report.py");
            
            if (!File.Exists(scriptPath))
            {
                MessageBox.Show($"Python script not found at: {scriptPath}\n\nPlease ensure generate_report.py is in the application directory.", 
                              "Script Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                RecordingStatusText.Text = "Ready to record";
                RecordingStatusText.Foreground = new SolidColorBrush(Color.FromRgb(128, 128, 128));
                return;
            }
            
            var outputPdf = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), 
                                        $"SystemMonitor_Report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
            
            var startInfo = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"\"{scriptPath}\" \"{_recordingDataFile}\" \"{outputPdf}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            
            var process = Process.Start(startInfo);
            if (process != null)
            {
                process.WaitForExit(30000); // Wait up to 30 seconds
                
                if (File.Exists(outputPdf))
                {
                    RecordingStatusText.Text = "PDF generated successfully!";
                    RecordingStatusText.Foreground = new SolidColorBrush(Color.FromRgb(78, 201, 176));
                    
                    var result = MessageBox.Show($"PDF report generated successfully!\n\nLocation: {outputPdf}\n\nWould you like to open it?", 
                                  "Success", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    
                    if (result == MessageBoxResult.Yes)
                    {
                        Process.Start(new ProcessStartInfo(outputPdf) { UseShellExecute = true });
                    }
                }
                else
                {
                    var error = process.StandardError.ReadToEnd();
                    var output = process.StandardOutput.ReadToEnd();
                    
                    string errorMessage = "PDF generation failed.";
                    if (error.Contains("No module named") || error.Contains("reportlab"))
                    {
                        errorMessage = "PDF generation failed: reportlab module not found.\n\n" +
                                     "Please install Python dependencies:\n" +
                                     "1. Open a command prompt\n" +
                                     "2. Navigate to the SystemMonitor folder\n" +
                                     "3. Run: pip install -r requirements.txt\n\n" +
                                     $"Error: {error}";
                    }
                    else
                    {
                        errorMessage = $"PDF generation failed.\n\nError: {error}\n\nOutput: {output}";
                    }
                    
                    MessageBox.Show(errorMessage, 
                                  "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    RecordingStatusText.Text = "PDF generation failed";
                    RecordingStatusText.Foreground = new SolidColorBrush(Color.FromRgb(244, 135, 113));
                }
            }
            
            // Clean up temp file
            try
            {
                if (File.Exists(_recordingDataFile))
                    File.Delete(_recordingDataFile);
            }
            catch { }
            
            Dispatcher.BeginInvoke(() =>
            {
                System.Threading.Thread.Sleep(3000);
                RecordingStatusText.Text = "Ready to record";
                RecordingStatusText.Foreground = new SolidColorBrush(Color.FromRgb(128, 128, 128));
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error generating PDF: {ex.Message}", 
                          "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            RecordingStatusText.Text = "Ready to record";
            RecordingStatusText.Foreground = new SolidColorBrush(Color.FromRgb(128, 128, 128));
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        _updateTimer?.Stop();
        _cpuCounter?.Dispose();
        _ramCounter?.Dispose();
        _gpuCounter?.Dispose();
        base.OnClosed(e);
    }
}