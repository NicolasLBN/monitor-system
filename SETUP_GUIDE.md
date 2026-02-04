# Setup Guide for System Monitor

## Prerequisites

1. **.NET SDK 10.0 or later**
   - Download from: https://dotnet.microsoft.com/download
   
2. **Python 3.7 or later**
   - Download from: https://www.python.org/downloads/
   - Make sure Python is added to your PATH during installation

## Installation Steps

### 1. Install Python Dependencies

Open a terminal/command prompt in the `SystemMonitor` directory and run:

```bash
pip install -r requirements.txt
```

Or install manually:

```bash
pip install reportlab
```

### 2. Build the Application

```bash
cd SystemMonitor
dotnet build
```

### 3. Run the Application

```bash
dotnet run
```

Or run the compiled executable from:
```
SystemMonitor/bin/Debug/net10.0-windows/SystemMonitor.exe
```

## Features

### Real-Time Monitoring
- **CPU**: Speedometer gauge (0-100%)
- **RAM**: Real-time line chart with history
- **Disk**: Histogram showing all drives
- **Network**: Download/upload speeds
- **System Info**: OS, computer name, processor, uptime

### Performance Recording
1. Click **Start Recording** to begin data collection
2. Let the application monitor your system
3. Click **Stop Recording** to generate a PDF report
4. The PDF will be saved to your Desktop with timestamp

## Troubleshooting

### "Python not found"
- Make sure Python is installed and added to PATH
- Test by running `python --version` in terminal

### PDF Generation Fails
- Ensure reportlab is installed: `pip install reportlab`
- Check that Python can be called from command line

### Performance Counters Not Working
- This application works best on Windows
- On Linux/Mac, some features may have limited functionality

## Notes

- The application updates every second
- Recording data is stored in a CSV file temporarily
- PDF reports include summary statistics and detailed data
- The gauge uses a speedometer-style visualization
- Charts maintain a 30-second history window
