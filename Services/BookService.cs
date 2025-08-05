using Microsoft.EntityFrameworkCore;
using StaffManagementN.Data;
using StaffManagementN.Interfaces;
using StaffManagementN.Models;

namespace StaffManagementN.Services;

public class BookService : IBookService
{
    private readonly ApplicationDbContext _context;

    public BookService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BookModel>> GetAllBooksAsync()
    {
        return await _context.Books.ToListAsync();
    }

    public async Task<BookModel> GetBookByIdAsync(int id)
    {
        var book = await _context.Books
            .Include(b => b.BorrowedBooks)
            .ThenInclude(bb => bb.Member)
            .FirstOrDefaultAsync(b => b.BookId == id);

        if (book == null)
        {
            throw new KeyNotFoundException($"Book with ID {id} not found.");
        }

        return book;
    }

    public async Task<BookModel> CreateBookAsync(BookModel book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return book;
    }

    public async Task<BookModel> UpdateBookAsync(int id, BookModel book)
    {
        if (id != book.BookId)
        {
            throw new ArgumentException("ID mismatch");
        }

        var existingBook = await _context.Books.FindAsync(id);
        if (existingBook == null)
        {
            throw new KeyNotFoundException($"Book with ID {id} not found.");
        }

        _context.Entry(existingBook).CurrentValues.SetValues(book);
        await _context.SaveChangesAsync();

        return book;
    }

    public async Task DeleteBookAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
        {
            throw new KeyNotFoundException($"Book with ID {id} not found.");
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
    }
} 