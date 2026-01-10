using HomeInventory.Application.Contracts;
using MediatR;

namespace HomeInventory.Application.Houses.Queries.GetHouses;

public class GetHousesHandler(IHouseReadRepository repository)
    : IRequestHandler<GetHousesQuery, List<HouseLookupDto>>
{
    public async Task<List<HouseLookupDto>> Handle(GetHousesQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetHouses(cancellationToken);
    }
}