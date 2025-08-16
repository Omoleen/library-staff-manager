using System.ComponentModel.DataAnnotations;

namespace StaffManagementN.Models;

public class MemberModel
{
    [Key]
    public int MemberId { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public int PhoneNumber { get; set; }

    public string? ImagePath { get; set; }

    // A member can borrow many books
    public virtual ICollection<BorrowedBookModel>? BorrowedBooks { get; set; }
} 