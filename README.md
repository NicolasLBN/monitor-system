# monitor-system

A .NET WPF System Monitor application that displays real-time system performance metrics and generates PDF reports.

## Features

### Real-time Monitoring
- **CPU Usage**: Displayed with a speedometer-style needle gauge (0-100%)
- **RAM Usage**: Line chart showing memory usage over time
- **Disk Usage**: Histogram showing used and free space for all drives
- **Network Speed**: Real-time download and upload speeds
- **System Information**: OS version, computer name, processor info, and uptime

### Performance Recording
- Click "Start Recording" to begin collecting performance data
- Click "Stop Recording" to generate a PDF report
- PDF report includes:
  - Summary statistics (min, max, average for CPU and RAM)
  - Detailed performance data table with 4 columns:
    1. Timestamp
    2. CPU usage (%)
    3. RAM usage (%)
    4. Network speed (download/upload)
- Reports are saved to your Desktop

## Requirements

### .NET Requirements
- .NET 10.0 or later
- Windows OS (for full performance counter support)

### Python Requirements
- Python 3.7 or later
- reportlab library

## Setup

1. **Install Python dependencies:**
   ```bash
   cd SystemMonitor
   pip install -r requirements.txt
   ```

2. **Build the application:**
   ```bash
   dotnet build
   ```

3. **Run the application:**
   ```bash
   dotnet run
   ```

## Usage

1. Launch the application to see real-time system monitoring
2. To record performance:
   - Click "Start Recording"
   - Let the application run while monitoring your system
   - Click "Stop Recording" when done
   - The PDF report will be generated and saved to your Desktop

## Project Structure

```
SystemMonitor/
├── MainWindow.xaml          # UI layout
├── MainWindow.xaml.cs       # Application logic
├── generate_report.py       # Python script for PDF generation
├── requirements.txt         # Python dependencies
└── SystemMonitor.csproj     # Project configuration
```

## Technologies Used

- **WPF (Windows Presentation Foundation)**: Modern UI framework
- **LiveCharts**: Charting library for gauges and graphs
- **PerformanceCounter**: System performance monitoring
- **Python + ReportLab**: PDF report generation

## Notes

- The application works best on Windows systems where all performance counters are available
- On non-Windows systems, some features may have limited functionality
- Make sure Python is installed and available in your system PATH for PDF generation to work
