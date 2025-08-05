using StaffManagementN.Models;

namespace StaffManagementN.Interfaces;

public interface IBorrowedBookService
{
    Task<IEnumerable<BorrowedBookModel>> GetAllBorrowedBooksAsync();
    Task<BorrowedBookModel> GetBorrowedBookByIdAsync(int id);
    Task<BorrowedBookModel> CreateBorrowedBookAsync(BorrowedBookModel borrowedBook);
    Task<BorrowedBookModel> UpdateBorrowedBookAsync(int id, BorrowedBookModel borrowedBook);
    Task DeleteBorrowedBookAsync(int id);
    Task<IEnumerable<BorrowedBookModel>> GetBorrowedBooksByMemberAsync(int memberId);
    Task<IEnumerable<BorrowedBookModel>> GetBorrowedBooksByBookAsync(int bookId);
    Task<BorrowedBookModel> ReturnBookAsync(int borrowedBookId);
} 