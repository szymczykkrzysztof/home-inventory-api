using HomeInventory.Domain.Aggregates.House;

namespace HomeInventory.Application.Contracts;

public interface IHouseRepository
{
    Task<House?> Get(Guid houseId, CancellationToken cancellationToken);
    Task Add(House house, CancellationToken cancellationToken);
    Task SaveChanges(CancellationToken cancellationToken);
}