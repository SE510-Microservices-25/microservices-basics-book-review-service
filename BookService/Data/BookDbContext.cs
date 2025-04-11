namespace BookService.Data;

using Microsoft.EntityFrameworkCore;

using Models;

public class BookDbContext(DbContextOptions<BookDbContext> options) : DbContext(options) {
    public DbSet<Book> Books { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Book>().HasKey(b => b.Id);
        
        var fixedDate = new DateTime(2025, 3, 4, 16, 0, 0, DateTimeKind.Utc);
        
        modelBuilder.Entity<Book>().HasData(
            new Book { Id = 1, Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", Genre = "Classic", CreatedAt = fixedDate },
            new Book { Id = 2, Title = "To Kill a Mockingbird", Author = "Harper Lee", Genre = "Fiction", CreatedAt = fixedDate },
            new Book { Id = 3, Title = "1984", Author = "George Orwell", Genre = "Dystopian", CreatedAt = fixedDate }
        );
    }
}
