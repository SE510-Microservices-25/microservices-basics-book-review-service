using Microsoft.EntityFrameworkCore;
using ReviewService.Data;
using ReviewService.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ReviewDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope()) {
    var dbContext = scope.ServiceProvider.GetRequiredService<ReviewDbContext>();
    dbContext.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
