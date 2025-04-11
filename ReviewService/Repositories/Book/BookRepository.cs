namespace ReviewService.Repositories.Book;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Data;
using Models;

public class BookRepository(ReviewDbContext context, ILogger<BookRepository> logger) : IBookRepository {
    public async Task<bool> ExistsAsync(int id) {
        return await context.Books.AnyAsync(b => b.Id == id);
    }

    public async Task<Book?> GetByIdAsync(int id) {
        return await context.Books.FindAsync(id);
    }

    public async Task AddAsync(Book book) {
        var existingBook = await context.Books.FindAsync(book.Id);
        if (existingBook != null) {
            logger.LogInformation("Book already exists in local cache: {Title} (ID: {Id})", book.Title, book.Id);
            return;
        }
        context.Books.Add(book);
        await context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id) {
        var book = await context.Books.FindAsync(id);
        if (book == null)
            return false;
            
        context.Books.Remove(book);
        await context.SaveChangesAsync();
        logger.LogInformation("Removed book from local cache: ID: {Id}", id);
        return true;
    }
}
