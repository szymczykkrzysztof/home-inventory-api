using HomeInventory.Application.Contracts;
using HomeInventory.Domain.Exceptions;
using MediatR;

namespace HomeInventory.Application.Houses.Commands.Items.MoveItem;

public class MoveItemCommandHandler(IHouseRepository houseRepository) : IRequestHandler<MoveItemCommand>
{
    public async Task Handle(MoveItemCommand request, CancellationToken cancellationToken)
    {
        var house = await houseRepository.Get(request.HouseId, cancellationToken)
                    ?? throw new NotFoundException("House",request.HouseId);
        house.MoveItem(request.ItemId, request.FromLocationId, request.ToLocationId);
        await houseRepository.SaveChanges(cancellationToken);
    }
}