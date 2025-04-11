namespace BookService.Repositories;

using Models;

public interface IBookRepository {
    Task<IEnumerable<Book>> GetAllAsync();
    Task<Book?> GetByIdAsync(int id);
    Task<Book> CreateAsync(Book book);
    Task<bool> DeleteAsync(int id);
}
