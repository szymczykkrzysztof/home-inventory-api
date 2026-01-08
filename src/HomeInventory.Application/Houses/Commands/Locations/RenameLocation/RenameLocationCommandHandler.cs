using HomeInventory.Application.Contracts;
using HomeInventory.Domain.Exceptions;
using HomeInventory.Domain.ValueObjects;
using MediatR;

namespace HomeInventory.Application.Houses.Commands.Locations.RenameLocation;

public class RenameLocationCommandHandler(IHouseRepository houseRepository) : IRequestHandler<RenameLocationCommand>
{
    public async Task Handle(RenameLocationCommand request, CancellationToken cancellationToken)
    {
        var house = await houseRepository.Get(request.HouseId, cancellationToken) ??
                    throw new DomainException($"House with id {request.HouseId} not found.");
        var newRoom = Room.Create(request.RoomName);
        var newContainer = string.IsNullOrWhiteSpace(request.ContainerName)
            ? null
            : Container.Create(request.ContainerName);
        house.UpdateLocation(request.LocationId, newRoom, newContainer);
        await houseRepository.SaveChanges(cancellationToken);
    }
}