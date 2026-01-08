using HomeInventory.Application.Contracts;
using HomeInventory.Domain.Aggregates.House;

namespace HomeInventory.Domain.Tests.TestDoubles;

public class FakeHouseRepository : IHouseRepository
{
    private readonly Dictionary<Guid, House> _store = new();

    public Task<House?> Get(Guid houseId, CancellationToken cancellationToken)
    {
        _store.TryGetValue(houseId, out var house);
        return Task.FromResult(house);
    }

    public Task Add(House house, CancellationToken cancellationToken)
    {
        _store[house.Id] = house;
        return Task.CompletedTask;
    }

    public Task SaveChanges(CancellationToken cancellationToken) => Task.CompletedTask;
}