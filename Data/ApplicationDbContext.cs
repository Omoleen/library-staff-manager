using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StaffManagementN.Models;

namespace StaffManagementN.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<EmployeeModel> Employees { get; set; }
    
    public DbSet<ShiftModel> Shifts { get; set; }

    public DbSet<StaffManagementN.Models.EmployeeShiftModel> EmployeeShiftModel { get; set; } = default!;

    public DbSet<BookModel> Books { get; set; }
    public DbSet<MemberModel> Members { get; set; }
    public DbSet<BorrowedBookModel> BorrowedBooks { get; set; }
}