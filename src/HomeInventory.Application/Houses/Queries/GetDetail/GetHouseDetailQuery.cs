using MediatR;

namespace HomeInventory.Application.Houses.Queries.GetDetail;

public sealed record GetHouseDetailQuery(Guid HouseId) : IRequest<HouseDetailDto>;