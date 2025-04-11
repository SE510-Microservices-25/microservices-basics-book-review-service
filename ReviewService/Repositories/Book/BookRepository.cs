namespace ReviewService.Repositories.Book;

using Microsoft.EntityFrameworkCore;

using Data;
using Models;

public class BookRepository(ReviewDbContext context) : IBookRepository {
    public async Task<bool> ExistsAsync(int id) {
        return await context.Books.AnyAsync(b => b.Id == id);
    }

    public async Task<Book?> GetByIdAsync(int id) {
        return await context.Books.FindAsync(id);
    }

    public async Task AddAsync(Book book) {
        var existingBook = await context.Books.FindAsync(book.Id);
        if (existingBook != null) {
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
        return true;
    }
}
