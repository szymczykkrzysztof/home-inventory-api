using HomeInventory.Application.Contracts;
using HomeInventory.Application.Houses.Queries.GetDetail;
using ItemDto = HomeInventory.Application.Houses.Queries.GetItems.ItemDto;
using LocationDto = HomeInventory.Application.Houses.Queries.GetLocations.LocationDto;
using HouseDto = HomeInventory.Application.Houses.Queries.GetHouses.HouseLookupDto;

namespace HomeInventory.Application.Tests.TestDoubles;

public sealed class FakeHouseReadRepository : IHouseReadRepository
{
    public List<HouseDto> Houses { get; set; } = [];
    public HouseDetailDto? HouseDetail { get; set; }
    public IReadOnlyList<ItemDto> Items { get; set; } = [];
    public List<LocationDto> Locations { get; set; } = [];

    public Task<HouseDetailDto?> GetHouseDetail(Guid houseId, CancellationToken cancellationToken) =>
        Task.FromResult(HouseDetail);


    public Task<IReadOnlyList<ItemDto>> GetItems(Guid houseId, string? searchTerm, string? roomName,
        string? containerName,
        CancellationToken cancellationToken) => Task.FromResult(Items);

    public Task<List<LocationDto>> GetLocations(Guid houseId, CancellationToken cancellationToken) =>
        Task.FromResult(Locations);

    public Task<List<HouseDto>> GetHouses(CancellationToken cancellationToken)
        => Task.FromResult(Houses);
}