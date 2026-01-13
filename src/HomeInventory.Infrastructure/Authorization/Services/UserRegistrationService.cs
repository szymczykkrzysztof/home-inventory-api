using HomeInventory.Application.Contracts;
using HomeInventory.Domain.Exceptions;
using HomeInventory.Infrastructure.Authorization.Entity;
using Microsoft.AspNetCore.Identity;

namespace HomeInventory.Infrastructure.Authorization.Services;

public class IdentityUserRegistrationService(
    UserManager<User> userManager,
    RoleManager<IdentityRole> roleManager)
    : IUserRegistrationService
{
    public async Task<Guid> RegisterAsync(string email, string password)
    {
        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser != null)
            throw new UserAlreadyExistException();

        var user = new User
        {
            UserName = email,
            Email = email
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            throw new PasswordMismatchException(
                string.Join(", ", result.Errors.Select(e => e.Description)));

        // 🔹 Role bootstrap (pierwszy user = Admin)
        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new IdentityRole("Admin"));

        if (!await roleManager.RoleExistsAsync("User"))
            await roleManager.CreateAsync(new IdentityRole("User"));

        var isFirstUser = !(await userManager.GetUsersInRoleAsync("Admin")).Any();

        await userManager.AddToRoleAsync(
            user,
            isFirstUser ? "Admin" : "User");

        // 🔹 Domyślny claim
        // await userManager.AddClaimAsync(
        //     user,
        //     new Claim("permission", "restaurants.read"));

        return Guid.Parse(user.Id);
    }
}