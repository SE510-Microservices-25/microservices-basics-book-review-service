namespace BookService.Models;

using System.ComponentModel.DataAnnotations;

public class Book {
    public int Id { get; set; }
    
    [MaxLength(100)]
    public string? Title { get; set; }
    
    [MaxLength(100)]
    public string? Author { get; set; }
    
    [MaxLength(50)]
    public string? Genre { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
