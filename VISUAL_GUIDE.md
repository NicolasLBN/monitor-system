# System Monitor - Visual Guide

## Application Screenshot Description

Since this is a WPF Windows application being developed on Linux, here's a detailed description of what the application looks like when running:

### Main Window Layout

**Window Title**: "System Monitor"
**Size**: 1200x750 pixels
**Background**: Dark theme (#1E1E1E)

### Top Section
- **Title**: "System Monitor" in large white text (24px, bold, centered)

### Main Content Area (2x2 Grid)

#### Top-Left: CPU Monitor Panel
- **Border**: Rounded corners, dark background (#2D2D2D)
- **Title**: "CPU Usage" in teal color (#4EC9B0)
- **Gauge**: Speedometer-style needle gauge
  - Range: 0-100
  - Active fill: Teal (#4EC9B0)
  - Background: Dark gray (#3F3F3F)
  - Large percentage display at center
- **Bottom**: Current percentage in large text (e.g., "45.2%")

#### Top-Right: RAM Monitor Panel
- **Border**: Rounded corners, dark background (#2D2D2D)
- **Title**: "RAM Usage" in orange color (#CE9178)
- **Chart**: Real-time line chart
  - Orange line showing memory usage over time
  - X-axis: Time labels (HH:MM:SS)
  - Y-axis: Percentage (0-100%)
  - Shows last 30 seconds of data
- **Bottom**: 
  - "Used: X.XX GB"
  - "Total: X.XX GB"

#### Bottom-Left: Disk Monitor Panel
- **Border**: Rounded corners, dark background (#2D2D2D)
- **Title**: "Disk Usage" in purple color (#C586C0)
- **Chart**: Column chart (histogram)
  - Purple bars for "Used" space
  - Gray bars for "Free" space
  - One pair of columns per drive
  - X-axis: Drive letters (C:, D:, etc.)
  - Y-axis: Gigabytes (GB)
- **Bottom**: "Disks: X | Total: XXX GB"

#### Bottom-Right: Network & System Info Panel
- **Border**: Rounded corners, dark background (#2D2D2D)
- **Title**: "Network & System Info" in yellow color (#DCDCAA)
- **Content**: Scrollable list with two sections
  
  **Network Section**:
  - Header: "Network" in blue
  - Download: X.XX KB/s or MB/s
  - Upload: X.XX KB/s or MB/s
  
  **System Information Section**:
  - Header: "System Information" in blue
  - OS: Windows version or Linux distribution
  - Computer: Machine name
  - Processor: X cores
  - Uptime: Xd Xh Xm

### Bottom Section: Recording Controls
- **Border**: Rounded corners, dark background (#2D2D2D)
- **Label**: "Performance Recording:" in white
- **Status Text**: 
  - "Ready to record" (gray) when idle
  - "Recording..." (green) when active
  - "Generating PDF..." (orange) when processing
  - "PDF generated successfully!" (green) when done
- **Buttons**:
  - **Start Recording**: Green button (#4EC9B0), black text
  - **Stop Recording**: Red button (#F48771), black text
    - Disabled (grayed out) when not recording
    - Enabled when recording is active

### Visual Theme
- Modern dark theme throughout
- Professional color coding:
  - Teal for CPU (processing)
  - Orange for RAM (memory)
  - Purple for Disk (storage)
  - Yellow for Network/System (connectivity/info)
- Consistent spacing and padding
- Rounded corners on all panels
- Clear visual hierarchy

### Dynamic Elements
- **CPU Gauge**: Needle moves smoothly from 0-100%
- **RAM Chart**: Line grows from right to left as new data arrives
- **Disk Histogram**: Columns adjust height based on space used/free
- **Network Speeds**: Numbers update every second
- **Uptime**: Increments every minute

### User Interaction
1. Application starts → All panels begin updating every second
2. User clicks "Start Recording" → Button becomes disabled, Stop button enables
3. Application records data every second to CSV file
4. User clicks "Stop Recording" → Python script launches, generates PDF
5. Success dialog shows → User can choose to open PDF or dismiss
6. PDF opens in default PDF viewer (if user chose "Yes")
7. Application continues monitoring in real-time

### Sample PDF Report Layout

**Page Header**:
- Title: "System Monitor Performance Report"
- Generated timestamp
- Total records count
- Time range

**Summary Table**:
```
| Metric  | Minimum | Maximum | Average |
|---------|---------|---------|---------|
| CPU (%) | 15.20   | 87.50   | 42.30   |
| RAM (%) | 38.10   | 65.20   | 48.90   |
```

**Detailed Data Table**:
```
| Timestamp           | CPU (%) | RAM (%) | Network                    |
|---------------------|---------|---------|----------------------------|
| 2024-02-04 10:00:00 | 25.50   | 45.20   | ↓ 250.5 KB/s, ↑ 80.2 KB/s |
| 2024-02-04 10:00:01 | 28.30   | 46.10   | ↓ 280.3 KB/s, ↑ 85.5 KB/s |
| ...                 | ...     | ...     | ...                        |
```

## Color Reference

- **Dark Background**: #1E1E1E
- **Panel Background**: #2D2D2D
- **Border Color**: #3F3F3F
- **CPU Color (Teal)**: #4EC9B0
- **RAM Color (Orange)**: #CE9178
- **Disk Color (Purple)**: #C586C0
- **Network/System Color (Yellow)**: #DCDCAA
- **Text Color (Primary)**: #FFFFFF (white)
- **Text Color (Secondary)**: #808080 (gray)
- **Success Color**: #4EC9B0 (green)
- **Warning/Error Color**: #F48771 (red)
