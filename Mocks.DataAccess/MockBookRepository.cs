using Application.Api;
using Domain.Entities;

namespace Mocks.DataAccess;

public class MockBookRepository : IBooksRepository
{
  private readonly List<Book> _books = new()
    {
        new Book { Id = Guid.NewGuid(), Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", Genre = "Classic" },
        new Book { Id = Guid.NewGuid(), Title = "To Kill a Mockingbird", Author = "Harper Lee", Genre = "Fiction" },
        new Book { Id = Guid.NewGuid(), Title = "1984", Author = "George Orwell", Genre = "Dystopian" },
        new Book { Id = Guid.NewGuid(), Title = "Moby-Dick", Author = "Herman Melville", Genre = "Adventure" },
        new Book { Id = Guid.NewGuid(), Title = "Pride and Prejudice", Author = "Jane Austen", Genre = "Romance" },
        new Book { Id = Guid.NewGuid(), Title = "The Catcher in the Rye", Author = "J.D. Salinger", Genre = "Coming-of-Age" }
    };

  public Task<List<Book>> GetAllAsync()
  {
    return Task.FromResult(_books);
  }

  public Task<Book?> GetByIdAsync(Guid id)
  {
    var book = _books.FirstOrDefault(b => b.Id == id);
    return Task.FromResult(book);
  }

  public Task AddAsync(Book book)
  {
    return Task.CompletedTask;
  }

  public Task UpdateAsync(Book book)
  {
    return Task.CompletedTask;
  }

  public Task DeleteAsync(Guid id)
  {
    return Task.CompletedTask;
  }
}
