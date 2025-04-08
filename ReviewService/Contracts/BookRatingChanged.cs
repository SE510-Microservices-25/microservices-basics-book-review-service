namespace ReviewService.Contracts;

using Models;

public record BookRatingChanged(int BookId, double AverageRating, int ReviewCount) {
    public static BookRatingChanged FromStatistics(BookRatingStatistics statistics) {
        return new BookRatingChanged(
            statistics.BookId,
            statistics.AverageRating,
            statistics.ReviewCount
        );
    }
}
