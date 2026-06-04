using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using ONENET.Application.Common.Interfaces;
using ONENET.Domain.Interfaces;
using ONENET.Infrastructure.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ONENET.UnitTests;

public class ReaderCodeGeneratorTests
{
    private readonly Mock<IReaderRepository> _mockRepository;
    private readonly Mock<IDateTime> _mockDateTime;
    private readonly Mock<ILogger<ReaderCodeGenerator>> _mockLogger;
    private readonly ReaderCodeGenerator _generator;

    public ReaderCodeGeneratorTests()
    {
        _mockRepository = new Mock<IReaderRepository>();
        _mockDateTime = new Mock<IDateTime>();
        _mockLogger = new Mock<ILogger<ReaderCodeGenerator>>();
        _generator = new ReaderCodeGenerator(_mockRepository.Object, _mockDateTime.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GenerateUniqueCodeAsync_ShouldReturnFirstCode_WhenCodeDoesNotExist()
    {
        // Arrange
        var testDate = new DateTime(2026, 6, 4, 0, 0, 0, DateTimeKind.Utc);
        _mockDateTime.Setup(d => d.UtcNow).Returns(testDate);
        
        // IsReaderCodeUniqueAsync returns false (means it does NOT exist in DB)
        _mockRepository.Setup(r => r.IsReaderCodeUniqueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var code = await _generator.GenerateUniqueCodeAsync();

        // Assert
        Assert.Equal("DG20260604001", code);
        _mockRepository.Verify(r => r.IsReaderCodeUniqueAsync("DG20260604001", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GenerateUniqueCodeAsync_ShouldIncrementSequence_WhenCodeAlreadyExists()
    {
        // Arrange
        var testDate = new DateTime(2026, 6, 4, 0, 0, 0, DateTimeKind.Utc);
        _mockDateTime.Setup(d => d.UtcNow).Returns(testDate);
        
        // First code exists (returns true), second does not (returns false)
        _mockRepository.Setup(r => r.IsReaderCodeUniqueAsync("DG20260604001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockRepository.Setup(r => r.IsReaderCodeUniqueAsync("DG20260604002", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var code = await _generator.GenerateUniqueCodeAsync();

        // Assert
        Assert.Equal("DG20260604002", code);
        _mockRepository.Verify(r => r.IsReaderCodeUniqueAsync("DG20260604001", It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.IsReaderCodeUniqueAsync("DG20260604002", It.IsAny<CancellationToken>()), Times.Once);
    }
}
