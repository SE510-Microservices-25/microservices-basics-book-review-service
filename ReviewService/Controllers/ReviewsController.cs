using Microsoft.AspNetCore.Mvc;
using ReviewService.Models;
using ReviewService.Repositories;

namespace ReviewService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController(IReviewRepository repository) : ControllerBase {
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

    [HttpPost]
    public async Task<ActionResult<Review>> Create(Review review) {
        var createdReview = await repository.CreateAsync(review);
        return CreatedAtAction(nameof(GetReview), new { id = createdReview.Id }, createdReview);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Review review) {
        var result = await repository.UpdateAsync(id, review);
        if (!result)
            return NotFound();
        
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) {
        var result = await repository.DeleteAsync(id);
        if (!result)
            return NotFound();
        
        return NoContent();
    }
}
