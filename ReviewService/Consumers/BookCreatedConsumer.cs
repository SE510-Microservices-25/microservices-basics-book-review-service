namespace ReviewService.Consumers;

using MassTransit;
using Microsoft.Extensions.Logging;

using Contracts;
using Services.Event;

public class BookCreatedConsumer(ILogger<BookCreatedConsumer> logger, IEventProcessingService eventProcessingService) : IConsumer<BookCreated> {
    public async Task Consume(ConsumeContext<BookCreated> context) {
        var (id, title, author) = context.Message;
        const string eventType = nameof(BookCreated);
        if (await eventProcessingService.IsEventProcessedAsync(id, eventType)) {
            return;
        }

        logger.LogInformation("Book created: {Title} by {Author}", title, author);
        await eventProcessingService.MarkEventAsProcessedAsync(id, eventType);
    }
}
