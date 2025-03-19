namespace ReviewService.Repositories;

using Microsoft.EntityFrameworkCore;

using Data;
using Models;

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

    public async Task<bool> UpdateAsync(int id, Review review) {
        var existingReview = await context.Reviews.FindAsync(id);
        if (existingReview == null)
            return false;

        existingReview.BookId = review.BookId;
        existingReview.ReviewerName = review.ReviewerName;
        existingReview.Content = review.Content;
        existingReview.Rating = review.Rating;

        await context.SaveChangesAsync();
        return true;
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
