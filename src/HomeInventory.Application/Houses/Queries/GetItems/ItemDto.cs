namespace HomeInventory.Application.Houses.Queries.GetItems;

public sealed class ItemDto
{
    public Guid ItemId { get; init; }
    public string Name { get; init; } = null!;
    public string ImageUrl { get; init; } = null!;
    public string RoomName { get; init; } = null!;
    public string? ContainerName { get; init; }
}