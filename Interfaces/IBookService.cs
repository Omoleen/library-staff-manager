using StaffManagementN.Models;

namespace StaffManagementN.Interfaces;

public interface IBookService
{
    Task<IEnumerable<BookModel>> GetAllBooksAsync();
    Task<BookModel> GetBookByIdAsync(int id);
    Task<BookModel> CreateBookAsync(BookModel book);
    Task<BookModel> UpdateBookAsync(int id, BookModel book);
    Task DeleteBookAsync(int id);
} 