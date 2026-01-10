using HomeInventory.Domain.Aggregates.House;
using HomeInventory.Infrastructure.Persistence.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Persistence;

public class HomeInventoryDbContext(DbContextOptions<HomeInventoryDbContext> options) : DbContext(options)
{
    public DbSet<House> Houses => Set<House>();
    public DbSet<Location> Locations => Set<Location>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HomeInventoryDbContext).Assembly);
        modelBuilder
            .Entity<ItemReadModel>()
            .HasNoKey()
            .ToView(null); 
        base.OnModelCreating(modelBuilder);
    }
}