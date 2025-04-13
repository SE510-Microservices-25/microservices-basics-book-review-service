namespace ReviewService.Models;

using System.ComponentModel.DataAnnotations;

public class OutboxMessage {
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string MessageType { get; set; }
    
    [Required]
    public string Payload { get; set; }
    
    public string Headers { get; set; } = "{}";
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? ProcessedAt { get; set; }
    
    public bool Processed => ProcessedAt.HasValue;
}
