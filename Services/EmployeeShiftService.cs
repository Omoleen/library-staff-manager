using Microsoft.EntityFrameworkCore;
using StaffManagementN.Data;
using StaffManagementN.Interfaces;
using StaffManagementN.Models;

namespace StaffManagementN.Services
{
    public class EmployeeShiftService : IEmployeeShiftService
    {
        private readonly ApplicationDbContext _context;

        public EmployeeShiftService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmployeeShiftModel>> GetAllEmployeeShiftsAsync()
        {
            return await _context.EmployeeShiftModel.ToListAsync();
        }

        public async Task<EmployeeShiftModel?> GetEmployeeShift(int id)
        {
            return await _context.EmployeeShiftModel.FindAsync(id);
        }

        public async Task<EmployeeShiftModel> CreateEmployeeShift(EmployeeShiftModel employeeShiftModel)
        {
            _context.EmployeeShiftModel.Add(employeeShiftModel);
            await _context.SaveChangesAsync();
            return employeeShiftModel;
        }

        public async Task<bool> UpdateEmployeeShift(int id, EmployeeShiftModel employeeShiftModel)
        {
            if (id != employeeShiftModel.EmployeeShiftID)
            {
                return false;
            }

            _context.Entry(employeeShiftModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.EmployeeShiftModel.Any(e => e.EmployeeShiftID == id))
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

        public async Task<bool> DeleteEmployeeShift(int id)
        {
            var employeeShiftModel = await _context.EmployeeShiftModel.FindAsync(id);
            if (employeeShiftModel == null)
            {
                return false;
            }

            _context.EmployeeShiftModel.Remove(employeeShiftModel);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<LinkEmployeeShiftDto?> LinkEmployeeToShift(LinkEmployeeShiftDto dto)
        {
            var exists = await _context.EmployeeShiftModel.AnyAsync(es => es.EmployeeID == dto.EmployeeID && es.ShiftID == dto.ShiftID);
            if (exists)
            {
                return null;
            }

            var model = new EmployeeShiftModel { EmployeeID = dto.EmployeeID, ShiftID = dto.ShiftID };
            _context.EmployeeShiftModel.Add(model);
            await _context.SaveChangesAsync();
            return dto;
        }

        public async Task<bool> UnlinkEmployeeFromShift(int employeeId, int shiftId)
        {
            var link = await _context.EmployeeShiftModel.FirstOrDefaultAsync(es => es.EmployeeID == employeeId && es.ShiftID == shiftId);
            if (link == null)
            {
                return false;
            }

            _context.EmployeeShiftModel.Remove(link);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<EmployeeDto>> GetEmployeesInShift(int shiftId)
        {
            var employees = await _context.EmployeeShiftModel
                .Where(es => es.ShiftID == shiftId)
                .Include(es => es.Employee)
                .Select(es => es.Employee)
                .ToListAsync();
            
            return employees.Select(e => new EmployeeDto
            {
                EmployeeID = e.EmployeeID,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                Role = e.Role
            });
        }

        public async Task<IEnumerable<ShiftDto>> GetShiftsForEmployee(int employeeId)
        {
            var shifts = await _context.EmployeeShiftModel
                .Where(es => es.EmployeeID == employeeId)
                .Include(es => es.Shift)
                .Select(es => es.Shift)
                .ToListAsync();
            
            return shifts.Select(s => new ShiftDto
            {
                ShiftID = s.ShiftID,
                StartDateTime = s.StartDateTime,
                EndDateTime = s.EndDateTime
            });
        }
    }
} 