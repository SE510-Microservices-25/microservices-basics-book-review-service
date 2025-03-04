using ReviewService.Models;

namespace ReviewService.Repositories;

public interface IReviewRepository {
    Task<IEnumerable<Review>> GetAllAsync();
    Task<Review?> GetByIdAsync(int id);
    Task<IEnumerable<Review>> GetByBookIdAsync(int bookId);
    Task<Review> CreateAsync(Review review);
    Task<bool> UpdateAsync(int id, Review review);
    Task<bool> DeleteAsync(int id);
}
