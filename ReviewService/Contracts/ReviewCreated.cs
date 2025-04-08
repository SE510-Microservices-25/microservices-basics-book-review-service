namespace ReviewService.Contracts;

public record ReviewCreated(int Id, int BookId, string ReviewerName, string Content, int Rating, DateTime CreatedAt);
