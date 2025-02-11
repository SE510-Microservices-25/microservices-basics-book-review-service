using Domain.Entities;

namespace Application.Api;

public interface IBooksRepository
{
  Task<Book?> GetByIdAsync(Guid id);
  Task<List<Book>> GetAllAsync();
  Task AddAsync(Book book);
  Task UpdateAsync(Book book);
  Task DeleteAsync(Guid id);
}