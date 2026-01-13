using HomeInventory.Application.Contracts;
using HomeInventory.Application.Users.Dtos;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace HomeInventory.Application.Users.Commands.LoginUser;

public class LoginUserCommandHandler(
    IAuthenticationService authenticationService,
    IJwtTokenService jwt,
    IConfiguration config)
    : IRequestHandler<LoginUserCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(
        LoginUserCommand request,
        CancellationToken cancellationToken)
    {
        var userId = await authenticationService.AuthenticateAsync(
            request.Email,
            request.Password);

        var token = await jwt.GenerateTokenAsync(userId);

        var expirationMs = Convert.ToInt32(
            config.GetSection("Jwt")["ExpirationMs"]);

        return new AuthResponseDto(
            "Bearer",
            token,
            expirationMs);
    }
}