namespace ReviewService.Contracts;

public interface IReviewEvent;

public record ReviewDeleted(int Id, int BookId) : IReviewEvent;
