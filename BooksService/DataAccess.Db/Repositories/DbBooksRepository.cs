using Application.Api;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Db.DataAccess.Repositories;

public class DbBooksRepository : IBooksRepository
{
  private readonly ServiceDbContext _dbContext;

  public DbBooksRepository(ServiceDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public async Task AddAsync(Book book)
  {
    await _dbContext.Books.AddAsync(book);
    await _dbContext.SaveChangesAsync();
  }

  public async Task DeleteAsync(Guid id)
  {
    var book = await _dbContext.Books.FindAsync(id);
    if (book == null) return;
    _dbContext.Books.Remove(book);
    await _dbContext.SaveChangesAsync();
  }

  public async Task<List<Book>> GetAllAsync()
  {
    return await _dbContext.Books.ToListAsync();
  }

  public async Task<Book?> GetByIdAsync(Guid id)
  {
    return await _dbContext.Books.FindAsync(id);
  }

  public async Task UpdateAsync(Book book)
  {
    _dbContext.Books.Update(book);
    await _dbContext.SaveChangesAsync();
  }
}