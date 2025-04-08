namespace ReviewService.Controllers;

using Microsoft.AspNetCore.Mvc;
using MassTransit;

using Contracts;

[ApiController]
[Route("api/[controller]")]
public class TestMessageController(IBus bus) : ControllerBase {
    [HttpPost("send-book-created")]
    public async Task<IActionResult> SendBookCreated([FromBody] BookCreated book) {
        await bus.Publish(new BookCreated(book.Id, book.Title, book.Author));
        return Ok("Message sent");
    }
}
