using HomeInventory.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Application.Tests.Infrastructure;

internal static class TestDbContextFactory
{
    public static HomeInventoryDbContext Create()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<HomeInventoryDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new HomeInventoryDbContext(options);
        context.Database.EnsureCreated();

        return context;
    }
}