using StaffManagementN.Models;

namespace StaffManagementN.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetEmployees();
        Task<EmployeeDto?> GetEmployee(int id);
        Task<UpdateEmployeeDto?> GetEmployeeForUpdate(int id);
        Task<EmployeeDto> CreateEmployee(CreateEmployeeDto createEmployeeDto);
        Task<bool> UpdateEmployee(int id, UpdateEmployeeDto updateEmployeeDto);
        Task<bool> DeleteEmployee(int id);
        Task<IEnumerable<EmployeeDto>> GetEmployeesForShift(int shiftId);
    }
} 