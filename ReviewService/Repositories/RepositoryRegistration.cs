namespace ReviewService.Repositories;

using Event;
using Review;
using Book;

public static class RepositoryRegistration {
    public static void AddRepositories(this IServiceCollection services) {
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IBookRepository, BookRepository>();
    }
}
