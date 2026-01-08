using MediatR;

namespace HomeInventory.Application.Houses.Queries.GetItems;

public sealed record GetItemsQuery(
    Guid HouseId,
    string? SearchTerm,
    string? RoomName,
    string? ContainerName
) : IRequest<IReadOnlyList<ItemDto>>;