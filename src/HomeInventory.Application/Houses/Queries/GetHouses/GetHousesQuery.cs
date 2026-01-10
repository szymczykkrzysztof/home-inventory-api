using MediatR;

namespace HomeInventory.Application.Houses.Queries.GetHouses;

public record GetHousesQuery : IRequest<List<HouseLookupDto>>;