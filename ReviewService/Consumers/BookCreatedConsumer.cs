namespace ReviewService.Consumers;

using MassTransit;
using Microsoft.Extensions.Logging;

// test class if other services are not available
public record BookCreated(int Id, string Title, string Author);

public class BookCreatedConsumer(ILogger<BookCreatedConsumer> logger) : IConsumer<BookCreated> {
    public Task Consume(ConsumeContext<BookCreated> context) {
        logger.LogInformation("Book created: {Title} by {Author}", context.Message.Title, context.Message.Author);
        return Task.CompletedTask;
    }
}
