namespace ReviewService.Contracts;

using Models;

public record ReviewCreated(int Id, int BookId, string ReviewerName, string Content, int Rating, DateTime CreatedAt) {
    public static ReviewCreated FromReview(Review review) {
        return new ReviewCreated(
            review.Id,
            review.BookId,
            review.ReviewerName,
            review.Content,
            review.Rating,
            review.CreatedAt
        );
    }
}
