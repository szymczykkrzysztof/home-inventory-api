using HomeInventory.Application.Contracts;
using HomeInventory.Infrastructure.Authorization.Entity;
using HomeInventory.Infrastructure.Authorization.Services;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Repositories;
using HomeInventory.Infrastructure.Seeders;
using Microsoft.AspNetCore.Identity;
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
                options.UseSqlServer(configuration.GetConnectionString("HomeInventoryDb")));
            services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("IdentityDb")));

            services
                .AddIdentityCore<User>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders();
        }

        services.AddScoped<IHomeInventorySeeder, HomeInventorySeeder>();
        services.AddScoped<IHouseRepository, HouseRepository>();
        services.AddScoped<IHouseReadRepository, HouseReadRepository>();
        services.AddScoped<IAuthenticationService, IdentityAuthenticationService>();
        services.AddScoped<IUserRegistrationService, IdentityUserRegistrationService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        return services;
    }
}