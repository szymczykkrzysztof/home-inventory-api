namespace HomeInventory.Infrastructure.Seeders;

public interface IHomeInventorySeeder
{
    Task SeedRolesAsync(IServiceProvider services);
}