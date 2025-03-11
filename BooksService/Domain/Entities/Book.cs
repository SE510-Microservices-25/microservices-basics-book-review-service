namespace Domain.Entities;

public class Book {
  public Guid Id { get; init; }
  public string? Title { get; init; }
  public string? Author { get; init; }
  public string? Genre { get; init; }
}