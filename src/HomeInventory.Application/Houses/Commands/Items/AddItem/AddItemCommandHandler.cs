using HomeInventory.Application.Contracts;
using HomeInventory.Domain.Exceptions;
using MediatR;

namespace HomeInventory.Application.Houses.Commands.Items.AddItem;

public class AddItemCommandHandler(IHouseRepository houseRepository) : IRequestHandler<AddItemCommand, Guid>
{
    public async Task<Guid> Handle(AddItemCommand request, CancellationToken cancellationToken)
    {
        var house = await houseRepository.Get(request.HouseId, cancellationToken) ??
                    throw new NotFoundException("House",request.HouseId);
        var location = house.GetLocation(request.LocationId);
        var itemId = location.AddItem(request.Name, request.ImageUrl);
        await houseRepository.SaveChanges(cancellationToken);
        return itemId;
    }
}