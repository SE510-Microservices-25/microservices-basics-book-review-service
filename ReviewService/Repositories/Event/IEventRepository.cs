namespace ReviewService.Repositories.Event;

using Models;

public interface IEventRepository {
    Task<bool> ExistsAsync(int eventId, string eventType);
    Task CreateAsync(ProcessedEvent processedEvent);
}
