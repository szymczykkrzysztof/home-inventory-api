using HomeInventory.Application.Contracts;
using HomeInventory.Domain.Exceptions;
using MediatR;

namespace HomeInventory.Application.Houses.Commands.Items.RemoveItem;

public class RemoveItemCommandHandler(IHouseRepository houseRepository) : IRequestHandler<RemoveItemCommand>
{
    public async Task Handle(RemoveItemCommand request, CancellationToken cancellationToken)
    {
        var house = await houseRepository.Get(request.HouseId, cancellationToken) ??
                    throw new DomainException($"House with id:{request.HouseId} not found.");
        var location = house.GetLocation(request.LocationId);
        location.RemoveItem(request.ItemId);
        await houseRepository.SaveChanges(cancellationToken);
    }
}