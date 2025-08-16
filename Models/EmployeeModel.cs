using System.ComponentModel.DataAnnotations;

namespace StaffManagementN.Models;

public class EmployeeModel
{
    [Key]
    public int EmployeeID { get; set; }
    
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public string Email { get; set; }
    
    public string Role { get; set; }
    
    public Decimal HourlyRate { get; set; }
    
    public DateTime DateHired { get; set; }

    public string? ImagePath { get; set; }
}