using StaffManagementN.Models;

namespace StaffManagementN.Interfaces
{
    public interface IEmployeeShiftService
    {
        Task<IEnumerable<EmployeeShiftModel>> GetAllEmployeeShiftsAsync();
        Task<EmployeeShiftModel?> GetEmployeeShift(int id);
        Task<EmployeeShiftModel> CreateEmployeeShift(EmployeeShiftModel employeeShiftModel);
        Task<bool> UpdateEmployeeShift(int id, EmployeeShiftModel employeeShiftModel);
        Task<bool> DeleteEmployeeShift(int id);
        Task<LinkEmployeeShiftDto?> LinkEmployeeToShift(LinkEmployeeShiftDto dto);
        Task<bool> UnlinkEmployeeFromShift(int employeeId, int shiftId);
        Task<IEnumerable<EmployeeDto>> GetEmployeesInShift(int shiftId);
        Task<IEnumerable<ShiftDto>> GetShiftsForEmployee(int employeeId);
    }
} 