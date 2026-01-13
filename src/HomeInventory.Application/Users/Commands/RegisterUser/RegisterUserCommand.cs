using MediatR;

namespace HomeInventory.Application.Users.Commands.RegisterUser;

public class RegisterUserCommand : IRequest<string>
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }
}