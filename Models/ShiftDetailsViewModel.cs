namespace StaffManagementN.Models
{
    public class ShiftDetailsViewModel
    {
        public ShiftDto Shift { get; set; }
        public IEnumerable<EmployeeDto> AssignedEmployees { get; set; }
        public IEnumerable<EmployeeDto> AvailableEmployees { get; set; }
        public int? EmployeeToAssignId { get; set; }
    }
} 