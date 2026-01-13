namespace HomeInventory.Application.Users.Dtos;

public record AuthResponseDto(
    string TokenType,
    string AccessToken,
    int ExpiresInMs
);