namespace StaffManagementN.Models;

public class CreateEmployeeDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public decimal HourlyRate { get; set; }
    public DateTime DateHired { get; set; }
} 