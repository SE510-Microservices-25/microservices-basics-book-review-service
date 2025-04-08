namespace ReviewService.Contracts;

public record BookRatingChanged(int BookId, double AverageRating, int ReviewCount);
