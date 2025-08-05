using Microsoft.EntityFrameworkCore;
using StaffManagementN.Data;
using StaffManagementN.Interfaces;
using StaffManagementN.Models;

namespace StaffManagementN.Services;

public class BorrowedBookService : IBorrowedBookService
{
    private readonly ApplicationDbContext _context;

    public BorrowedBookService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BorrowedBookModel>> GetAllBorrowedBooksAsync()
    {
        return await _context.BorrowedBooks
            .Include(bb => bb.Book)
            .Include(bb => bb.Member)
            .ToListAsync();
    }

    public async Task<BorrowedBookModel> GetBorrowedBookByIdAsync(int id)
    {
        var borrowedBook = await _context.BorrowedBooks
            .Include(bb => bb.Book)
            .Include(bb => bb.Member)
            .FirstOrDefaultAsync(bb => bb.BorrowId == id);

        if (borrowedBook == null)
        {
            throw new KeyNotFoundException($"Borrowed book with ID {id} not found.");
        }

        return borrowedBook;
    }

    public async Task<BorrowedBookModel> CreateBorrowedBookAsync(BorrowedBookModel borrowedBook)
    {
        _context.BorrowedBooks.Add(borrowedBook);
        await _context.SaveChangesAsync();
        return borrowedBook;
    }

    public async Task<BorrowedBookModel> UpdateBorrowedBookAsync(int id, BorrowedBookModel borrowedBook)
    {
        if (id != borrowedBook.BorrowId)
        {
            throw new ArgumentException("ID mismatch");
        }

        var existingBorrowedBook = await _context.BorrowedBooks.FindAsync(id);
        if (existingBorrowedBook == null)
        {
            throw new KeyNotFoundException($"Borrowed book with ID {id} not found.");
        }

        _context.Entry(existingBorrowedBook).CurrentValues.SetValues(borrowedBook);
        await _context.SaveChangesAsync();

        return borrowedBook;
    }

    public async Task DeleteBorrowedBookAsync(int id)
    {
        var borrowedBook = await _context.BorrowedBooks.FindAsync(id);
        if (borrowedBook == null)
        {
            throw new KeyNotFoundException($"Borrowed book with ID {id} not found.");
        }

        _context.BorrowedBooks.Remove(borrowedBook);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<BorrowedBookModel>> GetBorrowedBooksByMemberAsync(int memberId)
    {
        return await _context.BorrowedBooks
            .Include(bb => bb.Book)
            .Include(bb => bb.Member)
            .Where(bb => bb.MemberId == memberId)
            .ToListAsync();
    }

    public async Task<IEnumerable<BorrowedBookModel>> GetBorrowedBooksByBookAsync(int bookId)
    {
        return await _context.BorrowedBooks
            .Include(bb => bb.Book)
            .Include(bb => bb.Member)
            .Where(bb => bb.BookId == bookId)
            .ToListAsync();
    }

    public async Task<BorrowedBookModel> ReturnBookAsync(int borrowedBookId)
    {
        var borrowedBook = await _context.BorrowedBooks.FindAsync(borrowedBookId);
        if (borrowedBook == null)
        {
            throw new KeyNotFoundException($"Borrowed book with ID {borrowedBookId} not found.");
        }

        borrowedBook.ReturnDate = DateTime.Now;
        await _context.SaveChangesAsync();

        return borrowedBook;
    }
} 