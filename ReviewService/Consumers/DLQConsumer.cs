namespace ReviewService.Consumers;

using MassTransit;
using Microsoft.Extensions.Logging;
using System.Text.Json;

public class DLQConsumer(ILogger<DLQConsumer> logger) : IConsumer<object> {
    public Task Consume(ConsumeContext<object> context) {
        var messageType = context.Message.GetType().Name;
        var messageId = context.MessageId;
        var payload = JsonSerializer.Serialize(context.Message);
        
        logger.LogError("Dead letter queue message received: {MessageType} with ID {MessageId}. Payload: {Payload}", 
            messageType, messageId, payload);
        return Task.CompletedTask;
    }
}
