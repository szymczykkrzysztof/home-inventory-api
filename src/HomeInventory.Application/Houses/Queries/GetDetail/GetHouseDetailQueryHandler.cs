using HomeInventory.Application.Contracts;
using HomeInventory.Domain.Exceptions;
using MediatR;

namespace HomeInventory.Application.Houses.Queries.GetDetail;

public class GetHouseDetailQueryHandler(IHouseReadRepository houseReadRepository)
    : IRequestHandler<GetHouseDetailQuery, HouseDetailDto>
{
    public async Task<HouseDetailDto> Handle(GetHouseDetailQuery request, CancellationToken cancellationToken)
    {
        var house = await houseReadRepository.GetHouseDetail(
            request.HouseId,
            cancellationToken);
        return house ?? throw new NotFoundException("House", request.HouseId);
    }
}