namespace ReviewService.Contracts {
    public interface IReviewEvent;
    public record ReviewDeleted(int Id, int BookId) : IReviewEvent;
}

namespace ReviewService.Models {
    using Contracts;
    
    public record Review(int Id, int BookId, string? ReviewerName, string? Content, int Rating, DateTime CreatedAt) : IReviewEvent;
    public record BookRatingStatistics(int BookId, double AverageRating, int TotalReviews) : IReviewEvent;
}
