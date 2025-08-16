using Microsoft.AspNetCore.Identity;

namespace StaffManagementN.Interfaces;

public interface IAdminService
{
    Task<bool> CreateAdminUserAsync(string email, string password);
    Task<bool> IsInRoleAsync(string userId, string roleName);
    Task<IdentityResult> AddToRoleAsync(string userId, string roleName);
    Task<bool> EnsureAdminRoleExistsAsync();
    Task<IList<IdentityUser>> GetUsersInRoleAsync(string roleName);
} 