// BookService/Consumers/ReviewMessageLogConsumer.cs
namespace BookService.Consumers;

using MassTransit;
using Microsoft.Extensions.Logging;

public class ReviewMessageLogConsumer(ILogger<ReviewMessageLogConsumer> logger) : IConsumer<object> {
    public Task Consume(ConsumeContext<object> context) {
        var messageType = context.Message.GetType().Name;
        var messageId = context.MessageId;
        logger.LogInformation("Received message: {MessageType} with ID {MessageId}", messageType, messageId);
        return Task.CompletedTask;
    }
}
