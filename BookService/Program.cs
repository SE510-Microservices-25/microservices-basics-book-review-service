using Microsoft.EntityFrameworkCore;
using MassTransit;

using BookService.Data;
using BookService.Repositories;
using BookService.Services.MQ;
using BookService.Services.Sync;
using BookService.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BookDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IMessageBusService, MessageBusService>();
builder.Services.AddScoped<InitialDataSyncService>();

builder.Services.AddMassTransit(x => {
    x.SetKebabCaseEndpointNameFormatter();
    x.AddConsumer<ReviewMessageLogConsumer>();
    
    x.UsingRabbitMq((context, cfg) => {
        var host = builder.Configuration["RabbitMQ:Host"] ?? "rabbitmq";
        var username = builder.Configuration["RabbitMQ:Username"] ?? "guest";
        var password = builder.Configuration["RabbitMQ:Password"] ?? "guest";
        
        cfg.Host(host, h => {
            h.Username(username);
            h.Password(password);
        });
        
        cfg.ReceiveEndpoint("book-service-message-logs", e => {
            e.ConfigureConsumer<ReviewMessageLogConsumer>(context);
        });
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope()) {
    var dbContext = scope.ServiceProvider.GetRequiredService<BookDbContext>();
    dbContext.Database.Migrate();
    var syncService = scope.ServiceProvider.GetRequiredService<InitialDataSyncService>();
    await syncService.SyncInitialDataAsync();
}

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
