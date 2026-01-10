using MediatR;

namespace HomeInventory.Application.Houses.Queries.GetLocations;

public record GetLocationsQuery(Guid HouseId) : IRequest<List<LocationDto>>;