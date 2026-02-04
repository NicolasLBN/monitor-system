#!/usr/bin/env python3
"""
System Monitor PDF Report Generator
Generates a PDF report from performance monitoring data
"""

import sys
import csv
from datetime import datetime
from reportlab.lib import colors
from reportlab.lib.pagesizes import letter, A4
from reportlab.platypus import SimpleDocTemplate, Table, TableStyle, Paragraph, Spacer, PageBreak
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.lib.units import inch
from reportlab.lib.enums import TA_CENTER, TA_LEFT

def generate_pdf_report(csv_file, output_pdf):
    """
    Generate a PDF report from CSV data with 4 columns: RAM, CPU, Network, and Timestamp
    """
    try:
        # Read CSV data
        data = []
        with open(csv_file, 'r', encoding='utf-8') as f:
            reader = csv.DictReader(f)
            for row in reader:
                data.append(row)
        
        if not data:
            print("No data found in CSV file")
            return False
        
        # Create PDF
        doc = SimpleDocTemplate(output_pdf, pagesize=letter,
                                rightMargin=30, leftMargin=30,
                                topMargin=30, bottomMargin=30)
        
        # Container for the 'Flowable' objects
        elements = []
        
        # Define styles
        styles = getSampleStyleSheet()
        title_style = ParagraphStyle(
            'CustomTitle',
            parent=styles['Heading1'],
            fontSize=24,
            textColor=colors.HexColor('#2E86AB'),
            spaceAfter=30,
            alignment=TA_CENTER
        )
        
        heading_style = ParagraphStyle(
            'CustomHeading',
            parent=styles['Heading2'],
            fontSize=14,
            textColor=colors.HexColor('#06AED5'),
            spaceAfter=12,
            alignment=TA_LEFT
        )
        
        # Add title
        title = Paragraph("System Monitor Performance Report", title_style)
        elements.append(title)
        
        # Add report info
        report_info = Paragraph(f"<b>Generated:</b> {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}<br/>"
                                f"<b>Total Records:</b> {len(data)}<br/>"
                                f"<b>Time Range:</b> {data[0]['Timestamp']} to {data[-1]['Timestamp']}", 
                                styles['Normal'])
        elements.append(report_info)
        elements.append(Spacer(1, 20))
        
        # Add summary statistics
        elements.append(Paragraph("Summary Statistics", heading_style))
        
        # Calculate statistics
        cpu_values = [float(row['CPU']) for row in data]
        ram_values = [float(row['RAM']) for row in data]
        
        summary_data = [
            ['Metric', 'Minimum', 'Maximum', 'Average'],
            ['CPU (%)', f"{min(cpu_values):.2f}", f"{max(cpu_values):.2f}", f"{sum(cpu_values)/len(cpu_values):.2f}"],
            ['RAM (%)', f"{min(ram_values):.2f}", f"{max(ram_values):.2f}", f"{sum(ram_values)/len(ram_values):.2f}"]
        ]
        
        summary_table = Table(summary_data, colWidths=[2*inch, 1.5*inch, 1.5*inch, 1.5*inch])
        summary_table.setStyle(TableStyle([
            ('BACKGROUND', (0, 0), (-1, 0), colors.HexColor('#2E86AB')),
            ('TEXTCOLOR', (0, 0), (-1, 0), colors.whitesmoke),
            ('ALIGN', (0, 0), (-1, -1), 'CENTER'),
            ('FONTNAME', (0, 0), (-1, 0), 'Helvetica-Bold'),
            ('FONTSIZE', (0, 0), (-1, 0), 12),
            ('BOTTOMPADDING', (0, 0), (-1, 0), 12),
            ('BACKGROUND', (0, 1), (-1, -1), colors.beige),
            ('GRID', (0, 0), (-1, -1), 1, colors.black)
        ]))
        elements.append(summary_table)
        elements.append(Spacer(1, 20))
        
        # Add detailed data table
        elements.append(Paragraph("Detailed Performance Data", heading_style))
        
        # Prepare table data with 4 columns as requested
        table_data = [['Timestamp', 'CPU (%)', 'RAM (%)', 'Network']]
        
        for row in data:
            network_info = f"↓ {row['Network Download']}, ↑ {row['Network Upload']}"
            table_data.append([
                row['Timestamp'],
                f"{float(row['CPU']):.2f}",
                f"{float(row['RAM']):.2f}",
                network_info
            ])
        
        # Create table with appropriate column widths for 4 columns
        col_widths = [2.2*inch, 1.2*inch, 1.2*inch, 2*inch]
        perf_table = Table(table_data, colWidths=col_widths, repeatRows=1)
        
        # Style the table
        table_style = TableStyle([
            # Header row
            ('BACKGROUND', (0, 0), (-1, 0), colors.HexColor('#06AED5')),
            ('TEXTCOLOR', (0, 0), (-1, 0), colors.whitesmoke),
            ('ALIGN', (0, 0), (-1, -1), 'CENTER'),
            ('FONTNAME', (0, 0), (-1, 0), 'Helvetica-Bold'),
            ('FONTSIZE', (0, 0), (-1, 0), 10),
            ('BOTTOMPADDING', (0, 0), (-1, 0), 12),
            
            # Data rows
            ('FONTNAME', (0, 1), (-1, -1), 'Helvetica'),
            ('FONTSIZE', (0, 1), (-1, -1), 8),
            ('GRID', (0, 0), (-1, -1), 0.5, colors.grey),
            ('VALIGN', (0, 0), (-1, -1), 'MIDDLE'),
            
            # Alternating row colors
            ('ROWBACKGROUNDS', (0, 1), (-1, -1), [colors.white, colors.lightgrey])
        ])
        
        perf_table.setStyle(table_style)
        elements.append(perf_table)
        
        # Build PDF
        doc.build(elements)
        print(f"PDF report generated successfully: {output_pdf}")
        return True
        
    except Exception as e:
        print(f"Error generating PDF: {str(e)}", file=sys.stderr)
        import traceback
        traceback.print_exc()
        return False

if __name__ == "__main__":
    if len(sys.argv) != 3:
        print("Usage: python generate_report.py <input_csv> <output_pdf>")
        sys.exit(1)
    
    csv_file = sys.argv[1]
    output_pdf = sys.argv[2]
    
    success = generate_pdf_report(csv_file, output_pdf)
    sys.exit(0 if success else 1)
