namespace ReviewService.Services.Outbox;

using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MassTransit;

using Repositories.Outbox;

public class OutboxProcessor(IServiceProvider serviceProvider, ILogger<OutboxProcessor> logger) : BackgroundService {
    
    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(5);
    private const int BatchSize = 50;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        logger.LogInformation("Outbox processor started");
        
        while (!stoppingToken.IsCancellationRequested) {
            try {
                await ProcessOutboxMessagesAsync(stoppingToken);
                await Task.Delay(Interval, stoppingToken);
            } catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) {
                logger.LogInformation("Outbox processor stopping");
                break;
            } catch (Exception ex) {
                logger.LogError(ex, "Error processing outbox messages");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
    
    private async Task ProcessOutboxMessagesAsync(CancellationToken stoppingToken) {
        using var scope = serviceProvider.CreateScope();
        var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        var bus = scope.ServiceProvider.GetRequiredService<IBus>();
        
        var messages = await outboxRepository.GetUnprocessedMessagesAsync(BatchSize);
        if (messages.Count == 0) return;
        
        foreach (var message in messages) {
            try {
                var messageType = Type.GetType(message.MessageType);
                if (messageType == null) {
                    logger.LogWarning("Cannot find type {Type}", message.MessageType);
                    await outboxRepository.MarkAsProcessedAsync(message.Id);
                    continue;
                }
                
                var messageObject = JsonSerializer.Deserialize(message.Payload, messageType);
                if (messageObject == null) {
                    logger.LogWarning("Cannot deserialize message {Id}", message.Id);
                    await outboxRepository.MarkAsProcessedAsync(message.Id);
                    continue;
                }
                
                await bus.Publish(messageObject, messageType, ctx => {
                    ctx.Headers.Set("ServiceAuthentication", message.Secret);
                }, stoppingToken);
                logger.LogInformation("Published message {Id} of type {Type}", message.Id, messageType);
                await outboxRepository.MarkAsProcessedAsync(message.Id);
            } catch (Exception ex) {
                logger.LogError(ex, "Error processing message {Id}", message.Id);
            }
        }
    }
}
