using System.ComponentModel.DataAnnotations;

namespace StaffManagementN.Models;


public class ShiftModel
{
    [Key]
    public int ShiftID { get; set; }
    
    public DateTime StartDateTime { get; set; }
    
    public DateTime EndDateTime { get; set; }
}