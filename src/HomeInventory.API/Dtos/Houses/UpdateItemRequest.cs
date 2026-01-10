namespace HomeInventory.API.Dtos.Houses;

public sealed class UpdateItemRequest
{
    public string Name { get; init; } = null!;
    public string ImageUrl { get; init; } = null!;
}