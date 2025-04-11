namespace ReviewService.Features.Reviews.Commands;

using MediatR;

using Models;
using Models.DTOs;
using Services.MQ;
using Repositories.Review;

public class UpdateReviewHandler(IReviewRepository repository, IMessageBusService messageBus) : IRequestHandler<UpdateReviewCommand , Review?> {
    public async Task<Review?> Handle(UpdateReviewCommand request, CancellationToken cancellationToken) {
        var updatedReview = await repository.UpdateAsync(request);
        if (updatedReview != null) {
            await messageBus.PublishReviewUpdated(updatedReview);
            var statistics = await repository.GetBookStatisticsAsync(updatedReview.BookId);
            await messageBus.PublishBookRatingChanged(statistics);
        }

        return updatedReview;
    }
}
