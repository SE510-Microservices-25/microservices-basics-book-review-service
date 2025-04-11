namespace ReviewService.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Models;
using Models.DTOs;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController(IMediator mediator) : ControllerBase {
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Review>>> GetReviews() {
        var reviews = await mediator.Send(new GetAllReviewsQuery());
        return Ok(reviews);
    }
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Review>> GetReview(int id) {
        var review = await mediator.Send(new GetReviewByIdQuery(id));
        if (review == null) {
            return NotFound();
        }
        return Ok(review);
    }
    
    [HttpGet("book/{bookId:int}")]
    public async Task<ActionResult<IEnumerable<Review>>> GetByBookId(int bookId) {
        var reviews = await mediator.Send(new GetReviewsByBookIdQuery(bookId));
        return Ok(reviews);
    }

    [HttpGet("statistics")]
    public async Task<ActionResult<IEnumerable<BookRatingStatistics>>> GetAllStatistics() {
        var statistics = await mediator.Send(new GetAllBooksStatisticsQuery());
        return Ok(statistics);
    }
    
    [HttpGet("statistics/book/{bookId:int}")]
    public async Task<ActionResult<BookRatingStatistics>> GetBookStatistics(int bookId) {
        var statistics = await mediator.Send(new GetBookStatisticsQuery(bookId));
        return Ok(statistics);
    }
    
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Review>> Create(CreateReviewCommand command) {
        var createdReview = await mediator.Send(command);
        return CreatedAtAction(nameof(GetReview), new { id = createdReview.Id }, createdReview);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, UpdateReviewCommand command) {
        var result = await mediator.Send(command with { Id = id });
        if (result == null)
            return NotFound();
        
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id) {
        var result = await mediator.Send(new DeleteReviewCommand(id));
        if (!result)
            return NotFound();
        
        return NoContent();
    }
}
