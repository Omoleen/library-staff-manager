using Microsoft.AspNetCore.Identity;
using StaffManagementN.Interfaces;

namespace StaffManagementN.Data;

public static class DbSeeder
{
    public static async Task SeedAdminUser(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var adminService = scope.ServiceProvider.GetRequiredService<IAdminService>();
        
        // Create admin role if it doesn't exist
        await adminService.EnsureAdminRoleExistsAsync();
        
        // Check if admin user exists
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var adminEmail = "admin@staffmanagement.com";
        
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            // Create admin user with a secure password
            // In production, this should be configured through environment variables or secure configuration
            var defaultAdminPassword = "Admin@123456";
            await adminService.CreateAdminUserAsync(adminEmail, defaultAdminPassword);
        }
    }
} 