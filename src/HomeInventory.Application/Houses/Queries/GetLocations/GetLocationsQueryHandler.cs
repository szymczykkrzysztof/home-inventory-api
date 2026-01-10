using HomeInventory.Application.Contracts;
using MediatR;

namespace HomeInventory.Application.Houses.Queries.GetLocations;

public class GetLocationsQueryHandler(IHouseReadRepository repository)
    : IRequestHandler<GetLocationsQuery, List<LocationDto>>
{
    public async Task<List<LocationDto>> Handle(GetLocationsQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetLocations(request.HouseId, cancellationToken);
    }
}