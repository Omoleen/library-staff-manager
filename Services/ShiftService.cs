using Microsoft.EntityFrameworkCore;
using StaffManagementN.Data;
using StaffManagementN.Interfaces;
using StaffManagementN.Models;

namespace StaffManagementN.Services
{
    public class ShiftService : IShiftService
    {
        private readonly ApplicationDbContext _context;

        public ShiftService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ShiftDto>> GetShifts()
        {
            var shifts = await _context.Shifts.ToListAsync();
            return shifts.Select(s => new ShiftDto
            {
                ShiftID = s.ShiftID,
                StartDateTime = s.StartDateTime,
                EndDateTime = s.EndDateTime
            });
        }

        public async Task<ShiftDto?> GetShift(int id)
        {
            var s = await _context.Shifts.FindAsync(id);
            if (s == null)
            {
                return null;
            }
            return new ShiftDto
            {
                ShiftID = s.ShiftID,
                StartDateTime = s.StartDateTime,
                EndDateTime = s.EndDateTime
            };
        }

        public async Task<ShiftDto> CreateShift(CreateShiftDto dto)
        {
            var shiftModel = new ShiftModel
            {
                StartDateTime = dto.StartDateTime,
                EndDateTime = dto.EndDateTime
            };
            _context.Shifts.Add(shiftModel);
            await _context.SaveChangesAsync();
            var resultDto = new ShiftDto
            {
                ShiftID = shiftModel.ShiftID,
                StartDateTime = shiftModel.StartDateTime,
                EndDateTime = shiftModel.EndDateTime
            };
            return resultDto;
        }

        public async Task<bool> UpdateShift(int id, UpdateShiftDto dto)
        {
            var shiftModel = await _context.Shifts.FindAsync(id);
            if (shiftModel == null)
            {
                return false;
            }

            shiftModel.StartDateTime = dto.StartDateTime;
            shiftModel.EndDateTime = dto.EndDateTime;

            _context.Entry(shiftModel).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Shifts.Any(e => e.ShiftID == id))
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

        public async Task<bool> DeleteShift(int id)
        {
            var shiftModel = await _context.Shifts.FindAsync(id);
            if (shiftModel == null)
            {
                return false;
            }

            _context.Shifts.Remove(shiftModel);
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 