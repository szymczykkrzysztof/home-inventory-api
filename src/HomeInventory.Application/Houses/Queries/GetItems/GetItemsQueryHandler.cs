using HomeInventory.Application.Contracts;
using MediatR;

namespace HomeInventory.Application.Houses.Queries.GetItems;

public sealed class GetItemsQueryHandler(IHouseReadRepository houseReadRepository)
    : IRequestHandler<GetItemsQuery, IReadOnlyList<ItemDto>>
{
    public async Task<IReadOnlyList<ItemDto>> Handle(GetItemsQuery request, CancellationToken cancellationToken)
    {
        return await houseReadRepository.GetItems(
            request.HouseId,
            request.SearchTerm,
            request.RoomName,
            request.ContainerName,
            cancellationToken);
    }
}