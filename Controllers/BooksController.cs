using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StaffManagementN.Interfaces;
using StaffManagementN.Models;

namespace StaffManagementN.Controllers;

[Authorize(Roles = "Admin")]
public class BooksController : Controller
{
    private readonly IBookService _bookService;
    private readonly IBorrowedBookService _borrowedBookService;
    private readonly IFileService _fileService;
    private readonly ILogger<BooksController> _logger;

    public BooksController(IBookService bookService, IBorrowedBookService borrowedBookService, 
        IFileService fileService, ILogger<BooksController> logger)
    {
        _bookService = bookService;
        _borrowedBookService = borrowedBookService;
        _fileService = fileService;
        _logger = logger;
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
    public async Task<IActionResult> Create(BookModel book, IFormFile? imageFile)
    {
        if (ModelState.IsValid)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                book.ImagePath = await _fileService.UploadFileAsync(imageFile, "books");
            }

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
    public async Task<IActionResult> Edit(int id, BookModel book, IFormFile? imageFile)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // Get the current book to preserve the image path if no new image is uploaded
                var currentBook = await _bookService.GetBookByIdAsync(id);
                if (currentBook == null)
                {
                    return NotFound();
                }

                if (imageFile != null && imageFile.Length > 0)
                {
                    // Delete the old image if it exists
                    if (!string.IsNullOrEmpty(currentBook.ImagePath))
                    {
                        await _fileService.DeleteFileAsync(currentBook.ImagePath);
                    }

                    // Upload the new image
                    book.ImagePath = await _fileService.UploadFileAsync(imageFile, "books");
                    _logger.LogInformation("New image uploaded for book {BookId}: {ImagePath}", id, book.ImagePath);
                }
                else
                {
                    // Preserve the existing image path if no new image is uploaded
                    book.ImagePath = currentBook.ImagePath;
                    _logger.LogInformation("Preserving existing image for book {BookId}: {ImagePath}", id, book.ImagePath);
                }

                await _bookService.UpdateBookAsync(id, book);
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating book {BookId}", id);
                ModelState.AddModelError("", "An error occurred while updating the book. Please try again.");
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
            var book = await _bookService.GetBookByIdAsync(id);
            if (book != null && !string.IsNullOrEmpty(book.ImagePath))
            {
                await _fileService.DeleteFileAsync(book.ImagePath);
            }

            await _bookService.DeleteBookAsync(id);
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
} 