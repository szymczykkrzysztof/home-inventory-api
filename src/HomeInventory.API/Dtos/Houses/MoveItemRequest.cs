namespace HomeInventory.API.Dtos.Houses;

public sealed record MoveItemRequest(
    Guid FromLocationId,
    Guid ToLocationId);