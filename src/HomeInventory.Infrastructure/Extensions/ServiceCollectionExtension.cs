using HomeInventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace HomeInventory.Infrastructure.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<HomeInventoryDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("HomeInventoryDb"));
        });
        return services;
    }
}