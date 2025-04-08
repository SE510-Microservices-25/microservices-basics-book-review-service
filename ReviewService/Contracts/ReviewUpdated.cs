namespace ReviewService.Contracts;

using Models;

public record ReviewUpdated(int Id, int BookId, string ReviewerName, string Content, int Rating) {
    public static ReviewUpdated FromReview(Review review) {
        return new ReviewUpdated(
            review.Id,
            review.BookId,
            review.ReviewerName,
            review.Content,
            review.Rating
        );
    }
}
