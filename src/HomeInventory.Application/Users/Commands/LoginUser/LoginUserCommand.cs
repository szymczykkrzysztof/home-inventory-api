using HomeInventory.Application.Users.Dtos;
using MediatR;

namespace HomeInventory.Application.Users.Commands.LoginUser;

public class LoginUserCommand : IRequest<AuthResponseDto>
{
    public required string Email { get; set; }

    public required string Password { get; set; }
}