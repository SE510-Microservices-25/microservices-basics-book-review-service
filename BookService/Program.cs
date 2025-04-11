using Microsoft.EntityFrameworkCore;
using MassTransit;

using BookService.Data;
using BookService.Repositories;
using BookService.Services.MQ;
using BookService.Services.Sync;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BookDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IMessageBusService, MessageBusService>();
builder.Services.AddScoped<InitialDataSyncService>();

builder.Services.AddMassTransit(x => {
    x.UsingRabbitMq((_, cfg) => {
        var rabbitMqHost = builder.Configuration["RabbitMQ:Host"] ?? "rabbitmq";
        var rabbitMqUsername = builder.Configuration["RabbitMQ:Username"] ?? "guest";
        var rabbitMqPassword = builder.Configuration["RabbitMQ:Password"] ?? "guest";

        cfg.Host(rabbitMqHost, h => {
            h.Username(rabbitMqUsername);
            h.Password(rabbitMqPassword);
        });
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope()) {
    var syncService = scope.ServiceProvider.GetRequiredService<InitialDataSyncService>();
    await syncService.SyncInitialDataAsync();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
