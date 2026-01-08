using HomeInventory.Application.Contracts;
using HomeInventory.Application.Houses.Queries.GetDetail;
using ItemDto = HomeInventory.Application.Houses.Queries.GetItems.ItemDto;

namespace HomeInventory.Domain.Tests.TestDoubles;

public sealed class FakeHouseReadRepository : IHouseReadRepository
{
    public HouseDetailDto? HouseDetail { get; set; }
    public IReadOnlyList<ItemDto> Items { get; set; } = [];

    public Task<HouseDetailDto?> GetHouseDetail(Guid houseId, CancellationToken cancellationToken) =>
        Task.FromResult(HouseDetail);


    public Task<IReadOnlyList<ItemDto>> GetItems(Guid houseId, string? searchTerm, string? roomName,
        string? containerName,
        CancellationToken cancellationToken) => Task.FromResult(Items);
}