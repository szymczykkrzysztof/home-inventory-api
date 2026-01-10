namespace HomeInventory.API.Dtos.Houses;

public sealed class AddItemRequest
{
    public string Name { get; init; } = null!;
    public string ImageUrl { get; init; } = null!;
}