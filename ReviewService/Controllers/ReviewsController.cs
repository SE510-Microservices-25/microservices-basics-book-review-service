using Microsoft.AspNetCore.Mvc;
using ReviewService.Models;

namespace ReviewService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private static List<Review> _reviews = [
        new() { Id = 1, BookId = 1, ReviewerName = "Alice", Content = "Great book!", Rating = 5 },
        new() { Id = 2, BookId = 1, ReviewerName = "Bob", Content = "I didn't like it", Rating = 2 },
        new() { Id = 3, BookId = 2, ReviewerName = "Charlie", Content = "It was okay", Rating = 3 }
    ];
    
    [HttpGet]
    public ActionResult<List<Review>> GetReviews() {
        return Ok(_reviews);
    }
    
    [HttpGet("{id:int}")]
    public ActionResult<Review> GetReview(int id) {
        var review = _reviews.FirstOrDefault(r => r.Id == id);
        if (review == null) {
            return NotFound();
        }
        return Ok(review);
    }
    
    [HttpGet("book/{bookId:int}")]
    public ActionResult<IEnumerable<Review>> GetByBookId(int bookId) {
        var reviews = _reviews.Where(r => r.BookId == bookId).ToList();
        return Ok(reviews);
    }

    [HttpPost]
    public ActionResult<Review> Create(Review review) {
        review.Id = _reviews.Count > 0 ? _reviews.Max(r => r.Id) + 1 : 1;
        _reviews.Add(review);
        return CreatedAtAction(nameof(GetReview), new { id = review.Id }, review);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, Review review) {
        var existingReview = _reviews.FirstOrDefault(r => r.Id == id);
        if (existingReview == null)
            return NotFound();
        
        existingReview.BookId = review.BookId;
        existingReview.ReviewerName = review.ReviewerName;
        existingReview.Content = review.Content;
        existingReview.Rating = review.Rating;
            
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id) {
        var review = _reviews.FirstOrDefault(r => r.Id == id);
        if (review == null)
            return NotFound();
        
        _reviews.Remove(review);
        return NoContent();
    }
}
