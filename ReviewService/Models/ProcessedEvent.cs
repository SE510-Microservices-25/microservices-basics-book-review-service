namespace ReviewService.Models;

using System.ComponentModel.DataAnnotations;

public class ProcessedEvent {
    public int Id { get; set; }
    
    [Required]
    public int EventId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string EventType { get; set; }
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}
