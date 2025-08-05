using System.ComponentModel.DataAnnotations;

namespace StaffManagementN.Models;

public class BookModel
{
    [Key]
    public int BookId { get; set; }

    public string Title { get; set; }

    public string Author { get; set; }

    public string ISBN { get; set; }

    public string Status { get; set; }

    // A book can be borrowed many times
    public virtual ICollection<BorrowedBookModel>? BorrowedBooks { get; set; }
} 