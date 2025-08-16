using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StaffManagementN.Interfaces;
using StaffManagementN.Models;

namespace StaffManagementN.Controllers;

/// <summary>
/// Controller responsible for managing library books through the web interface.
/// Provides functionality for CRUD operations on books, including book cover image management.
/// This controller requires Admin role authorization for all actions.
/// </summary>
[Authorize(Roles = "Admin")]
public class BooksController : Controller
{
    private readonly IBookService _bookService;
    private readonly IBorrowedBookService _borrowedBookService;
    private readonly IFileService _fileService;
    private readonly ILogger<BooksController> _logger;

    /// <summary>
    /// Initializes a new instance of the BooksController with required services.
    /// </summary>
    /// <param name="bookService">Service for managing book data</param>
    /// <param name="borrowedBookService">Service for managing book borrowing records</param>
    /// <param name="fileService">Service for handling file operations</param>
    /// <param name="logger">Logger for the BooksController</param>
    public BooksController(IBookService bookService, IBorrowedBookService borrowedBookService, 
        IFileService fileService, ILogger<BooksController> logger)
    {
        _bookService = bookService;
        _borrowedBookService = borrowedBookService;
        _fileService = fileService;
        _logger = logger;
    }

    /// <summary>
    /// Displays a list of all books in the library.
    /// </summary>
    /// <returns>A view containing a list of all books</returns>
    public async Task<IActionResult> Index()
    {
        var books = await _bookService.GetAllBooksAsync();
        return View(books);
    }

    /// <summary>
    /// Displays detailed information about a specific book.
    /// </summary>
    /// <param name="id">The ID of the book to display</param>
    /// <returns>A view containing detailed book information</returns>
    public async Task<IActionResult> Details(int id)
    {
        var book = await _bookService.GetBookByIdAsync(id);
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    /// <summary>
    /// Displays the form for adding a new book to the library.
    /// </summary>
    /// <returns>A view containing the book creation form</returns>
    public IActionResult Create()
    {
        return View();
    }

    /// <summary>
    /// Processes the creation of a new book, including handling of book cover image upload.
    /// </summary>
    /// <param name="book">The book model containing the new book's information</param>
    /// <param name="imageFile">Optional cover image file for the book</param>
    /// <returns>Redirects to the Index action if successful, otherwise returns to the creation form with validation errors</returns>
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

    /// <summary>
    /// Displays the form for editing an existing book's information.
    /// </summary>
    /// <param name="id">The ID of the book to edit</param>
    /// <returns>A view containing the book edit form</returns>
    public async Task<IActionResult> Edit(int id)
    {
        var book = await _bookService.GetBookByIdAsync(id);
        if (book == null)
        {
            return NotFound();
        }
        return View(book);
    }

    /// <summary>
    /// Processes the update of an existing book's information, including handling of book cover image updates.
    /// </summary>
    /// <param name="id">The ID of the book to update</param>
    /// <param name="book">The book model containing the updated information</param>
    /// <param name="imageFile">Optional new cover image file for the book</param>
    /// <returns>Redirects to the Index action if successful, otherwise returns to the edit form with validation errors</returns>
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

    /// <summary>
    /// Displays the confirmation page for deleting a book.
    /// </summary>
    /// <param name="id">The ID of the book to delete</param>
    /// <returns>A view containing the book deletion confirmation</returns>
    public async Task<IActionResult> Delete(int id)
    {
        var book = await _bookService.GetBookByIdAsync(id);
        if (book == null)
        {
            return NotFound();
        }
        return View(book);
    }

    /// <summary>
    /// Processes the deletion of a book, including removal of its cover image.
    /// </summary>
    /// <param name="id">The ID of the book to delete</param>
    /// <returns>Redirects to the Index action after successful deletion</returns>
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