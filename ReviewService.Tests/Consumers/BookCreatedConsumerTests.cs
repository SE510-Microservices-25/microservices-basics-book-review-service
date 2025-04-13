namespace ReviewService.Tests.Consumers;

using Moq;
using Xunit;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MassTransit;

using BookService.Contracts;
using ReviewService.Consumers;
using ReviewService.Services.Event;
using ReviewService.Repositories.Book;
using Models;

public class BookCreatedConsumerTests {
    [Fact]
    public async Task Consume_WhenCalled_AddsBookToRepository() {
        // Arrange
        var loggerMock = new Mock<ILogger<BookCreatedConsumer>>();
        var eventProcessingMock = new Mock<IEventProcessingService>();
        var bookRepositoryMock = new Mock<IBookRepository>();
        var configMock = new Mock<IConfiguration>();
        var consumeContextMock = new Mock<ConsumeContext<BookCreated>>();
        
        eventProcessingMock.Setup(s => s.IsEventProcessedAsync(It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(false);
        
        configMock.Setup(c => c["MessageBus:ServiceSecretKey"])
            .Returns("test-key");
            
        var bookCreated = new BookCreated(1, "Test Book", "Test Author", "Test Genre", DateTime.UtcNow);
        consumeContextMock.Setup(c => c.Message).Returns(bookCreated);
        
        var headersMock = new Mock<Headers>();
        headersMock.Setup(h => h.Get(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("test-key");
        consumeContextMock.Setup(c => c.Headers).Returns(headersMock.Object);
        
        var consumer = new BookCreatedConsumer(
            loggerMock.Object, 
            eventProcessingMock.Object,
            bookRepositoryMock.Object,
            configMock.Object);

        // Act
        await consumer.Consume(consumeContextMock.Object);

        // Assert
        bookRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Book>()), Times.Once);
    }
}
