namespace ReviewService.Tests.Repositories.Event;

using Microsoft.EntityFrameworkCore;
using Xunit;
using System;
using System.Threading.Tasks;

using Data;
using Models;
using ReviewService.Repositories.Event;

public class EventRepositoryTests {
    private readonly ReviewDbContext _context;
    private readonly EventRepository _repository;

    public EventRepositoryTests() {
        var options = new DbContextOptionsBuilder<ReviewDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            
        _context = new ReviewDbContext(options);
        SeedDatabase();
        
        _repository = new EventRepository(_context);
    }

    private void SeedDatabase() {
        var processedEvents = new[] {
            new ProcessedEvent { 
                Id = 1, 
                EventId = 1, 
                EventType = "TestEvent1", 
                ProcessedAt = DateTime.UtcNow.AddHours(-1) 
            },
            new ProcessedEvent { 
                Id = 2, 
                EventId = 2, 
                EventType = "TestEvent2", 
                ProcessedAt = DateTime.UtcNow.AddMinutes(-30) 
            }
        };
        
        _context.ProcessedEvents.AddRange(processedEvents);
        _context.SaveChanges();
    }

    [Fact]
    public async Task ExistsAsync_ExistingEvent_ReturnsTrue() {
        // Act
        var result = await _repository.ExistsAsync(1, "TestEvent1");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_NonExistentEvent_ReturnsFalse() {
        // Act
        var result = await _repository.ExistsAsync(999, "NonExistentEvent");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CreateAsync_AddsNewProcessedEvent() {
        // Arrange
        var newEvent = new ProcessedEvent {
            EventId = 3,
            EventType = "NewTestEvent",
            ProcessedAt = DateTime.UtcNow
        };

        // Act
        await _repository.CreateAsync(newEvent);

        // Assert
        var savedEvent = await _context.ProcessedEvents
            .FirstOrDefaultAsync(e => e.EventId == newEvent.EventId && e.EventType == newEvent.EventType);
            
        Assert.NotNull(savedEvent);
        Assert.Equal(newEvent.EventId, savedEvent.EventId);
        Assert.Equal(newEvent.EventType, savedEvent.EventType);
    }
}
