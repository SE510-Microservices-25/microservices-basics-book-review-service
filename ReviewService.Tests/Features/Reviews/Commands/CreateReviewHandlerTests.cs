namespace ReviewService.Tests.Features.Reviews.Commands;

using Moq;
using Xunit;

using ReviewService.Features.Reviews.Commands;
using Models;
using Models.DTOs;
using ReviewService.Repositories.Review;
using ReviewService.Repositories.Book;
using ReviewService.Services.MQ;

public class CreateReviewHandlerTests {
    private readonly Mock<IReviewRepository> _reviewRepositoryMock;
    private readonly Mock<IMessageBusService> _messageBusMock;
    private readonly Mock<IBookRepository> _bookRepositoryMock;
    private readonly CreateReviewHandler _handler;

    public CreateReviewHandlerTests() {
        _reviewRepositoryMock = new Mock<IReviewRepository>();
        _messageBusMock = new Mock<IMessageBusService>();
        _bookRepositoryMock = new Mock<IBookRepository>();
        _handler = new CreateReviewHandler(_reviewRepositoryMock.Object, _messageBusMock.Object, _bookRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsCreatedReview() {
        // Arrange
        var command = new CreateReviewCommand(1, "Test User", "Great book", 5);
        var createdReview = new Review {
            Id = 1,
            BookId = command.BookId,
            ReviewerName = command.ReviewerName,
            Content = command.Content,
            Rating = command.Rating
        };
        var statistics = new BookRatingStatistics { BookId = command.BookId };

        _bookRepositoryMock.Setup(repo => repo.ExistsAsync(command.BookId)).ReturnsAsync(true);
        _reviewRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Review>())).ReturnsAsync(createdReview);
        _reviewRepositoryMock.Setup(repo => repo.GetBookStatisticsAsync(command.BookId)).ReturnsAsync(statistics);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.BookId, result.BookId);
        Assert.Equal(command.Rating, result.Rating);

        _messageBusMock.Verify(bus => bus.PublishReviewCreated(It.IsAny<Review>()), Times.Once);
        _messageBusMock.Verify(bus => bus.PublishBookRatingChanged(It.IsAny<BookRatingStatistics>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistentBook_ThrowsInvalidOperationException() {
        // Arrange
        var command = new CreateReviewCommand(999, "Test User", "Great book", 5);
        _bookRepositoryMock.Setup(repo => repo.ExistsAsync(command.BookId)).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _handler.Handle(command, CancellationToken.None));
            
        _reviewRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Review>()), Times.Never);
    }
}
