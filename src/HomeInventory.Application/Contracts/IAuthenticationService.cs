namespace HomeInventory.Application.Contracts;

public interface IAuthenticationService
{
    Task<Guid> AuthenticateAsync(string email, string password);
}