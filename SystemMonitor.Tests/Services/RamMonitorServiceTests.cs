using Microsoft.Extensions.Logging;
using Moq;
using SystemMonitor.Services;
using Xunit;

namespace SystemMonitor.Tests.Services;

/// <summary>
/// Unit tests for RamMonitorService
/// </summary>
public class RamMonitorServiceTests
{
    private readonly Mock<ILogger<RamMonitorService>> _loggerMock;

    public RamMonitorServiceTests()
    {
        _loggerMock = new Mock<ILogger<RamMonitorService>>();
    }

    [Fact]
    public async Task GetRamUsageAsync_ReturnsRamUsageData()
    {
        // Arrange
        var service = new RamMonitorService(_loggerMock.Object);

        // Act
        var result = await service.GetRamUsageAsync();

        // Assert
        Assert.NotNull(result);
        Assert.InRange(result.UsagePercent, 0, 100);
        Assert.True(result.TotalBytes > 0);
        Assert.True(result.UsedBytes >= 0);
        Assert.True(result.Timestamp <= DateTime.Now);
    }

    [Fact]
    public void Dispose_DisposesServiceSuccessfully()
    {
        // Arrange
        var service = new RamMonitorService(_loggerMock.Object);

        // Act & Assert - Should not throw
        service.Dispose();
        service.Dispose(); // Second call should be safe
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RamMonitorService(null!));
    }

    [Fact]
    public async Task GetRamUsageAsync_CalculatesGBCorrectly()
    {
        // Arrange
        var service = new RamMonitorService(_loggerMock.Object);

        // Act
        var result = await service.GetRamUsageAsync();

        // Assert
        Assert.Equal(result.UsedBytes / (1024.0 * 1024 * 1024), result.UsedGB);
        Assert.Equal(result.TotalBytes / (1024.0 * 1024 * 1024), result.TotalGB);
    }
}
