namespace ReviewService.Repositories.Book;

using Models;

public interface IBookRepository {
    Task<bool> ExistsAsync(int id);
    Task<Book?> GetByIdAsync(int id);
    Task AddAsync(Book book);
    Task<bool> DeleteAsync(int id);
}
