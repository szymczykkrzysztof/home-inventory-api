using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HomeInventory.Application.Contracts;
using HomeInventory.Infrastructure.Authorization.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HomeInventory.Infrastructure.Authorization.Services;

public class JwtTokenService(
    IConfiguration config,
    UserManager<User> userManager,
    RoleManager<IdentityRole> roleManager) : IJwtTokenService
{
    public async Task<string> GenerateTokenAsync(Guid userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            throw new InvalidOperationException($"User with id {userId} not found");
        var jwt = config.GetSection("Jwt");

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            //Custom claims
            //new Claim(AppClaimTypes.Nationality, user.Nationality ?? ""),
            //new Claim(AppClaimTypes.DateOfBirth, user.DateOfBirth?.ToString("yyyy-MM-dd") ?? string.Empty)
        };

        // 🔹 Claims użytkownika
        var userClaims = await userManager.GetClaimsAsync(user);
        claims.AddRange(userClaims);
        // 🔹 Role użytkownika
        var roles = await userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));

            var identityRole = await roleManager.FindByNameAsync(role);
            if (identityRole != null)
            {
                var roleClaims = await roleManager.GetClaimsAsync(identityRole);
                claims.AddRange(roleClaims);
            }
        }

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwt["Key"]!)
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMilliseconds(Convert.ToDouble(jwt["ExpirationMs"])),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}