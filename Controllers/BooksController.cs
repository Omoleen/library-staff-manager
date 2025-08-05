using Microsoft.AspNetCore.Mvc;
using StaffManagementN.Interfaces;
using StaffManagementN.Models;

namespace StaffManagementN.Controllers;

public class BooksController : Controller
{
    private readonly IBookService _bookService;
    private readonly IBorrowedBookService _borrowedBookService;

    public BooksController(IBookService bookService, IBorrowedBookService borrowedBookService)
    {
        _bookService = bookService;
        _borrowedBookService = borrowedBookService;
    }

    // GET: Books
    public async Task<IActionResult> Index()
    {
        var books = await _bookService.GetAllBooksAsync();
        return View(books);
    }

    // GET: Books/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var book = await _bookService.GetBookByIdAsync(id);
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    // GET: Books/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Books/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookModel book)
    {
        if (ModelState.IsValid)
        {
            await _bookService.CreateBookAsync(book);
            return RedirectToAction(nameof(Index));
        }
        return View(book);
    }

    // GET: Books/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var book = await _bookService.GetBookByIdAsync(id);
        if (book == null)
        {
            return NotFound();
        }
        return View(book);
    }

    // POST: Books/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BookModel book)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _bookService.UpdateBookAsync(id, book);
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
        return View(book);
    }

    // GET: Books/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var book = await _bookService.GetBookByIdAsync(id);
        if (book == null)
        {
            return NotFound();
        }
        return View(book);
    }

    // POST: Books/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _bookService.DeleteBookAsync(id);
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
} 