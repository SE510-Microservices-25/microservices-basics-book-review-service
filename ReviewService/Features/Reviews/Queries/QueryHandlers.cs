namespace ReviewService.Features.Reviews.Queries;

using MediatR;

using Models;
using Models.DTOs;
using Repositories.Review;

public static class ReviewQueryHandlers {
    public class GetReviewByIdHandler(IReviewRepository repository) : IRequestHandler<GetReviewByIdQuery, Review?> {
        public async Task<Review?> Handle(GetReviewByIdQuery request, CancellationToken cancellationToken) {
            return await repository.GetByIdAsync(request.Id);
        }
    }

    public class GetReviewsByBookIdHandler(IReviewRepository repository) : IRequestHandler<GetReviewsByBookIdQuery, IEnumerable<Review>> {
        public async Task<IEnumerable<Review>> Handle(GetReviewsByBookIdQuery request, CancellationToken cancellationToken) {
            return await repository.GetByBookIdAsync(request.BookId);
        }
    }

    public class GetAllReviewsHandler(IReviewRepository repository)
        : IRequestHandler<GetAllReviewsQuery, IEnumerable<Review>> {
        public async Task<IEnumerable<Review>> Handle(GetAllReviewsQuery request, CancellationToken cancellationToken) {
            return await repository.GetAllAsync();
        }
    }

    public class GetBookStatisticsHandler(IReviewRepository repository) : IRequestHandler<GetBookStatisticsQuery, BookRatingStatistics> {
        public async Task<BookRatingStatistics> Handle(GetBookStatisticsQuery request, CancellationToken cancellationToken) {
            return await repository.GetBookStatisticsAsync(request.BookId);
        }
    }

    public class GetAllBooksStatisticsHandler(IReviewRepository repository) : IRequestHandler<GetAllBooksStatisticsQuery, IEnumerable<BookRatingStatistics>> {
        public async Task<IEnumerable<BookRatingStatistics>> Handle(GetAllBooksStatisticsQuery request, CancellationToken cancellationToken) {
            return await repository.GetAllBooksStatisticsAsync();
        }
    }
}
