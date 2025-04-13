namespace BookService.Consumers;

using MassTransit;
using Microsoft.Extensions.Logging;

using ReviewService.Contracts;

public class ReviewMessageLogConsumer(ILogger<ReviewMessageLogConsumer> logger) : IConsumer<IReviewEvent> {   
    public Task Consume(ConsumeContext<IReviewEvent> context) {
        var messageType = context.Message.GetType().Name;
        var messageId = context.MessageId;
        logger.LogInformation("Received message: {MessageType} with ID {MessageId}", messageType, messageId);
        return Task.CompletedTask;
    }
}
