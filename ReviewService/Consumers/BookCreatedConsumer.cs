namespace ReviewService.Consumers;

using MassTransit;
using Microsoft.Extensions.Logging;

using Contracts;
using Services.Event;

public class BookCreatedConsumer(ILogger<BookCreatedConsumer> logger, 
    IEventProcessingService eventProcessingService, IHttpContextAccessor httpContextAccessor) : IConsumer<BookCreated> {
    public async Task Consume(ConsumeContext<BookCreated> context) {
        var user = httpContextAccessor.HttpContext?.User;
        var isAuthenticated = user?.Identity?.IsAuthenticated ?? false;
        if (!isAuthenticated) {
            logger.LogWarning("Unauthorized access attempt to BookCreatedConsumer");
            return;
        }

        var (id, title, author) = context.Message;
        const string eventType = nameof(BookCreated);
        if (await eventProcessingService.IsEventProcessedAsync(id, eventType)) {
            return;
        }

        logger.LogInformation("Book created: {Title} by {Author}", title, author);
        await eventProcessingService.MarkEventAsProcessedAsync(id, eventType);
    }
}
