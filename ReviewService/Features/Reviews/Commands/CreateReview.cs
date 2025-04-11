namespace ReviewService.Features.Reviews.Commands;

using MediatR;

using Models;
using Models.DTOs;  
using Services.MQ;
using Repositories.Review;
using Repositories.Book;

public class CreateReviewHandler(IReviewRepository repository, IMessageBusService messageBus,
    IBookRepository bookRepository) : IRequestHandler<CreateReviewCommand, Review> {
    
    public async Task<Review> Handle(CreateReviewCommand request, CancellationToken cancellationToken) {
        if (!await bookRepository.ExistsAsync(request.BookId)) {
            throw new InvalidOperationException($"Cannot create review for non-existent book: BookId = {request.BookId}");
        }
        
        var review = new Review {
            BookId = request.BookId,
            ReviewerName = request.ReviewerName,
            Content = request.Content,
            Rating = request.Rating,
            CreatedAt = DateTime.UtcNow
        };

        var createdReview = await repository.CreateAsync(review);
        await messageBus.PublishReviewCreated(createdReview);
        
        var statistics = await repository.GetBookStatisticsAsync(createdReview.BookId);
        await messageBus.PublishBookRatingChanged(statistics);
        
        return createdReview;
    }
}
