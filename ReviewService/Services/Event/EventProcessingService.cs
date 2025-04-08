namespace ReviewService.Services.Event;

using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

using Models;
using Repositories.Event;

public class EventProcessingService(IEventRepository eventRepository, ILogger<EventProcessingService> logger) : IEventProcessingService {
    public async Task<bool> IsEventProcessedAsync(int eventId, string eventType) {
        var isProcessed = await eventRepository.ExistsAsync(eventId, eventType);
        if (isProcessed) {
            logger.LogInformation("Event {EventType} with ID {EventId} has already been processed", eventType, eventId);
        }
        
        return isProcessed;
    }

    public async Task MarkEventAsProcessedAsync(int eventId, string eventType) {
        try {
            var processedEvent = new ProcessedEvent {
                EventId = eventId,
                EventType = eventType,
                ProcessedAt = DateTime.UtcNow
            };

            await eventRepository.CreateAsync(processedEvent);
            logger.LogInformation("Event {EventType} with ID {EventId} marked as processed", eventType, eventId);
        } catch (Exception ex) {
            logger.LogError(ex, "Error marking event {EventType} with ID {EventId} as processed", eventType, eventId);
            throw;
        }
    }
}
