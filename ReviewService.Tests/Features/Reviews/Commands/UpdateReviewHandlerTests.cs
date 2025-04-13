namespace ReviewService.Tests.Features.Reviews.Commands;

using Moq;
using Xunit;
using System.Threading;

using ReviewService.Features.Reviews.Commands;
using Models;
using Models.DTOs;
using ReviewService.Repositories.Review;
using ReviewService.Services.MQ;

public class UpdateReviewHandlerTests {
    private readonly Mock<IReviewRepository> _reviewRepositoryMock;
    private readonly Mock<IMessageBusService> _messageBusMock;
    private readonly UpdateReviewHandler _handler;

    public UpdateReviewHandlerTests() {
        _reviewRepositoryMock = new Mock<IReviewRepository>();
        _messageBusMock = new Mock<IMessageBusService>();
        _handler = new UpdateReviewHandler(_reviewRepositoryMock.Object, _messageBusMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsUpdatedReview() {
        // Arrange
        var command = new UpdateReviewCommand(1, "Updated User", "Updated content", 4);
        var updatedReview = new Review {
            Id = command.Id,
            BookId = 1,
            ReviewerName = command.ReviewerName,
            Rating = command.Rating
        };
        var statistics = new BookRatingStatistics { BookId = updatedReview.BookId };

        _reviewRepositoryMock.Setup(repo => repo.UpdateAsync(command)).ReturnsAsync(updatedReview);
        _reviewRepositoryMock.Setup(repo => repo.GetBookStatisticsAsync(updatedReview.BookId)).ReturnsAsync(statistics);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Equal(command.Rating, result.Rating);

        _messageBusMock.Verify(bus => bus.PublishReviewUpdated(It.IsAny<Review>()), Times.Once);
        _messageBusMock.Verify(bus => bus.PublishBookRatingChanged(It.IsAny<BookRatingStatistics>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistentReview_ReturnsNull() {
        // Arrange
        var command = new UpdateReviewCommand(999, "Updated User", "Updated content", 4);
        _reviewRepositoryMock.Setup(repo => repo.UpdateAsync(command)).ReturnsAsync((Review?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _messageBusMock.Verify(bus => bus.PublishReviewUpdated(It.IsAny<Review>()), Times.Never);
    }
}
