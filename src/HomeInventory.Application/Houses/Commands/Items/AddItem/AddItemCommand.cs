using MediatR;

namespace HomeInventory.Application.Houses.Commands.Items.AddItem;

public sealed record AddItemCommand(Guid HouseId, Guid LocationId, string Name, string ImageUrl) : IRequest<Guid>;