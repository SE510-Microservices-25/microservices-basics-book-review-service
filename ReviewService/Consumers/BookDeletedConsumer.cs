namespace ReviewService.Consumers;

using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

using Contracts;
using Services.Event;
using Repositories.Review;
using Repositories.Book;

public class BookDeletedConsumer(ILogger<BookDeletedConsumer> logger, IEventProcessingService eventProcessingService,
    IReviewRepository reviewRepository, IBookRepository bookRepository, IConfiguration configuration) : IConsumer<BookDeleted> {
    private readonly string _serviceSecretKey = configuration["MessageBus:ServiceSecretKey"] ?? "default-key";

    public async Task Consume(ConsumeContext<BookDeleted> context) {
        var authHeader = context.Headers.Get<string>("ServiceAuthentication");
        
        if (string.IsNullOrEmpty(authHeader) || authHeader != _serviceSecretKey) {
            logger.LogWarning("Unauthorized access attempt to BookDeletedConsumer");
            return;
        }

        var bookId = context.Message.Id;
        const string eventType = nameof(BookDeleted);
        
        if (await eventProcessingService.IsEventProcessedAsync(bookId, eventType)) {
            return;
        }

        var deletedCount = await reviewRepository.DeleteByBookIdAsync(bookId);
        logger.LogInformation("Deleted {Count} reviews for book {BookId}", deletedCount, bookId);

        await bookRepository.DeleteAsync(bookId);
        await eventProcessingService.MarkEventAsProcessedAsync(bookId, eventType);
    }
}
