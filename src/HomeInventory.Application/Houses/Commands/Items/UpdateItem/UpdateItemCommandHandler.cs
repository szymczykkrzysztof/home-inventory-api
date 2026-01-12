using HomeInventory.Application.Contracts;
using HomeInventory.Domain.Exceptions;
using MediatR;

namespace HomeInventory.Application.Houses.Commands.Items.UpdateItem;

public class UpdateItemCommandHandler(IHouseRepository houseRepository) : IRequestHandler<UpdateItemCommand>
{
    public async Task Handle(UpdateItemCommand request, CancellationToken cancellationToken)
    {
        var house = await houseRepository.Get(request.HouseId, cancellationToken)
                    ?? throw new NotFoundException("House",request.HouseId);
        var location = house.GetLocation(request.LocationId);
        location.UpdateItem(request.ItemId, request.Name, request.ImageUrl);
        await houseRepository.SaveChanges(cancellationToken);
    }
}