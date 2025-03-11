using DataAccess.Db.Configurations;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Db.DataAccess;

public class ServiceDbContext : DbContext
{
  public ServiceDbContext(DbContextOptions<ServiceDbContext> options) : base(options) { }

  public DbSet<Book> Books { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfiguration<Book>(new BookConfiguration());
    base.OnModelCreating(modelBuilder);
  }
}