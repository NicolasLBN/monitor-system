#!/usr/bin/env python3
"""
Test Data Generator for System Monitor
Generates sample CSV data to test PDF report generation
"""

import random
import csv
from datetime import datetime, timedelta

def generate_test_data(output_file, duration_minutes=5):
    """
    Generate test performance data
    
    Args:
        output_file: Path to output CSV file
        duration_minutes: How many minutes of data to generate
    """
    print(f"Generating {duration_minutes} minutes of test data...")
    
    with open(output_file, 'w', newline='', encoding='utf-8') as f:
        writer = csv.writer(f)
        
        # Write header
        writer.writerow(['Timestamp', 'CPU', 'RAM', 'Network Download', 'Network Upload'])
        
        # Generate data points (one per second)
        start_time = datetime.now() - timedelta(minutes=duration_minutes)
        total_points = duration_minutes * 60
        
        # Simulate realistic performance patterns
        cpu_base = 30.0
        ram_base = 45.0
        
        for i in range(total_points):
            timestamp = start_time + timedelta(seconds=i)
            
            # CPU: varies between 20-80% with some spikes
            cpu = cpu_base + random.uniform(-10, 30)
            if random.random() < 0.05:  # 5% chance of spike
                cpu += random.uniform(10, 20)
            cpu = max(0, min(100, cpu))
            
            # RAM: slowly increases over time
            ram = ram_base + (i / total_points * 15) + random.uniform(-5, 5)
            ram = max(0, min(100, ram))
            
            # Network: varies more dramatically
            download_kbps = random.uniform(100, 1500)
            upload_kbps = random.uniform(50, 500)
            
            # Format network speeds
            if download_kbps > 1024:
                download = f"{download_kbps / 1024:.2f} MB/s"
            else:
                download = f"{download_kbps:.2f} KB/s"
            
            if upload_kbps > 1024:
                upload = f"{upload_kbps / 1024:.2f} MB/s"
            else:
                upload = f"{upload_kbps:.2f} KB/s"
            
            # Write row
            writer.writerow([
                timestamp.strftime('%Y-%m-%d %H:%M:%S'),
                f"{cpu:.2f}",
                f"{ram:.2f}",
                download,
                upload
            ])
            
            # Adjust base values for next iteration (drift)
            cpu_base = cpu_base * 0.99 + cpu * 0.01
            ram_base = ram_base * 0.99 + ram * 0.01
    
    print(f"Test data saved to: {output_file}")
    print(f"Generated {total_points} data points")

if __name__ == "__main__":
    import sys
    
    output_file = "test_performance_data.csv"
    duration = 5  # minutes
    
    if len(sys.argv) > 1:
        output_file = sys.argv[1]
    if len(sys.argv) > 2:
        duration = int(sys.argv[2])
    
    generate_test_data(output_file, duration)
    print(f"\nTo generate a PDF report, run:")
    print(f"python generate_report.py {output_file} output_report.pdf")
