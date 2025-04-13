namespace BookService.Contracts;

public record BookCreated(int Id, string Title, string Author, string Genre, DateTime CreatedAt);
public record BookDeleted(int Id);
