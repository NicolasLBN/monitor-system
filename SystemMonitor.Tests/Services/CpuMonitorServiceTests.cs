using Microsoft.Extensions.Logging;
using Moq;
using SystemMonitor.Services;
using Xunit;

namespace SystemMonitor.Tests.Services;

/// <summary>
/// Unit tests for CpuMonitorService
/// </summary>
public class CpuMonitorServiceTests
{
    private readonly Mock<ILogger<CpuMonitorService>> _loggerMock;

    public CpuMonitorServiceTests()
    {
        _loggerMock = new Mock<ILogger<CpuMonitorService>>();
    }

    [Fact]
    public async Task GetCpuUsageAsync_ReturnsCpuUsageData()
    {
        // Arrange
        var service = new CpuMonitorService(_loggerMock.Object);

        // Act
        var result = await service.GetCpuUsageAsync();

        // Assert
        Assert.NotNull(result);
        Assert.InRange(result.UsagePercent, 0, 100);
        Assert.True(result.Timestamp <= DateTime.Now);
    }

    [Fact]
    public void Dispose_DisposesServiceSuccessfully()
    {
        // Arrange
        var service = new CpuMonitorService(_loggerMock.Object);

        // Act & Assert - Should not throw
        service.Dispose();
        service.Dispose(); // Second call should be safe
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CpuMonitorService(null!));
    }
}
