using DotNetEnv;
using HomeInventory.API.Extensions;
using HomeInventory.API.Middlewares;
using HomeInventory.Application.Extensions;
using HomeInventory.Infrastructure.Extensions;
using HomeInventory.Infrastructure.Seeders;

Env.Load();
Env.TraversePath().Load();
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddApplication();
builder.Services.AddInfrastructureIfNotTest(builder.Configuration, builder.Environment);
builder.AddPresentation();
var app = builder.Build();
var scope = app.Services.CreateScope();
var seeder = scope.ServiceProvider.GetRequiredService<IHomeInventorySeeder>();
await seeder.SeedRolesAsync(scope.ServiceProvider);
if (!app.Environment.IsEnvironment("Test"))
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Home Inventory API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();