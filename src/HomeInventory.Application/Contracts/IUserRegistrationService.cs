namespace HomeInventory.Application.Contracts;

public interface IUserRegistrationService
{
    Task<Guid> RegisterAsync(string email, string password);
}