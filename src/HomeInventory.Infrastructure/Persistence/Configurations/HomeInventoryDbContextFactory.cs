using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HomeInventory.Infrastructure.Persistence.Configurations;

public class HomeInventoryDbContextFactory
    : IDesignTimeDbContextFactory<HomeInventoryDbContext>
{
    public HomeInventoryDbContext CreateDbContext(string[] args)
    {
        DotNetEnv.Env.TraversePath().Load();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
        
        var connectionString =
            configuration.GetConnectionString("HomeInventoryDb")
            ?? configuration["HOMEINVENTORY_CONNECTIONSTRING"];

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'HomeInventoryDb' not found.");
        }
        
        var optionsBuilder = new DbContextOptionsBuilder<HomeInventoryDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new HomeInventoryDbContext(optionsBuilder.Options);
    }
}