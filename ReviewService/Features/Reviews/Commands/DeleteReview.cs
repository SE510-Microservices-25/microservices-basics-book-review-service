namespace ReviewService.Features.Reviews.Commands;

using MediatR;

using Models.DTOs;
using Repositories.Review;
using Services.MQ;

public class DeleteReviewHandler(IReviewRepository repository, IMessageBusService messageBus) : IRequestHandler<DeleteReviewCommand, bool> {
    public async Task<bool> Handle(DeleteReviewCommand request, CancellationToken cancellationToken) {
        var review = await repository.GetByIdAsync(request.Id);
        if (review == null)
            return false;
        
        var result = await repository.DeleteAsync(request.Id);
        
        if (result) {
            await messageBus.PublishReviewDeleted(request.Id, review.BookId);
            var statistics = await repository.GetBookStatisticsAsync(review.BookId);
            await messageBus.PublishBookRatingChanged(statistics);
        }
        
        return result;
    }
}
