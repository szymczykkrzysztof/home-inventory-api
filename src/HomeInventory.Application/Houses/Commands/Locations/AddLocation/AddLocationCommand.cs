using MediatR;

namespace HomeInventory.Application.Houses.Commands.Locations.AddLocation;

public sealed record AddLocationCommand(Guid HouseId, string RoomName, string? ContainerName) : IRequest<Guid>;