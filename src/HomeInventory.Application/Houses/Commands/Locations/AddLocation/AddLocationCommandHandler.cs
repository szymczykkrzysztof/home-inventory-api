using HomeInventory.Application.Contracts;
using HomeInventory.Domain.Exceptions;
using HomeInventory.Domain.ValueObjects;
using MediatR;

namespace HomeInventory.Application.Houses.Commands.Locations.AddLocation;

public class AddLocationCommandHandler(IHouseRepository houseRepository) : IRequestHandler<AddLocationCommand, Guid>
{
    public async Task<Guid> Handle(AddLocationCommand request, CancellationToken cancellationToken)
    {
        var house = await houseRepository.Get(request.HouseId, cancellationToken) ??
                    throw new DomainException($"House with id:{request.HouseId} not found.");
        var room = Room.Create(request.RoomName);
        var container = string.IsNullOrWhiteSpace(request.ContainerName)
            ? null
            : Container.Create(request.ContainerName);
        var locationId = house.AddLocation(room, container);
        await houseRepository.SaveChanges(cancellationToken);


        return locationId;
    }
}