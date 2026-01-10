namespace HomeInventory.API.Dtos.Houses;

public sealed class RenameLocationRequest
{
    public string RoomName { get; init; } = null!;
    public string? ContainerName { get; init; }
}