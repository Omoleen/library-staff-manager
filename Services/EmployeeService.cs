using Microsoft.EntityFrameworkCore;
using StaffManagementN.Data;
using StaffManagementN.Interfaces;
using StaffManagementN.Models;

namespace StaffManagementN.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _context;

        public EmployeeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmployeeDto>> GetEmployees()
        {
            var employees = await _context.Employees.ToListAsync();
            return employees.Select(e => new EmployeeDto
            {
                EmployeeID = e.EmployeeID,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                Role = e.Role
            });
        }

        public async Task<EmployeeDto?> GetEmployee(int id)
        {
            var e = await _context.Employees.FindAsync(id);
            if (e == null)
            {
                return null;
            }
            return new EmployeeDto
            {
                EmployeeID = e.EmployeeID,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                Role = e.Role
            };
        }

        public async Task<UpdateEmployeeDto?> GetEmployeeForUpdate(int id)
        {
            var e = await _context.Employees.FindAsync(id);
            if (e == null)
            {
                return null;
            }
            return new UpdateEmployeeDto
            {
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                Role = e.Role,
                HourlyRate = e.HourlyRate,
                DateHired = e.DateHired
            };
        }

        public async Task<EmployeeDto> CreateEmployee(CreateEmployeeDto dto)
        {
            var employeeModel = new EmployeeModel
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Role = dto.Role,
                HourlyRate = dto.HourlyRate,
                DateHired = dto.DateHired
            };
            _context.Employees.Add(employeeModel);
            await _context.SaveChangesAsync();
            var resultDto = new EmployeeDto
            {
                EmployeeID = employeeModel.EmployeeID,
                FirstName = employeeModel.FirstName,
                LastName = employeeModel.LastName,
                Email = employeeModel.Email,
                Role = employeeModel.Role
            };
            return resultDto;
        }

        public async Task<bool> UpdateEmployee(int id, UpdateEmployeeDto dto)
        {
            var employeeModel = await _context.Employees.FindAsync(id);
            if (employeeModel == null)
            {
                return false;
            }

            employeeModel.FirstName = dto.FirstName;
            employeeModel.LastName = dto.LastName;
            employeeModel.Email = dto.Email;
            employeeModel.Role = dto.Role;
            employeeModel.HourlyRate = dto.HourlyRate;
            employeeModel.DateHired = dto.DateHired;

            _context.Entry(employeeModel).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Employees.Any(e => e.EmployeeID == id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
            return true;
        }

        public async Task<bool> DeleteEmployee(int id)
        {
            var employeeModel = await _context.Employees.FindAsync(id);
            if (employeeModel == null)
            {
                return false;
            }
            
            _context.Employees.Remove(employeeModel);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<EmployeeDto>> GetEmployeesForShift(int shiftId)
        {
            var employeesInShift = await _context.EmployeeShiftModel
                .Where(es => es.ShiftID == shiftId)
                .Include(es => es.Employee)
                .Select(es => new EmployeeDto
                {
                    EmployeeID = es.Employee.EmployeeID,
                    FirstName = es.Employee.FirstName,
                    LastName = es.Employee.LastName,
                    Email = es.Employee.Email,
                    Role = es.Employee.Role
                })
                .ToListAsync();

            return employeesInShift;
        }
    }
} 