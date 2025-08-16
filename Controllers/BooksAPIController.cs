using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StaffManagementN.Interfaces;
using StaffManagementN.Models;

namespace StaffManagementN.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class BooksAPIController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksAPIController(IBookService bookService)
    {
        _bookService = bookService;
    }

    // GET: api/BooksAPI
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookModel>>> GetBooks()
    {
        var books = await _bookService.GetAllBooksAsync();
        return Ok(books);
    }

    // GET: api/BooksAPI/5
    [HttpGet("{id}")]
    public async Task<ActionResult<BookModel>> GetBook(int id)
    {
        try
        {
            var book = await _bookService.GetBookByIdAsync(id);
            return Ok(book);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // PUT: api/BooksAPI/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutBook(int id, BookModel book)
    {
        try
        {
            await _bookService.UpdateBookAsync(id, book);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
    }

    // POST: api/BooksAPI
    [HttpPost]
    public async Task<ActionResult<BookModel>> PostBook(BookModel book)
    {
        var createdBook = await _bookService.CreateBookAsync(book);
        return CreatedAtAction("GetBook", new { id = createdBook.BookId }, createdBook);
    }

    // DELETE: api/BooksAPI/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        try
        {
            await _bookService.DeleteBookAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
} 