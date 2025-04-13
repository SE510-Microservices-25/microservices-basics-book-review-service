namespace ReviewService.Tests.Services.MQ;

using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

using ReviewService.Services.MQ;
using ReviewService.Repositories.Outbox;
using Models;
using Contracts;

public class MessageBusServiceTests {
    private readonly Mock<IOutboxRepository> _outboxRepositoryMock;
    private readonly MessageBusService _service;
    private const string ServiceSecretKey = "test-secret-key";

    public MessageBusServiceTests() {
        _outboxRepositoryMock = new Mock<IOutboxRepository>();
        var loggerMock = new Mock<ILogger<MessageBusService>>();
        var configurationMock = new Mock<IConfiguration>();
        configurationMock.Setup(c => c["MessageBus:ServiceSecretKey"]).Returns(ServiceSecretKey);
        _service = new MessageBusService(_outboxRepositoryMock.Object, loggerMock.Object, configurationMock.Object);
    }

    [Fact]
    public async Task PublishReviewCreated_AddsMessageToOutbox() {
        // Arrange
        var review = new Review { Id = 1, BookId = 2, Rating = 5 };

        // Act
        await _service.PublishReviewCreated(review);

        // Assert
        _outboxRepositoryMock.Verify(
            repo => repo.AddAsync(review, It.IsAny<string>(), ServiceSecretKey),
            Times.Once);
    }

    [Fact]
    public async Task PublishReviewDeleted_AddsMessageToOutbox() {
        // Arrange
        const int reviewId = 1;
        const int bookId = 2;

        // Act
        await _service.PublishReviewDeleted(reviewId, bookId);

        // Assert
        _outboxRepositoryMock.Verify(
            repo => repo.AddAsync(It.Is<ReviewDeleted>(r => r.Id == reviewId), It.IsAny<string>(), ServiceSecretKey),
            Times.Once);
    }

    [Fact]
    public async Task PublishBookRatingChanged_AddsMessageToOutbox() {
        // Arrange
        var statistics = new BookRatingStatistics { BookId = 1 };

        // Act
        await _service.PublishBookRatingChanged(statistics);

        // Assert
        _outboxRepositoryMock.Verify(
            repo => repo.AddAsync(It.Is<BookRatingStatistics>(s => s.BookId == 1), It.IsAny<string>(), ServiceSecretKey),
            Times.Once);
    }
}
