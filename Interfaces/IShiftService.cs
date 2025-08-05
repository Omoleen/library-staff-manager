using StaffManagementN.Models;

namespace StaffManagementN.Interfaces
{
    public interface IShiftService
    {
        Task<IEnumerable<ShiftDto>> GetShifts();
        Task<ShiftDto?> GetShift(int id);
        Task<ShiftDto> CreateShift(CreateShiftDto createShiftDto);
        Task<bool> UpdateShift(int id, UpdateShiftDto updateShiftDto);
        Task<bool> DeleteShift(int id);
    }
} 