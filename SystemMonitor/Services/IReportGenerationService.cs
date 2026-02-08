namespace SystemMonitor.Services;

/// <summary>
/// Service for generating PDF reports from performance data
/// </summary>
public interface IReportGenerationService
{
    /// <summary>
    /// Generates a PDF report from the recorded data file asynchronously
    /// </summary>
    /// <param name="dataFilePath">Path to the CSV data file</param>
    /// <param name="outputPdfPath">Path where the PDF should be saved</param>
    /// <returns>True if successful, false otherwise</returns>
    Task<bool> GenerateReportAsync(string dataFilePath, string outputPdfPath);
}
