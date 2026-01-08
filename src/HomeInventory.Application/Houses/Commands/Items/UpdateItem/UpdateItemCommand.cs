using MediatR;

namespace HomeInventory.Application.Houses.Commands.Items.UpdateItem;

public sealed record UpdateItemCommand(
    Guid HouseId,
    Guid LocationId,
    Guid ItemId,
    string Name,
    string ImageUrl) : IRequest;