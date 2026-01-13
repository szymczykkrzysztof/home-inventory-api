using HomeInventory.Application.Contracts;
using HomeInventory.Domain.Exceptions;
using MediatR;

namespace HomeInventory.Application.Users.Commands.RegisterUser;

public class RegisterUserCommandHandler(
    IUserRegistrationService registrationService,
    IJwtTokenService jwt)
    : IRequestHandler<RegisterUserCommand, string>
{
    public async Task<string> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Password != request.ConfirmPassword)
            throw new PasswordMismatchException();

        var userId = await registrationService.RegisterAsync(
            request.Email,
            request.Password);

        var token = await jwt.GenerateTokenAsync(userId);
        return token;
    }
}