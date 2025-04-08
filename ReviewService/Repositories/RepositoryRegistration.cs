namespace ReviewService.Repositories;

using Event;
using Review;

public static class RepositoryRegistration {
    public static void AddRepositories(this IServiceCollection services) {
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
    }
}
