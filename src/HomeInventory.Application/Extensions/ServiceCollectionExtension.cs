using HomeInventory.Application.Houses.Commands.Manage.Register;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Application.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var applicationAssembly = typeof(ServiceCollectionExtension).Assembly;
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));
        return services;
    }
}