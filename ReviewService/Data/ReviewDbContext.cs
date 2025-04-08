namespace ReviewService.Data;

using Microsoft.EntityFrameworkCore;

using Models;

public class ReviewDbContext(DbContextOptions<ReviewDbContext> options) : DbContext(options) {
    public DbSet<Review> Reviews { get; set; }
    public DbSet<ProcessedEvent> ProcessedEvents { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        ConfigureData(modelBuilder);
        SeedData(modelBuilder);
    }

    private static void ConfigureData(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Review>()
            .HasKey(r => r.Id);
        
        modelBuilder.Entity<ProcessedEvent>()
            .HasKey(e => e.Id);
        
        modelBuilder.Entity<ProcessedEvent>()
            .HasIndex(e => new { e.EventId, e.EventType })
            .IsUnique();
    }

    private static void SeedData(ModelBuilder modelBuilder) {
        var fixedDate = new DateTime(2025, 3, 4, 16, 0, 0, DateTimeKind.Utc);
    
        modelBuilder.Entity<Review>().HasData(
            new Review { Id = 1, BookId = 1, ReviewerName = "Alice", Content = "Great book!", Rating = 5, CreatedAt = fixedDate },
            new Review { Id = 2, BookId = 1, ReviewerName = "Bob", Content = "I didn't like it", Rating = 2, CreatedAt = fixedDate },
            new Review { Id = 3, BookId = 2, ReviewerName = "Charlie", Content = "It was okay", Rating = 3, CreatedAt = fixedDate }
        );
    }
}
