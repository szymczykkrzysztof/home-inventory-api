using HomeInventory.Application.Contracts;
using HomeInventory.Domain.Exceptions;
using MediatR;

namespace HomeInventory.Application.Houses.Commands.Locations.RemoveLocation;

public sealed class RemoveLocationCommandHandler(IHouseRepository houseRepository)
    : IRequestHandler<RemoveLocationCommand>
{
    public async Task Handle(RemoveLocationCommand request, CancellationToken cancellationToken)
    {
        var house = await houseRepository.Get(request.HouseId, cancellationToken) ??
                    throw new DomainException($"House with id:{request.HouseId} not found.");
        house.RemoveLocation(request.LocationId);
        await houseRepository.SaveChanges(cancellationToken);
    }
}