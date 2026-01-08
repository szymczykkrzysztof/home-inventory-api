using HomeInventory.Application.Houses.Queries.GetDetail;
using ItemDto = HomeInventory.Application.Houses.Queries.GetItems.ItemDto;

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
}