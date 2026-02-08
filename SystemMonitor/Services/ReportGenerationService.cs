using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.IO;

namespace SystemMonitor.Services;

/// <summary>
/// Implementation of report generation service using Python script
/// </summary>
public class ReportGenerationService : IReportGenerationService
{
    private readonly ILogger<ReportGenerationService> _logger;

    /// <summary>
    /// Initializes a new instance of the ReportGenerationService class
    /// </summary>
    /// <param name="logger">Logger instance for logging errors and information</param>
    public ReportGenerationService(ILogger<ReportGenerationService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Generates a PDF report from the recorded data file asynchronously
    /// </summary>
    /// <param name="dataFilePath">Path to the CSV data file</param>
    /// <param name="outputPdfPath">Path where the PDF should be saved</param>
    /// <returns>True if successful, false otherwise</returns>
    public async Task<bool> GenerateReportAsync(string dataFilePath, string outputPdfPath)
    {
        try
        {
            var scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "generate_report.py");

            if (!File.Exists(scriptPath))
            {
                _logger.LogError($"Python script not found at: {scriptPath}");
                return false;
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"\"{scriptPath}\" \"{dataFilePath}\" \"{outputPdfPath}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            _logger.LogInformation($"Generating PDF report: {outputPdfPath}");

            using var process = Process.Start(startInfo);
            
            if (process != null)
            {
                // Read output streams asynchronously to prevent deadlocks
                var outputTask = process.StandardOutput.ReadToEndAsync();
                var errorTask = process.StandardError.ReadToEndAsync();
                
                await process.WaitForExitAsync();
                
                var output = await outputTask;
                var error = await errorTask;

                if (File.Exists(outputPdfPath))
                {
                    _logger.LogInformation("PDF report generated successfully");
                    return true;
                }
                else
                {
                    _logger.LogError($"PDF generation failed. Error: {error}, Output: {output}");
                    return false;
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating PDF report");
            return false;
        }
        finally
        {
            // Clean up temp data file
            try
            {
                if (File.Exists(dataFilePath))
                {
                    File.Delete(dataFilePath);
                    _logger.LogInformation($"Deleted temporary data file: {dataFilePath}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Could not delete temporary file: {dataFilePath}");
            }
        }
    }
}
