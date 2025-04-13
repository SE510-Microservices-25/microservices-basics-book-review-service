namespace ReviewService.Tests.Features.Reviews.Queries;

using Moq;
using Xunit;
using System.Threading;
using System.Collections.Generic;

using ReviewService.Features.Reviews.Queries;
using Models;
using Models.DTOs;
using ReviewService.Repositories.Review;

public class QueryHandlersTests {
    private readonly Mock<IReviewRepository> _reviewRepositoryMock = new();

    [Fact]
    public async Task GetReviewById_ExistingReview_ReturnsReview() {
        // Arrange
        const int reviewId = 1;
        var review = new Review { Id = reviewId, BookId = 2, Rating = 5 };
        
        _reviewRepositoryMock.Setup(repo => repo.GetByIdAsync(reviewId)).ReturnsAsync(review);
            
        var handler = new ReviewQueryHandlers.GetReviewByIdHandler(_reviewRepositoryMock.Object);
        var query = new GetReviewByIdQuery(reviewId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(reviewId, result.Id);
    }

    [Fact]
    public async Task GetReviewById_NonExistentReview_ReturnsNull() {
        // Arrange
        const int reviewId = 999;
        _reviewRepositoryMock.Setup(repo => repo.GetByIdAsync(reviewId)).ReturnsAsync((Review?)null);
            
        var handler = new ReviewQueryHandlers.GetReviewByIdHandler(_reviewRepositoryMock.Object);
        var query = new GetReviewByIdQuery(reviewId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetReviewsByBookId_ReturnsReviewsForBook() {
        // Arrange
        const int bookId = 1;
        var reviews = new List<Review> {
            new() { Id = 1, BookId = bookId, Rating = 4 },
            new() { Id = 2, BookId = bookId, Rating = 5 }
        };
        
        _reviewRepositoryMock.Setup(repo => repo.GetByBookIdAsync(bookId)).ReturnsAsync(reviews);
            
        var handler = new ReviewQueryHandlers.GetReviewsByBookIdHandler(_reviewRepositoryMock.Object);
        var query = new GetReviewsByBookIdQuery(bookId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetBookStatistics_ReturnsStatisticsForBook() {
        // Arrange
        const int bookId = 1;
        var statistics = new BookRatingStatistics {
            BookId = bookId,
            AverageRating = 4.5,
            ReviewCount = 2
        };
        
        _reviewRepositoryMock.Setup(repo => repo.GetBookStatisticsAsync(bookId)).ReturnsAsync(statistics);
            
        var handler = new ReviewQueryHandlers.GetBookStatisticsHandler(_reviewRepositoryMock.Object);
        var query = new GetBookStatisticsQuery(bookId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(bookId, result.BookId);
        Assert.Equal(4.5, result.AverageRating);
    }
}
