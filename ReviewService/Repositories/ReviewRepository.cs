using Microsoft.EntityFrameworkCore;
using ReviewService.Data;
using ReviewService.Models;

namespace ReviewService.Repositories;

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
}
