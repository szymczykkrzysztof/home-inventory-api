using HomeInventory.Infrastructure.Authorization.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Infrastructure.Seeders;

public class HomeInventorySeeder : IHomeInventorySeeder
{
    public async Task SeedRolesAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roles = {UserRoles.User, UserRoles.Admin};

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}