namespace ReviewService.Consumers;

using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

using Contracts;
using Services.Event;
using Repositories.Book;
using Models;

public class BookCreatedConsumer(ILogger<BookCreatedConsumer> logger, IEventProcessingService eventProcessingService,
    IBookRepository bookRepository, IConfiguration configuration) : IConsumer<BookCreated> {
    private readonly string _serviceSecretKey = configuration["MessageBus:ServiceSecretKey"] ?? "default-key";

    public async Task Consume(ConsumeContext<BookCreated> context) {
        var authHeader = context.Headers.Get<string>("ServiceAuthentication");
        if (string.IsNullOrEmpty(authHeader) || authHeader != _serviceSecretKey) {
            logger.LogWarning("Unauthorized access attempt to BookCreatedConsumer");
            return;
        }

        var message = context.Message;
        const string eventType = nameof(BookCreated);
        if (await eventProcessingService.IsEventProcessedAsync(message.Id, eventType)) {
            return;
        }

        logger.LogInformation("Book created: {Title} by {Author}", message.Title, message.Author);
        
        await bookRepository.AddAsync(new Book {
            Id = message.Id,
            Title = message.Title,
            Author = message.Author,
            Genre = message.Genre,
            CreatedAt = message.CreatedAt
        });
        await eventProcessingService.MarkEventAsProcessedAsync(message.Id, eventType);
    }
}
