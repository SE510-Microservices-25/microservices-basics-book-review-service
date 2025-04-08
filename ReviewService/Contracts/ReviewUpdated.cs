namespace ReviewService.Contracts;

public record ReviewUpdated(int Id, int BookId, string ReviewerName, string Content, int Rating);
