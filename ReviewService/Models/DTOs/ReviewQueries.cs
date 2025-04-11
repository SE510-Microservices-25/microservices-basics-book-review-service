namespace ReviewService.Models.DTOs;

using MediatR;

public abstract record QueryBase<TResponse> : IRequest<TResponse>;

public record GetReviewByIdQuery(int Id) : QueryBase<Review?>;

public record GetReviewsByBookIdQuery(int BookId) : QueryBase<IEnumerable<Review>>;

public record GetAllReviewsQuery : QueryBase<IEnumerable<Review>>;

public record GetBookStatisticsQuery(int BookId) : QueryBase<BookRatingStatistics>;

public record GetAllBooksStatisticsQuery : QueryBase<IEnumerable<BookRatingStatistics>>;
