using HomeInventory.Application.Contracts;
using HomeInventory.Application.Houses.Queries.GetDetail;
using HomeInventory.Domain.Aggregates.House;
using Microsoft.EntityFrameworkCore;


namespace HomeInventory.Infrastructure.Persistence.Repositories;

public class HouseReadRepository(HomeInventoryDbContext dbContext) : IHouseReadRepository
{
    public async Task<HouseDetailDto?> GetHouseDetail(Guid houseId, CancellationToken cancellationToken)
    {
        return await dbContext
            .Set<Domain.Aggregates.House.House>()
            .AsNoTracking()
            .Where(h => h.Id == houseId)
            .Select(h => new HouseDetailDto
            {
                HouseId = h.Id,
                Name = h.Name,
                Locations = h.Locations.Select(l => new LocationDto
                {
                    LocationId = l.Id,
                    RoomName = l.Room.Name,
                    ContainerName = l.Container != null ? l.Container.Name : null,
                    Items = l.Items.Select(i => new ItemDto
                    {
                        ItemId = i.Id,
                        Name = i.Name,
                        ImageUrl = i.ImageUrl
                    }).ToList()
                }).ToList()
            })
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Application.Houses.Queries.GetItems.ItemDto>> GetItems(Guid houseId, string? searchTerm,
        string? roomName, string? containerName,
        CancellationToken cancellationToken)
    {
        var query =
            from i in dbContext.Set<Item>()
            join l in dbContext.Set<Location>() on
                EF.Property<Guid>(i, "LocationId") equals l.Id
            where EF.Property<Guid>(l, "HouseId") == houseId
            select new Application.Houses.Queries.GetItems.ItemDto
            {
                ItemId = i.Id,
                Name = i.Name,
                ImageUrl = i.ImageUrl,
                RoomName = l.Room.Name,
                ContainerName = l.Container != null ? l.Container.Name : null
            };

        if (!string.IsNullOrWhiteSpace(roomName))
            query = query.Where(x => x.RoomName == roomName);

        if (!string.IsNullOrWhiteSpace(containerName))
            query = query.Where(x => x.ContainerName == containerName);

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(x => x.Name.Contains(searchTerm));

        return await query.ToListAsync(cancellationToken);
    }
}