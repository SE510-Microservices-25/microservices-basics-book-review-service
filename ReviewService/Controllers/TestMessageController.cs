namespace ReviewService.Controllers;

using Microsoft.AspNetCore.Mvc;
using MassTransit;
using Microsoft.Extensions.Configuration;

using Contracts;

[ApiController]
[Route("api/[controller]")]
public class TestMessageController(IBus bus, IConfiguration configuration) : ControllerBase {
    private readonly string _serviceSecretKey = configuration["MessageBus:ServiceSecretKey"] ?? "default-key";

    [HttpPost("send-book-created")]
    public async Task<IActionResult> SendBookCreated([FromBody] BookCreated book) {
        await bus.Publish(new BookCreated(book.Id, book.Title, book.Author), ctx => {
            ctx.Headers.Set("ServiceAuthentication", _serviceSecretKey);
        });
        return Ok("Message sent");
    }
}
