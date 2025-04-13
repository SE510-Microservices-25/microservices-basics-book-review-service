namespace ReviewService.Contracts;

public record BookCreated(int Id, string Title, string Author, string Genre, DateTime CreatedAt);
