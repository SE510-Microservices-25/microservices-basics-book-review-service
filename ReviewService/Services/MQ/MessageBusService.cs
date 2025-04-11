namespace ReviewService.Services.MQ;

using MassTransit;
using Microsoft.Extensions.Logging;
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

    public async Task PublishReviewCreated(Review review) {
        var messageId = $"ReviewCreated-{review.Id}";
        await PublishWithLogging(review, messageId);
    }

    public async Task PublishReviewUpdated(Review review) {
        var messageId = $"ReviewUpdated-{review.Id}";
        await PublishWithLogging(review, messageId);
    }

    public async Task PublishReviewDeleted(int id, int bookId) {
        var messageId = $"ReviewDeleted-{id}";
        await PublishWithLogging(new ReviewDeleted(id, bookId), messageId);
    }

    public async Task PublishBookRatingChanged(BookRatingStatistics statistics) {
        var messageId = $"BookRatingChanged-{statistics.BookId}";
        await PublishWithLogging(statistics, messageId);
    }
}
