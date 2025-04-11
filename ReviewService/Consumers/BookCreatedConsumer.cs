namespace ReviewService.Consumers;

using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

using Contracts;
using Services.Event;

public class BookCreatedConsumer(ILogger<BookCreatedConsumer> logger, 
    IEventProcessingService eventProcessingService, IConfiguration configuration) : IConsumer<BookCreated> {
    private readonly string _serviceSecretKey = configuration["MessageBus:ServiceSecretKey"] ?? "default-key";

    public async Task Consume(ConsumeContext<BookCreated> context) {
        var authHeader = context.Headers.Get<string>("ServiceAuthentication");
        
        if (string.IsNullOrEmpty(authHeader) || authHeader != _serviceSecretKey) {
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
