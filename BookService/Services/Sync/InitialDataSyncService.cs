namespace BookService.Services.Sync;

using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using Data;
using MQ;

public class InitialDataSyncService(BookDbContext dbContext, IMessageBusService messageBus, ILogger<InitialDataSyncService> logger) {
    public async Task SyncInitialDataAsync() {
        logger.LogInformation("Starting initial data synchronization");
        var books = await dbContext.Books.ToListAsync();
        
        foreach (var book in books) {
            logger.LogInformation("Publishing initial book: {Title} (ID: {Id})", book.Title, book.Id);
            await messageBus.PublishBookCreated(book);
        }
        
        logger.LogInformation("Initial data synchronization completed: {Count} books published", books.Count);
    }
}
