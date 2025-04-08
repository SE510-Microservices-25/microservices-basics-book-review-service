namespace ReviewService.Services.Event;

public interface IEventProcessingService {
    Task<bool> IsEventProcessedAsync(int eventId, string eventType);
    Task MarkEventAsProcessedAsync(int eventId, string eventType);
}
