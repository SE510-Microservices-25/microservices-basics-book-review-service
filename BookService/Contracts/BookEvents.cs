namespace BookService.Contracts;

using Models;

public record BookCreated(int Id, string Title, string Author, string Genre, DateTime CreatedAt) {
    public static BookCreated FromBook(Book book) {
        return new BookCreated(
            book.Id,
            book.Title ?? string.Empty,
            book.Author ?? string.Empty,
            book.Genre ?? string.Empty,
            book.CreatedAt
        );
    }
}

public record BookDeleted(int Id);
