namespace BookService.Services.MQ;

using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

using Contracts;
using Models;

public class MessageBusService(IBus bus, ILogger<MessageBusService> logger, IConfiguration configuration) : IMessageBusService {
    private async Task PublishWithLogging<T>(T message, string messageId) where T : class {
        var messageType = typeof(T).Name;
    
        using var activity = new Activity("PublishEvent")
            .SetTag("message.type", messageType)
            .SetTag("message.id", messageId)
            .Start();

        try {
            logger.LogInformation("Publishing {MessageType} with ID: {MessageId}", messageType, messageId);
            await bus.Publish(message, ctx => {
                ctx.AddServiceAuthentication(configuration);
            });
            logger.LogInformation("Successfully published {MessageType}", messageType);
        }
        catch (Exception ex) {
            logger.LogError(ex, "Failed to publish {MessageType} with ID: {MessageId}", messageType, messageId);
            activity.SetTag("error", true);
            throw;
        }
    }

    public async Task PublishBookCreated(Book book) {
        var messageId = $"BookCreated-{book.Id}";
        await PublishWithLogging(BookCreated.FromBook(book), messageId);
    }
    public async Task PublishBookDeleted(int id) {
        var messageId = $"BookDeleted-{id}";
        await PublishWithLogging(new BookDeleted(id), messageId);
    }
}
