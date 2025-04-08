namespace ReviewService.Services;

using MassTransit;

using Contracts;
using Models;

public class MessageBusService(IBus bus) {
    public async Task PublishReviewCreated(Review review) {
        await bus.Publish(new ReviewCreated(
            review.Id, 
            review.BookId, 
            review.ReviewerName, 
            review.Content, 
            review.Rating, 
            review.CreatedAt));
    }

    public async Task PublishReviewUpdated(Review review) {
        await bus.Publish(new ReviewUpdated(
            review.Id, 
            review.BookId, 
            review.ReviewerName, 
            review.Content, 
            review.Rating));
    }

    public async Task PublishReviewDeleted(int id, int bookId) {
        await bus.Publish(new ReviewDeleted(id, bookId));
    }

    public async Task PublishBookRatingChanged(BookRatingStatistics statistics) {
        await bus.Publish(new BookRatingChanged(
            statistics.BookId, 
            statistics.AverageRating, 
            statistics.ReviewCount));
    }
}
