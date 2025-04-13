namespace ReviewService.Services.MQ;

using Microsoft.Extensions.Logging;
using System.Diagnostics;

using Repositories.Outbox;
using Contracts;
using Models;

public class MessageBusService(IOutboxRepository outboxRepository, ILogger<MessageBusService> logger, IConfiguration configuration) : IMessageBusService {
    private async Task PublishWithOutbox<T>(T message, string messageId) where T : class {
        var messageType = typeof(T).Name;
    
        using var activity = new Activity("PublishEvent")
            .SetTag("message.type", messageType)
            .SetTag("message.id", messageId)
            .Start();

        try {
            logger.LogInformation("Publishing {MessageType} with ID: {MessageId}", messageType, messageId);
            var headers = new Dictionary<string, object> {
                { "ServiceAuthentication", configuration["MessageBus:ServiceSecretKey"] ?? "default-key" }
            };
            await outboxRepository.AddAsync(message, headers);
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
        await PublishWithOutbox(review, messageId);
    }

    public async Task PublishReviewUpdated(Review review) {
        var messageId = $"ReviewUpdated-{review.Id}";
        await PublishWithOutbox(review, messageId);
    }

    public async Task PublishReviewDeleted(int id, int bookId) {
        var messageId = $"ReviewDeleted-{id}";
        await PublishWithOutbox(new ReviewDeleted(id, bookId), messageId);
    }

    public async Task PublishBookRatingChanged(BookRatingStatistics statistics) {
        var messageId = $"BookRatingChanged-{statistics.BookId}";
        await PublishWithOutbox(statistics, messageId);
    }
}
