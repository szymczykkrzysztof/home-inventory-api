using HomeInventory.Application.Contracts;
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
        if (house is null)
            throw new InvalidOperationException(
                $"House with id {request.HouseId} not found.");

        return house;
    }
}