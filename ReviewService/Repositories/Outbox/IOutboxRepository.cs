namespace ReviewService.Repositories.Outbox;

using Models;

public interface IOutboxRepository {
    Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(int batchSize = 10);
    Task MarkAsProcessedAsync(int id);
    Task AddAsync<T>(T message, Dictionary<string, object>? headers) where T : class;
    Task SaveChangesAsync();
}
