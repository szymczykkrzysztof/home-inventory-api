using HomeInventory.Application.Contracts;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HomeInventory.Infrastructure.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddInfrastructureIfNotTest(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        if (!environment.IsEnvironment("Test"))
        {
            services.AddDbContext<HomeInventoryDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("HomeInventoryDb"),
                        o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
                    .LogTo(Console.WriteLine));
        }

        services.AddScoped<IHouseRepository, HouseRepository>();
        services.AddScoped<IHouseReadRepository, HouseReadRepository>();
        return services;
    }
}