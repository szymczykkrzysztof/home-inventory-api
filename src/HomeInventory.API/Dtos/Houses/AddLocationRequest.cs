namespace HomeInventory.API.Dtos.Houses;

public sealed record AddLocationRequest(
    string RoomName,
    string? ContainerName);