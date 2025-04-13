namespace ReviewService.Models;

using Contracts;

public class BookRatingStatistics : IReviewEvent {
    public int BookId { get; set; }
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
}
