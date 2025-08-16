using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StaffManagementN.Interfaces;
using StaffManagementN.Models;

namespace StaffManagementN.Controllers;

/// <summary>
/// API controller for managing book borrowing records through RESTful endpoints.
/// Provides CRUD operations and specialized queries for borrowed books through API calls.
/// This controller requires Admin role authorization for all endpoints.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class BorrowedBooksAPIController : ControllerBase
{
    private readonly IBorrowedBookService _borrowedBookService;

    /// <summary>
    /// Initializes a new instance of the BorrowedBooksAPIController.
    /// </summary>
    /// <param name="borrowedBookService">The service for managing borrowed book records</param>
    public BorrowedBooksAPIController(IBorrowedBookService borrowedBookService)
    {
        _borrowedBookService = borrowedBookService;
    }

    /// <summary>
    /// Retrieves all borrowed book records in the system.
    /// </summary>
    /// <returns>A list of all borrowed books with their details</returns>
    /// <response code="200">Returns the list of borrowed books</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BorrowedBookModel>>> GetBorrowedBooks()
    {
        var borrowedBooks = await _borrowedBookService.GetAllBorrowedBooksAsync();
        return Ok(borrowedBooks);
    }

    /// <summary>
    /// Retrieves a specific borrowed book record by its ID.
    /// </summary>
    /// <param name="id">The ID of the borrowed book record to retrieve</param>
    /// <returns>The borrowed book details</returns>
    /// <response code="200">Returns the requested borrowed book record</response>
    /// <response code="404">If the borrowed book record is not found</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<BorrowedBookModel>> GetBorrowedBook(int id)
    {
        try
        {
            var borrowedBook = await _borrowedBookService.GetBorrowedBookByIdAsync(id);
            return Ok(borrowedBook);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Retrieves all borrowed book records for a specific member.
    /// </summary>
    /// <param name="memberId">The ID of the member whose borrowed books to retrieve</param>
    /// <returns>A list of borrowed books for the specified member</returns>
    /// <response code="200">Returns the list of borrowed books for the member</response>
    [HttpGet("member/{memberId}")]
    public async Task<ActionResult<IEnumerable<BorrowedBookModel>>> GetBorrowedBooksByMember(int memberId)
    {
        var borrowedBooks = await _borrowedBookService.GetBorrowedBooksByMemberAsync(memberId);
        return Ok(borrowedBooks);
    }

    /// <summary>
    /// Retrieves all borrowed book records for a specific book.
    /// </summary>
    /// <param name="bookId">The ID of the book whose borrowing history to retrieve</param>
    /// <returns>A list of borrowed book records for the specified book</returns>
    /// <response code="200">Returns the borrowing history for the book</response>
    [HttpGet("book/{bookId}")]
    public async Task<ActionResult<IEnumerable<BorrowedBookModel>>> GetBorrowedBooksByBook(int bookId)
    {
        var borrowedBooks = await _borrowedBookService.GetBorrowedBooksByBookAsync(bookId);
        return Ok(borrowedBooks);
    }

    /// <summary>
    /// Updates an existing borrowed book record.
    /// </summary>
    /// <param name="id">The ID of the borrowed book record to update</param>
    /// <param name="borrowedBook">The updated borrowed book information</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the borrowed book record was successfully updated</response>
    /// <response code="400">If the borrowed book data is invalid</response>
    /// <response code="404">If the borrowed book record is not found</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> PutBorrowedBook(int id, BorrowedBookModel borrowedBook)
    {
        try
        {
            await _borrowedBookService.UpdateBorrowedBookAsync(id, borrowedBook);
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
    /// Creates a new borrowed book record.
    /// </summary>
    /// <param name="borrowedBook">The borrowed book information to create</param>
    /// <returns>The newly created borrowed book record</returns>
    /// <response code="201">Returns the newly created borrowed book record</response>
    /// <response code="400">If the borrowed book data is invalid</response>
    [HttpPost]
    public async Task<ActionResult<BorrowedBookModel>> PostBorrowedBook(BorrowedBookModel borrowedBook)
    {
        var createdBorrowedBook = await _borrowedBookService.CreateBorrowedBookAsync(borrowedBook);
        return CreatedAtAction("GetBorrowedBook", new { id = createdBorrowedBook.BorrowId }, createdBorrowedBook);
    }

    /// <summary>
    /// Deletes a specific borrowed book record.
    /// </summary>
    /// <param name="id">The ID of the borrowed book record to delete</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the borrowed book record was successfully deleted</response>
    /// <response code="404">If the borrowed book record is not found</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBorrowedBook(int id)
    {
        try
        {
            await _borrowedBookService.DeleteBorrowedBookAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Records the return of a borrowed book.
    /// </summary>
    /// <param name="id">The ID of the borrowed book record to mark as returned</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the book return was successfully recorded</response>
    /// <response code="404">If the borrowed book record is not found</response>
    [HttpPost("{id}/return")]
    public async Task<IActionResult> ReturnBook(int id)
    {
        try
        {
            await _borrowedBookService.ReturnBookAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
} 