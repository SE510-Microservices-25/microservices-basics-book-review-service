namespace BookService.Services.MQ;

using MassTransit;
using Microsoft.Extensions.Configuration;

public static class MessageBusExtensions {
    public static void AddServiceAuthentication(this SendContext context, IConfiguration configuration) {
        var serviceSecretKey = configuration["MessageBus:ServiceSecretKey"] ?? "default-key";
        context.Headers.Set("ServiceAuthentication", serviceSecretKey);
    }
}
