namespace ReviewService.Models.DTOs;

using MediatR;

public abstract record CommandBase<TResponse> : IRequest<TResponse>;

public record CreateReviewCommand(int BookId, string ReviewerName, string Content, int Rating) 
    : CommandBase<Review>;

public record UpdateReviewCommand(int Id, string ReviewerName, string Content, int Rating) 
    : CommandBase<Review?>;

public record DeleteReviewCommand(int Id) : CommandBase<bool>;
