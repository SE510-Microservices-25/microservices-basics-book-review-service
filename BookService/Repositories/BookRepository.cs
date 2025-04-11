namespace BookService.Repositories;

using Microsoft.EntityFrameworkCore;

using Data;
using Models;

public class BookRepository(BookDbContext context) : IBookRepository {
    public async Task<IEnumerable<Book>> GetAllAsync() {
        return await context.Books.ToListAsync();
    }

    public async Task<Book?> GetByIdAsync(int id) {
        return await context.Books.FindAsync(id);
    }

    public async Task<Book> CreateAsync(Book book) {
        book.CreatedAt = DateTime.UtcNow;
        context.Books.Add(book);
        await context.SaveChangesAsync();
        return book;
    }

    public async Task<Book?> UpdateAsync(int id, Book book) {
        var existingBook = await context.Books.FindAsync(id);
        if (existingBook == null)
            return null;

        existingBook.Title = book.Title;
        existingBook.Author = book.Author;
        existingBook.Genre = book.Genre;

        await context.SaveChangesAsync();
        return existingBook;
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
