namespace ReviewService.Tests.Features.Reviews.Commands;

using Moq;
using Xunit;
using System.Threading;

using ReviewService.Features.Reviews.Commands;
using Models;
using Models.DTOs;
using ReviewService.Repositories.Review;
using ReviewService.Services.MQ;

public class DeleteReviewHandlerTests {
    private readonly Mock<IReviewRepository> _reviewRepositoryMock;
    private readonly Mock<IMessageBusService> _messageBusMock;
    private readonly DeleteReviewHandler _handler;

    public DeleteReviewHandlerTests() {
        _reviewRepositoryMock = new Mock<IReviewRepository>();
        _messageBusMock = new Mock<IMessageBusService>();
        _handler = new DeleteReviewHandler(_reviewRepositoryMock.Object, _messageBusMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingReview_ReturnsTrue() {
        // Arrange
        var command = new DeleteReviewCommand(1);
        var review = new Review { Id = 1, BookId = 2 };
        var statistics = new BookRatingStatistics { BookId = review.BookId };

        _reviewRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id)).ReturnsAsync(review);
        _reviewRepositoryMock.Setup(repo => repo.DeleteAsync(command.Id)).ReturnsAsync(true);
        _reviewRepositoryMock.Setup(repo => repo.GetBookStatisticsAsync(review.BookId)).ReturnsAsync(statistics);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        _messageBusMock.Verify(bus => bus.PublishReviewDeleted(review.Id, review.BookId), Times.Once);
        _messageBusMock.Verify(bus => bus.PublishBookRatingChanged(It.IsAny<BookRatingStatistics>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistentReview_ReturnsFalse() {
        // Arrange
        var command = new DeleteReviewCommand(999);
        _reviewRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id)).ReturnsAsync((Review?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
        _reviewRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<int>()), Times.Never);
        _messageBusMock.Verify(bus => bus.PublishReviewDeleted(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }
}
