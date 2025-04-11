namespace ReviewService.Repositories.Review;

using Microsoft.EntityFrameworkCore;

using Data;
using Models;
using Models.DTOs;

public class ReviewRepository(ReviewDbContext context) : IReviewRepository {
    public async Task<IEnumerable<Review>> GetAllAsync() {
        return await context.Reviews.ToListAsync();
    }

    public async Task<Review?> GetByIdAsync(int id) {
        return await context.Reviews.FindAsync(id);
    }

    public async Task<IEnumerable<Review>> GetByBookIdAsync(int bookId) {
        return await context.Reviews.Where(r => r.BookId == bookId).ToListAsync();
    }

    public async Task<Review> CreateAsync(Review review) {
        context.Reviews.Add(review);
        await context.SaveChangesAsync();
        return review;
    }

    public async Task<Review?> UpdateAsync(UpdateReviewCommand request) {
        var existingReview = await context.Reviews.FindAsync(request.Id);
        if (existingReview == null)
            return null;

        existingReview.ReviewerName = request.ReviewerName;
        existingReview.Content = request.Content;
        existingReview.Rating = request.Rating;

        await context.SaveChangesAsync();
        return existingReview;
    }

    public async Task<bool> DeleteAsync(int id) {
        var review = await context.Reviews.FindAsync(id);
        if (review == null)
            return false;

        context.Reviews.Remove(review);
        await context.SaveChangesAsync();
        return true;
    }
    
    public async Task<BookRatingStatistics> GetBookStatisticsAsync(int bookId) {
        var reviews = await context.Reviews.Where(r => r.BookId == bookId).ToListAsync();
        
        return new BookRatingStatistics {
            BookId = bookId,
            ReviewCount = reviews.Count,
            AverageRating = reviews.Count > 0 ? Math.Round(reviews.Average(r => r.Rating), 2) : 0
        };
    }
    
    public async Task<IEnumerable<BookRatingStatistics>> GetAllBooksStatisticsAsync() {
        return await context.Reviews
            .GroupBy(r => r.BookId)
            .Select(g => new BookRatingStatistics {
                BookId = g.Key,
                ReviewCount = g.Count(),
                AverageRating = Math.Round(g.Average(r => r.Rating), 2)
            })
            .ToListAsync();
    }
}
