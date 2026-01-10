using HomeInventory.Application.Houses.Queries.GetDetail;
using HomeInventory.Application.Houses.Queries.GetHouses;
using ItemDto = HomeInventory.Application.Houses.Queries.GetItems.ItemDto;
using LocationLookupDto = HomeInventory.Application.Houses.Queries.GetLocations.LocationDto;

namespace HomeInventory.Application.Contracts;

public interface IHouseReadRepository
{
    Task<HouseDetailDto?> GetHouseDetail(
        Guid houseId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<ItemDto>> GetItems(
        Guid houseId,
        string? searchTerm,
        string? roomName,
        string? containerName,
        CancellationToken cancellationToken);

    Task<List<LocationLookupDto>> GetLocations(Guid houseId, CancellationToken cancellationToken);
    Task<List<HouseLookupDto>> GetHouses(CancellationToken cancellationToken);
}