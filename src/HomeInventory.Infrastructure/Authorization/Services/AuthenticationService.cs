using HomeInventory.Application.Contracts;
using HomeInventory.Domain.Exceptions;
using HomeInventory.Infrastructure.Authorization.Entity;
using Microsoft.AspNetCore.Identity;

namespace HomeInventory.Infrastructure.Authorization.Services;

public class IdentityAuthenticationService(
    UserManager<User> userManager)
    : IAuthenticationService
{
    public async Task<Guid> AuthenticateAsync(
        string email,
        string password)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            throw new UserNotAuthorizedException();

        var valid = await userManager.CheckPasswordAsync(user, password);
        if (!valid)
            throw new UserNotAuthorizedException();

        return Guid.Parse(user.Id);
    }
}