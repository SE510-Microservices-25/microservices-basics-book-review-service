namespace ReviewService.Repositories.Outbox;

using System.Text.Json;
using Microsoft.EntityFrameworkCore;

using Data;
using Models;

public class OutboxRepository(ReviewDbContext context) : IOutboxRepository {
    
    public async Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(int batchSize = 10) {
        return await context.OutboxMessages
            .Where(m => m.ProcessedAt == null)
            .OrderBy(m => m.CreatedAt)
            .Take(batchSize)
            .ToListAsync();
    }
    
    public async Task MarkAsProcessedAsync(int id) {
        var message = await context.OutboxMessages.FindAsync(id);
        if (message != null) {
            message.ProcessedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();
        }
    }
    
    public async Task AddAsync<T>(T message, string messageType, string secretKey) where T : class {
        var outboxMessage = new OutboxMessage {
            MessageType = messageType,
            Payload = JsonSerializer.Serialize(message),
            CreatedAt = DateTime.UtcNow,
            Secret = secretKey
        };
        
        context.OutboxMessages.Add(outboxMessage);
        await context.SaveChangesAsync();
    }
}
