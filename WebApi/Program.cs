using Application.Api;
using Db.DataAccess;
using Db.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");

builder.Services.AddDbContext<ServiceDbContext>(options =>
    {
        options.UseMySQL(connectionString!);
    }
);

builder.Services.AddScoped<IBooksRepository, DbBooksRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();