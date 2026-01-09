using HomeInventory.Domain.Aggregates.House;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Persistence;

public class HomeInventoryDbContext(DbContextOptions<HomeInventoryDbContext> options) : DbContext(options)
{
    public DbSet<House> Houses => Set<House>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HomeInventoryDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}