namespace HomeInventory.Application.Houses.Queries.GetDetail;

public sealed class HouseDetailDto
{
    public Guid HouseId { get; init; }
    public string Name { get; init; } = null!;
    public IReadOnlyList<LocationDto> Locations { get; init; } = [];
}

public sealed class LocationDto
{
    public Guid LocationId { get; init; }
    public string RoomName { get; init; } = null!;
    public string? ContainerName { get; init; }
    public IReadOnlyList<ItemDto> Items { get; init; } = [];
}

public sealed class ItemDto
{
    public Guid ItemId { get; init; }
    public string Name { get; init; } = null!;
    public string ImageUrl { get; init; } = null!;
}