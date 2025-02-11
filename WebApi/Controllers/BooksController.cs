namespace WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using Domain.Entities;
using Application.Api;

[ApiController]
[Route("books")]
public class BooksController : ControllerBase
{
    private readonly IBooksRepository _booksRepository;

    public BooksController(IBooksRepository booksRepository)
    {
        _booksRepository = booksRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<Book>>> GetAllBooks()
    {
        var books = await _booksRepository.GetAllAsync();
        return Ok(books);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Book>> GetBookById(Guid id)
    {
        var book = await _booksRepository.GetByIdAsync(id);
        if (book == null)
        {
            return NotFound();
        }
        return Ok(book);
    }
}
