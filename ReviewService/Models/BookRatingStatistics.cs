namespace ReviewService.Models;

public class BookRatingStatistics {
    public int BookId { get; set; }
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
}
