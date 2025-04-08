namespace ReviewService.Repositories.Event;

using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using Data;
using Models;

public class EventRepository(ReviewDbContext context) : IEventRepository {
    public async Task<bool> ExistsAsync(int eventId, string eventType) {
        return await context.ProcessedEvents
            .AnyAsync(e => e.EventId == eventId && e.EventType == eventType);
    }

    public async Task CreateAsync(ProcessedEvent processedEvent) {
        context.ProcessedEvents.Add(processedEvent);
        await context.SaveChangesAsync();
    }
}
