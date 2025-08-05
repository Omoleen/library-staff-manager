namespace StaffManagementN.Models
{
    public class EmployeeDetailsViewModel
    {
        public EmployeeDto Employee { get; set; }
        public IEnumerable<ShiftDto> AssignedShifts { get; set; }
        public IEnumerable<ShiftDto> AvailableShifts { get; set; }
    }
} 