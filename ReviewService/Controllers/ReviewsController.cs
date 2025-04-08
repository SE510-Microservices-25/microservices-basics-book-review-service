namespace ReviewService.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Models;
using Repositories;
using Services;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController(IReviewRepository repository, MessageBusService messageBus) : ControllerBase {
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Review>>> GetReviews() {
        var reviews = await repository.GetAllAsync();
        return Ok(reviews);
    }
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Review>> GetReview(int id) {
        var review = await repository.GetByIdAsync(id);
        if (review == null) {
            return NotFound();
        }
        return Ok(review);
    }
    
    [HttpGet("book/{bookId:int}")]
    public async Task<ActionResult<IEnumerable<Review>>> GetByBookId(int bookId) {
        var reviews = await repository.GetByBookIdAsync(bookId);
        return Ok(reviews);
    }

    [HttpGet("statistics")]
    public async Task<ActionResult<IEnumerable<BookRatingStatistics>>> GetAllStatistics() {
        var statistics = await repository.GetAllBooksStatisticsAsync();
        return Ok(statistics);
    }
    
    [HttpGet("statistics/book/{bookId:int}")]
    public async Task<ActionResult<BookRatingStatistics>> GetBookStatistics(int bookId) {
        var statistics = await repository.GetBookStatisticsAsync(bookId);
        return Ok(statistics);
    }
    
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Review>> Create(Review review) {
        var createdReview = await repository.CreateAsync(review);
        await messageBus.PublishReviewCreated(createdReview);
        var statistics = await repository.GetBookStatisticsAsync(createdReview.BookId);
        await messageBus.PublishBookRatingChanged(statistics);
        return CreatedAtAction(nameof(GetReview), new { id = createdReview.Id }, createdReview);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, Review review) {
        var result = await repository.UpdateAsync(id, review);
        if (result == null)
            return NotFound();
        
        await messageBus.PublishReviewUpdated(review);
        var statistics = await repository.GetBookStatisticsAsync(review.BookId);
        await messageBus.PublishBookRatingChanged(statistics);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id) {
        var review = await repository.GetByIdAsync(id);
        if (review == null)
            return NotFound();
        
        var result = await repository.DeleteAsync(id);
        if (!result)
            return NotFound();
        
        await messageBus.PublishReviewDeleted(id, review.BookId);
        var statistics = await repository.GetBookStatisticsAsync(review.BookId);
        await messageBus.PublishBookRatingChanged(statistics);
        return NoContent();
    }
}
