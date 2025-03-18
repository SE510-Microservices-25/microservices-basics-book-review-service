namespace ReviewService.Models;

using System.ComponentModel.DataAnnotations;

public class Review {
    public int Id { get; set; }
    public int BookId { get; set; }
    
    [MaxLength(100)]
    public string? ReviewerName { get; set; }
    
    [MaxLength(1000)]
    public string? Content { get; set; }
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
