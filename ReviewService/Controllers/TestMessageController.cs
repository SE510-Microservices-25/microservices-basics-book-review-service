namespace ReviewService.Controllers;

using Microsoft.AspNetCore.Mvc;
using MassTransit;

using Consumers;

public record BookCreatedTest(int Id, string Title, string Author);

[ApiController]
[Route("api/[controller]")]
public class TestMessageController(IBus bus) : ControllerBase {
    [HttpPost("send-book-created")]
    public async Task<IActionResult> SendBookCreated([FromBody] BookCreatedTest book) {
        await bus.Publish(new BookCreated(book.Id, book.Title, book.Author));
        return Ok("Message sent");
    }
}
