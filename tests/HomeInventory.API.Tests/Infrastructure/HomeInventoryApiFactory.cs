using HomeInventory.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HomeInventory.API.Tests.Infrastructure;

public sealed class HomeInventoryApiFactory
    : WebApplicationFactory<Program>
{
    private SqliteConnection _connection = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureServices(services =>
        {
            // 1️⃣ Usuń WSZYSTKIE rejestracje DbContext
            services.RemoveAll<DbContextOptions<HomeInventoryDbContext>>();

            // 2️⃣ Utwórz JEDNO współdzielone połączenie
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            // 3️⃣ Zarejestruj DbContext z TYM połączeniem
            services.AddDbContext<HomeInventoryDbContext>(options =>
                options.UseSqlite(_connection));

            // 4️⃣ Zainicjalizuj bazę NA TYM SAMYM providerze
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider
                .GetRequiredService<HomeInventoryDbContext>();

            db.Database.EnsureCreated();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _connection?.Dispose();
        }
    }
}