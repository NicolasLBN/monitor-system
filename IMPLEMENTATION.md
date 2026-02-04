# System Monitor - Implementation Details

## Vue d'ensemble / Overview

Cette application est un moniteur système développé en .NET WPF qui affiche des informations en temps réel sur les performances du système et permet d'enregistrer ces données dans des rapports PDF.

This application is a system monitor developed in .NET WPF that displays real-time system performance information and allows recording this data in PDF reports.

## Architecture

### Components WPF / WPF Components

#### 1. CPU Monitor - Jauge à Aiguille / Needle Gauge
- **Type**: LiveCharts Gauge (speedometer style)
- **Range**: 0-100%
- **Update**: Every second
- **Source**: PerformanceCounter ("Processor", "% Processor Time")
- **Fallback**: Process-based CPU estimation for non-Windows systems

#### 2. RAM Monitor - Graphique / Graph
- **Type**: LiveCharts Line Chart
- **History**: 30 data points (30 seconds)
- **Display**: Percentage and actual values (GB)
- **Source**: PerformanceCounter ("Memory", "% Committed Bytes In Use")
- **Info**: Shows used/total memory

#### 3. Disk Monitor - Histogramme / Histogram
- **Type**: LiveCharts Column Chart
- **Drives**: All ready drives
- **Data**: Used and Free space per drive
- **Units**: Gigabytes (GB)
- **Update**: Every second

#### 4. Network Monitor - Vitesse Réseau / Network Speed
- **Metrics**: Download and Upload speeds
- **Units**: KB/s or MB/s (automatic)
- **Source**: NetworkInterface statistics
- **Display**: Real-time speed calculation

#### 5. System Information
- Operating System version
- Computer name
- Processor core count
- System uptime (days, hours, minutes)

### Performance Recording / Enregistrement des Performances

#### CSV Data Collection
When recording is active, the application collects:
- Timestamp (YYYY-MM-DD HH:MM:SS)
- CPU usage (%)
- RAM usage (%)
- Network download speed
- Network upload speed

Data is saved to a temporary CSV file every second.

#### PDF Report Generation
The Python script (`generate_report.py`) generates a PDF with:

1. **Title and Metadata**
   - Report generation date/time
   - Total number of records
   - Time range of the recording

2. **Summary Statistics Table**
   - CPU: Minimum, Maximum, Average
   - RAM: Minimum, Maximum, Average

3. **Detailed Data Table** (4 columns as requested)
   - Column 1: Timestamp
   - Column 2: CPU (%)
   - Column 3: RAM (%)
   - Column 4: Network (Download ↓ / Upload ↑)

4. **Styling**
   - Professional colors
   - Alternating row colors
   - Clear headers
   - Grid layout

## Technologies / Technologies Utilisées

### .NET / WPF
- **Framework**: .NET 10.0
- **UI**: Windows Presentation Foundation (WPF)
- **Language**: C# with nullable reference types

### NuGet Packages
1. **LiveCharts.Wpf** (v0.9.7)
   - For gauge and chart visualizations
   - Provides Gauge, LineChart, and ColumnChart controls

2. **System.Diagnostics.PerformanceCounter** (v9.0.0)
   - For Windows performance monitoring
   - Provides CPU and RAM metrics

### Python
- **Python**: 3.7+
- **ReportLab**: PDF generation library
  - Table layouts
  - Styling and colors
  - Multi-page support

## UI Design / Conception de l'Interface

### Color Scheme / Schéma de Couleurs
- Background: Dark theme (#1E1E1E, #2D2D2D)
- CPU: Teal (#4EC9B0)
- RAM: Orange (#CE9178)
- Disk: Purple (#C586C0)
- Network/System: Yellow (#DCDCAA)
- Recording: Green (#4EC9B0) / Red (#F48771)

### Layout / Disposition
- Grid layout: 2x2 main panels
- Top: Title bar
- Middle: 4 monitoring panels
- Bottom: Recording controls

## Data Flow / Flux de Données

1. **Monitoring Loop**
   ```
   Timer (1s) → Update CPU → Update RAM → Update Disk → Update Network
   ```

2. **Recording Flow**
   ```
   Start Recording → Create CSV → Collect Data (1s intervals) → Stop Recording → Generate PDF
   ```

3. **PDF Generation**
   ```
   CSV File → Python Script → Parse Data → Generate PDF → Save to Desktop
   ```

## Platform Compatibility / Compatibilité

### Windows
- ✅ Full feature support
- ✅ All performance counters work
- ✅ Best experience

### Linux/Mac
- ⚠️ Limited performance counter support
- ✅ Basic monitoring still works
- ✅ PDF generation works
- ⚠️ Some features may show placeholder values

## File Structure / Structure des Fichiers

```
monitor-system/
├── README.md                    # Main documentation
├── SETUP_GUIDE.md              # Installation instructions
├── .gitignore                  # Git ignore rules
└── SystemMonitor/
    ├── SystemMonitor.csproj    # Project configuration
    ├── App.xaml                # Application entry point
    ├── App.xaml.cs             # Application code-behind
    ├── MainWindow.xaml         # Main UI definition
    ├── MainWindow.xaml.cs      # Main application logic
    ├── generate_report.py      # PDF generation script
    └── requirements.txt        # Python dependencies
```

## Future Enhancements / Améliorations Futures

Potential improvements:
- [ ] Save/load recording sessions
- [ ] Configurable update intervals
- [ ] More chart types (temperature, GPU, etc.)
- [ ] Alert thresholds
- [ ] Multiple language support
- [ ] Export to other formats (Excel, JSON)
- [ ] Historical data comparison
- [ ] Email report functionality

## Troubleshooting / Dépannage

### Build Issues
- Ensure .NET 10.0 SDK is installed
- Run `dotnet restore` to restore packages
- Enable Windows targeting for cross-platform builds

### Runtime Issues
- Performance counters require Windows
- Python must be in system PATH
- ReportLab must be installed via pip

### PDF Generation Issues
- Check Python installation
- Verify reportlab package: `pip list | grep reportlab`
- Ensure write permissions on Desktop folder
