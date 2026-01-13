namespace HomeInventory.Application.Contracts;

public interface IJwtTokenService
{
    Task<string> GenerateTokenAsync(Guid userId);
}