namespace ReviewService.Services;

using Event;
using MQ;

public static class ServiceRegistration {
    public static void AddServices(this IServiceCollection services) {
        services.AddScoped<IEventProcessingService, EventProcessingService>();
        services.AddScoped<MessageBusService>();
    }
}
