namespace BookService.Controllers;

using Microsoft.AspNetCore.Mvc;

using Models;
using Repositories;
using Services.MQ;

[ApiController]
[Route("api/[controller]")]
public class BooksController(IBookRepository bookRepository, IMessageBusService messageBus) : ControllerBase {
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetBooks() {
        var books = await bookRepository.GetAllAsync();
        return Ok(books);
    }
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Book>> GetBook(int id) {
        var book = await bookRepository.GetByIdAsync(id);
        if (book == null) {
            return NotFound();
        }
        return Ok(book);
    }
    
    [HttpPost]
    public async Task<ActionResult<Book>> CreateBook(Book book) {
        var createdBook = await bookRepository.CreateAsync(book);
        await messageBus.PublishBookCreated(createdBook);
        
        return CreatedAtAction(nameof(GetBook), new { id = createdBook.Id }, createdBook);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteBook(int id) {
        var result = await bookRepository.DeleteAsync(id);
        if (!result)
            return NotFound();
        
        await messageBus.PublishBookDeleted(id);
        return NoContent();
    }
}
