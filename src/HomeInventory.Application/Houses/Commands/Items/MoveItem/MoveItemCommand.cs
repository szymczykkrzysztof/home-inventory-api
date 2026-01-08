using MediatR;

namespace HomeInventory.Application.Houses.Commands.Items.MoveItem;

public sealed record MoveItemCommand(Guid HouseId, Guid ItemId, Guid FromLocationId, Guid ToLocationId) : IRequest;