namespace ReviewService.Tests.Consumers;

using Moq;
using Xunit;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MassTransit;

using BookService.Contracts;
using ReviewService.Consumers;
using ReviewService.Services.Event;
using ReviewService.Repositories.Review;
using ReviewService.Repositories.Book;

public class BookDeletedConsumerTests {
    [Fact]
    public async Task Consume_WhenCalled_DeletesReviewsAndBook() {
        // Arrange
        var loggerMock = new Mock<ILogger<BookDeletedConsumer>>();
        var eventProcessingMock = new Mock<IEventProcessingService>();
        var reviewRepositoryMock = new Mock<IReviewRepository>();
        var bookRepositoryMock = new Mock<IBookRepository>();
        var configMock = new Mock<IConfiguration>();
        var consumeContextMock = new Mock<ConsumeContext<BookDeleted>>();
        
        eventProcessingMock.Setup(s => s.IsEventProcessedAsync(It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(false);
        
        configMock.Setup(c => c["MessageBus:ServiceSecretKey"])
            .Returns("test-key");
            
        var bookDeleted = new BookDeleted(1);
        consumeContextMock.Setup(c => c.Message).Returns(bookDeleted);
        
        var headersMock = new Mock<Headers>();
        headersMock.Setup(h => h.Get(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("test-key");
        consumeContextMock.Setup(c => c.Headers).Returns(headersMock.Object);
        
        reviewRepositoryMock.Setup(r => r.DeleteByBookIdAsync(It.IsAny<int>()))
            .ReturnsAsync(1);
        bookRepositoryMock.Setup(r => r.DeleteAsync(It.IsAny<int>()))
            .ReturnsAsync(true);
        
        var consumer = new BookDeletedConsumer(
            loggerMock.Object, 
            eventProcessingMock.Object,
            reviewRepositoryMock.Object,
            bookRepositoryMock.Object,
            configMock.Object);

        // Act
        await consumer.Consume(consumeContextMock.Object);

        // Assert
        reviewRepositoryMock.Verify(r => r.DeleteByBookIdAsync(It.IsAny<int>()), Times.Once);
        bookRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Once);
    }
}
