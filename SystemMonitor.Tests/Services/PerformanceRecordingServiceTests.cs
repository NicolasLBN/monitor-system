using Microsoft.Extensions.Logging;
using Moq;
using SystemMonitor.Services;
using Xunit;

namespace SystemMonitor.Tests.Services;

/// <summary>
/// Unit tests for PerformanceRecordingService
/// </summary>
public class PerformanceRecordingServiceTests
{
    private readonly Mock<ILogger<PerformanceRecordingService>> _loggerMock;

    public PerformanceRecordingServiceTests()
    {
        _loggerMock = new Mock<ILogger<PerformanceRecordingService>>();
    }

    [Fact]
    public async Task StartRecordingAsync_CreatesDataFile()
    {
        // Arrange
        var service = new PerformanceRecordingService(_loggerMock.Object);

        // Act
        var filePath = await service.StartRecordingAsync();

        // Assert
        Assert.False(string.IsNullOrEmpty(filePath));
        Assert.True(File.Exists(filePath));
        Assert.True(service.IsRecording);

        // Cleanup
        await service.StopRecordingAsync();
        if (File.Exists(filePath))
            File.Delete(filePath);
    }

    [Fact]
    public async Task StopRecordingAsync_StopsRecording()
    {
        // Arrange
        var service = new PerformanceRecordingService(_loggerMock.Object);
        await service.StartRecordingAsync();

        // Act
        var filePath = await service.StopRecordingAsync();

        // Assert
        Assert.False(service.IsRecording);
        Assert.False(string.IsNullOrEmpty(filePath));

        // Cleanup
        if (File.Exists(filePath))
            File.Delete(filePath);
    }

    [Fact]
    public async Task RecordDataPointAsync_WritesDataToFile()
    {
        // Arrange
        var service = new PerformanceRecordingService(_loggerMock.Object);
        var filePath = await service.StartRecordingAsync();

        // Act
        await service.RecordDataPointAsync(50.5, 75.3, "100 KB/s", "50 KB/s");

        // Assert
        var fileContent = await File.ReadAllTextAsync(filePath);
        Assert.Contains("50.50", fileContent);
        Assert.Contains("75.30", fileContent);

        // Cleanup
        await service.StopRecordingAsync();
        if (File.Exists(filePath))
            File.Delete(filePath);
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new PerformanceRecordingService(null!));
    }

    [Fact]
    public void IsRecording_InitiallyFalse()
    {
        // Arrange
        var service = new PerformanceRecordingService(_loggerMock.Object);

        // Assert
        Assert.False(service.IsRecording);
    }
}
