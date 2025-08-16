using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StaffManagementN.Interfaces;
using StaffManagementN.Models;

namespace StaffManagementN.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class BorrowedBooksAPIController : ControllerBase
{
    private readonly IBorrowedBookService _borrowedBookService;

    public BorrowedBooksAPIController(IBorrowedBookService borrowedBookService)
    {
        _borrowedBookService = borrowedBookService;
    }

    // GET: api/BorrowedBooksAPI
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BorrowedBookModel>>> GetBorrowedBooks()
    {
        var borrowedBooks = await _borrowedBookService.GetAllBorrowedBooksAsync();
        return Ok(borrowedBooks);
    }

    // GET: api/BorrowedBooksAPI/5
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

    // GET: api/BorrowedBooksAPI/member/5
    [HttpGet("member/{memberId}")]
    public async Task<ActionResult<IEnumerable<BorrowedBookModel>>> GetBorrowedBooksByMember(int memberId)
    {
        var borrowedBooks = await _borrowedBookService.GetBorrowedBooksByMemberAsync(memberId);
        return Ok(borrowedBooks);
    }

    // GET: api/BorrowedBooksAPI/book/5
    [HttpGet("book/{bookId}")]
    public async Task<ActionResult<IEnumerable<BorrowedBookModel>>> GetBorrowedBooksByBook(int bookId)
    {
        var borrowedBooks = await _borrowedBookService.GetBorrowedBooksByBookAsync(bookId);
        return Ok(borrowedBooks);
    }

    // PUT: api/BorrowedBooksAPI/5
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

    // POST: api/BorrowedBooksAPI
    [HttpPost]
    public async Task<ActionResult<BorrowedBookModel>> PostBorrowedBook(BorrowedBookModel borrowedBook)
    {
        var createdBorrowedBook = await _borrowedBookService.CreateBorrowedBookAsync(borrowedBook);
        return CreatedAtAction("GetBorrowedBook", new { id = createdBorrowedBook.BorrowId }, createdBorrowedBook);
    }

    // DELETE: api/BorrowedBooksAPI/5
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

    // POST: api/BorrowedBooksAPI/5/return
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