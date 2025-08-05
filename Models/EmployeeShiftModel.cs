using System.ComponentModel.DataAnnotations;

namespace StaffManagementN.Models;


public class EmployeeShiftModel
{
    [Key]
    public int EmployeeShiftID { get; set; }
    
    public int EmployeeID { get; set; }
    
    public int ShiftID { get; set; }
    
    public virtual EmployeeModel Employee { get; set; }
    
    public virtual ShiftModel Shift { get; set; }
}