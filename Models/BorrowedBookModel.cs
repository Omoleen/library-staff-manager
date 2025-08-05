using System.ComponentModel.DataAnnotations;

namespace StaffManagementN.Models;

public class BorrowedBookModel
{
    [Key]
    public int BorrowId { get; set; }

    public int MemberId { get; set; }

    public int BookId { get; set; }

    public DateTime BorrowDate { get; set; }

    public DateTime DueDate { get; set; }

    public DateTime? ReturnDate { get; set; }

    public int? BorrowedDuringShiftId { get; set; }

    public int? ReturnedDuringShiftId { get; set; }

    public int? ProcessedByEmployeeId { get; set; }

    public int? ReceivedByEmployeeId { get; set; }

    public virtual MemberModel? Member { get; set; }

    public virtual BookModel? Book { get; set; }

    public virtual ShiftModel? BorrowedDuringShift { get; set; }

    public virtual ShiftModel? ReturnedDuringShift { get; set; }

    public virtual EmployeeModel? ProcessedByEmployee { get; set; }

    public virtual EmployeeModel? ReceivedByEmployee { get; set; }
} 