using HomeInventory.Application.Contracts;
using HomeInventory.Domain.Aggregates.House;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Persistence.Repositories;

public class HouseRepository(HomeInventoryDbContext dbContext) : IHouseRepository
{
    public async Task<House?> Get(Guid houseId, CancellationToken cancellationToken)
    {
        return await dbContext
            .Set<House>()
            .Include(h => h.Locations)
            .ThenInclude(l => l.Items)
            .SingleOrDefaultAsync(h => h.Id == houseId, cancellationToken);
    }

    public async Task Add(House house, CancellationToken cancellationToken)
    {
        await dbContext.Set<House>().AddAsync(house, cancellationToken);
    }

    public async Task SaveChanges(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}