using MediatR;

namespace HomeInventory.Application.Houses.Commands.Locations.RenameLocation;

public sealed record RenameLocationCommand(
    Guid HouseId,
    Guid LocationId,
    string RoomName,
    string? ContainerName) : IRequest;