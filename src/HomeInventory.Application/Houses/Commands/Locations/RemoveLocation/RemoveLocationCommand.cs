using MediatR;

namespace HomeInventory.Application.Houses.Commands.Locations.RemoveLocation;

public sealed record RemoveLocationCommand(Guid HouseId, Guid LocationId) : IRequest;