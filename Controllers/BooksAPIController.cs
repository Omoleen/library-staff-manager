using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StaffManagementN.Interfaces;
using StaffManagementN.Models;

namespace StaffManagementN.Controllers;

/// <summary>
/// API controller for managing library books through RESTful endpoints.
/// Provides CRUD operations for book management through API calls.
/// This controller requires Admin role authorization for all endpoints.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class BooksAPIController : ControllerBase
{
    private readonly IBookService _bookService;

    /// <summary>
    /// Initializes a new instance of the BooksAPIController.
    /// </summary>
    /// <param name="bookService">The service for managing book data</param>
    public BooksAPIController(IBookService bookService)
    {
        _bookService = bookService;
    }

    /// <summary>
    /// Retrieves all books in the library.
    /// </summary>
    /// <returns>A list of all books with their details</returns>
    /// <response code="200">Returns the list of books</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookModel>>> GetBooks()
    {
        var books = await _bookService.GetAllBooksAsync();
        return Ok(books);
    }

    /// <summary>
    /// Retrieves a specific book by its ID.
    /// </summary>
    /// <param name="id">The ID of the book to retrieve</param>
    /// <returns>The book details</returns>
    /// <response code="200">Returns the requested book</response>
    /// <response code="404">If the book is not found</response>
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

    /// <summary>
    /// Updates an existing book's information.
    /// </summary>
    /// <param name="id">The ID of the book to update</param>
    /// <param name="book">The updated book information</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the book was successfully updated</response>
    /// <response code="400">If the book data is invalid</response>
    /// <response code="404">If the book is not found</response>
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

    /// <summary>
    /// Creates a new book in the library.
    /// </summary>
    /// <param name="book">The book information to create</param>
    /// <returns>The newly created book</returns>
    /// <response code="201">Returns the newly created book</response>
    /// <response code="400">If the book data is invalid</response>
    [HttpPost]
    public async Task<ActionResult<BookModel>> PostBook(BookModel book)
    {
        var createdBook = await _bookService.CreateBookAsync(book);
        return CreatedAtAction("GetBook", new { id = createdBook.BookId }, createdBook);
    }

    /// <summary>
    /// Deletes a specific book from the library.
    /// </summary>
    /// <param name="id">The ID of the book to delete</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the book was successfully deleted</response>
    /// <response code="404">If the book is not found</response>
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