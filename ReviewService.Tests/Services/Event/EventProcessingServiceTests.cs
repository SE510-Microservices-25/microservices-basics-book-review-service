namespace ReviewService.Tests.Services.Event;

using Moq;
using Xunit;
using Microsoft.Extensions.Logging;

using ReviewService.Services.Event;
using ReviewService.Repositories.Event;
using Models;

public class EventProcessingServiceTests {
    private readonly Mock<IEventRepository> _eventRepositoryMock;
    private readonly EventProcessingService _service;

    public EventProcessingServiceTests() {
        _eventRepositoryMock = new Mock<IEventRepository>();
        var loggerMock = new Mock<ILogger<EventProcessingService>>();
        _service = new EventProcessingService(_eventRepositoryMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task IsEventProcessedAsync_ProcessedEvent_ReturnsTrue() {
        // Arrange
        const int eventId = 1;
        const string eventType = "TestEvent";
        _eventRepositoryMock.Setup(repo => repo.ExistsAsync(eventId, eventType)).ReturnsAsync(true);

        // Act
        var result = await _service.IsEventProcessedAsync(eventId, eventType);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsEventProcessedAsync_UnprocessedEvent_ReturnsFalse() {
        // Arrange
        const int eventId = 1;
        const string eventType = "TestEvent";
        _eventRepositoryMock.Setup(repo => repo.ExistsAsync(eventId, eventType)).ReturnsAsync(false);

        // Act
        var result = await _service.IsEventProcessedAsync(eventId, eventType);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task MarkEventAsProcessedAsync_SavesEvent() {
        // Arrange
        const int eventId = 1;
        const string eventType = "TestEvent";

        // Act
        await _service.MarkEventAsProcessedAsync(eventId, eventType);

        // Assert
        _eventRepositoryMock.Verify(
            repo => repo.CreateAsync(It.Is<ProcessedEvent>(e => e.EventId == eventId && e.EventType == eventType)),
            Times.Once);
    }

    [Fact]
    public async Task MarkEventAsProcessedAsync_OnError_RethrowsException() {
        // Arrange
        const int eventId = 1;
        const string eventType = "TestEvent";
        var expectedException = new Exception("Test exception");
        _eventRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<ProcessedEvent>())).ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => 
            _service.MarkEventAsProcessedAsync(eventId, eventType));
            
        Assert.Same(expectedException, exception);
    }
}
