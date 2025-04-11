namespace ReviewService.Repositories.Review;

using Models;
using Models.DTOs;

public interface IReviewRepository {
    Task<IEnumerable<Review>> GetAllAsync();
    Task<Review?> GetByIdAsync(int id);
    Task<IEnumerable<Review>> GetByBookIdAsync(int bookId);
    Task<Review> CreateAsync(Review review);
    Task<Review?> UpdateAsync(UpdateReviewCommand request);
    Task<bool> DeleteAsync(int id);
    
    Task<BookRatingStatistics> GetBookStatisticsAsync(int bookId);
    Task<IEnumerable<BookRatingStatistics>> GetAllBooksStatisticsAsync();
}
