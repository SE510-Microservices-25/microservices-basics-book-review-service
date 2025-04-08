namespace ReviewService.Services.MQ;

using Models;

public interface IMessageBusService {
    Task PublishReviewCreated(Review review);
    Task PublishReviewUpdated(Review review);
    Task PublishReviewDeleted(int id, int bookId);
    Task PublishBookRatingChanged(BookRatingStatistics statistics);
}
