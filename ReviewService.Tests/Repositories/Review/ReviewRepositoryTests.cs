namespace ReviewService.Tests.Repositories.Review;

using Microsoft.EntityFrameworkCore;
using Xunit;
using System;
using System.Linq;
using System.Threading.Tasks;

using Data;
using Models;
using Models.DTOs;
using ReviewService.Repositories.Review;

public class ReviewRepositoryTests {
    private readonly ReviewDbContext _context;
    private readonly ReviewRepository _repository;

    public ReviewRepositoryTests() {
        var options = new DbContextOptionsBuilder<ReviewDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            
        _context = new ReviewDbContext(options);
        SeedDatabase();
        
        _repository = new ReviewRepository(_context);
    }

    private void SeedDatabase() {
        var reviews = new[] {
            new Review { Id = 1, BookId = 1, ReviewerName = "User 1", Rating = 4, CreatedAt = DateTime.UtcNow },
            new Review { Id = 2, BookId = 1, ReviewerName = "User 2", Rating = 5, CreatedAt = DateTime.UtcNow },
            new Review { Id = 3, BookId = 2, ReviewerName = "User 3", Rating = 3, CreatedAt = DateTime.UtcNow }
        };
        
        _context.Reviews.AddRange(reviews);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllReviews() {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ExistingReview_ReturnsReview() {
        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(1, result.BookId);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistentReview_ReturnsNull() {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByBookIdAsync_ReturnsReviewsForBook() {
        // Act
        var result = (await _repository.GetByBookIdAsync(1)).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.True(result.All(r => r.BookId == 1));
    }
    
    [Fact]
    public async Task CreateAsync_AddsNewReview() {
        // Arrange
        var newReview = new Review {
            BookId = 3,
            ReviewerName = "New User",
            Rating = 5
        };

        // Act
        var result = await _repository.CreateAsync(newReview);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        
        var savedReview = await _context.Reviews.FindAsync(result.Id);
        Assert.NotNull(savedReview);
        Assert.Equal(newReview.BookId, savedReview.BookId);
    }

    [Fact]
    public async Task UpdateAsync_ExistingReview_UpdatesReview() {
        // Arrange
        var command = new UpdateReviewCommand(1, "Updated User", "Updated Content", 5);

        // Act
        var result = await _repository.UpdateAsync(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Equal(command.Rating, result.Rating);
        
        var updatedReview = await _context.Reviews.FindAsync(command.Id);
        Assert.Equal(command.ReviewerName, updatedReview?.ReviewerName);
    }

    [Fact]
    public async Task DeleteAsync_ExistingReview_DeletesReview() {
        // Act
        var result = await _repository.DeleteAsync(1);

        // Assert
        Assert.True(result);
        Assert.Null(await _context.Reviews.FindAsync(1));
    }

    [Fact]
    public async Task GetBookStatisticsAsync_ReturnsCorrectStatistics() {
        // Act
        var result = await _repository.GetBookStatisticsAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.BookId);
        Assert.Equal(2, result.ReviewCount);
        Assert.Equal(4.5, result.AverageRating);
    }
}
