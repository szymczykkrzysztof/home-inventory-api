using DotNetEnv;
using HomeInventory.Application.Extensions;
using HomeInventory.Infrastructure.Extensions;
using Microsoft.OpenApi;

Env.Load();
Env.TraversePath().Load();
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructureIfNotTest(builder.Configuration, builder.Environment);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Home Inventory API",
        Version = "v1",
        Description = "REST API for managing home inventory (DDD + CQRS)"
    });
});
var app = builder.Build();
if (!app.Environment.IsEnvironment("Test"))
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Home Inventory API v1");
        options.RoutePrefix = "swagger"; // /swagger
    });
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();