using MediatR;

namespace HomeInventory.Application.Houses.Commands.Items.RemoveItem;

public sealed record RemoveItemCommand(Guid HouseId, Guid LocationId, Guid ItemId) : IRequest;