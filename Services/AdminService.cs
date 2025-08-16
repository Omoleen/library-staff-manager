using Microsoft.AspNetCore.Identity;
using StaffManagementN.Interfaces;

namespace StaffManagementN.Services;

public class AdminService : IAdminService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private const string AdminRoleName = "Admin";

    public AdminService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<bool> CreateAdminUserAsync(string email, string password)
    {
        var user = new IdentityUser { UserName = email, Email = email };
        var result = await _userManager.CreateAsync(user, password);
        
        if (result.Succeeded)
        {
            await EnsureAdminRoleExistsAsync();
            var roleResult = await _userManager.AddToRoleAsync(user, AdminRoleName);
            return roleResult.Succeeded;
        }
        
        return false;
    }

    public async Task<bool> IsInRoleAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;
        
        return await _userManager.IsInRoleAsync(user, roleName);
    }

    public async Task<IdentityResult> AddToRoleAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found" });
        
        return await _userManager.AddToRoleAsync(user, roleName);
    }

    public async Task<bool> EnsureAdminRoleExistsAsync()
    {
        if (!await _roleManager.RoleExistsAsync(AdminRoleName))
        {
            var result = await _roleManager.CreateAsync(new IdentityRole(AdminRoleName));
            return result.Succeeded;
        }
        return true;
    }

    public async Task<IList<IdentityUser>> GetUsersInRoleAsync(string roleName)
    {
        return await _userManager.GetUsersInRoleAsync(roleName);
    }
} 