namespace ReviewService.Services;

using Event;
using MQ;
using Outbox;

public static class ServiceRegistration {
    public static void AddServices(this IServiceCollection services) {
        services.AddScoped<IEventProcessingService, EventProcessingService>();
        services.AddScoped<IMessageBusService, MessageBusService>();
        services.AddHostedService<OutboxProcessor>();
    }
}
