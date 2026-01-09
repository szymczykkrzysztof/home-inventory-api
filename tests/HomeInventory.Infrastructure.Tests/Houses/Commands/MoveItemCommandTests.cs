using HomeInventory.Application.Contracts;
using HomeInventory.Application.Houses.Commands.Items.MoveItem;
using HomeInventory.Application.Tests.Infrastructure;
using HomeInventory.Domain.Aggregates.House;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Repositories;

namespace HomeInventory.Application.Tests.Houses.Commands;

public class MoveItemCommandTests
{
    [Fact]
    public async Task ShouldMoveItemBetweenLocations()
    {
        var dbContext = TestDbContextFactory.Create();
        IHouseRepository repository = new HouseRepository(dbContext);
        var house = House.Create("Test House");
        var fromLocationId = house.AddLocation(Room.Create("Living Room"), null);
        var toLocationId = house.AddLocation(Room.Create("Kitchen"), null);
        var itemId = house.GetLocation(fromLocationId).AddItem("Test Item", "https://example.com/test.jpg");
        await repository.Add(house, default);
        await repository.SaveChanges(default);

        var handler = new MoveItemCommandHandler(repository);
        var command = new MoveItemCommand(house.Id, itemId, fromLocationId, toLocationId);
        await handler.Handle(command, default);
        var reloadedHouse = await repository.Get(house.Id, CancellationToken.None);
        reloadedHouse.Should().NotBeNull();

        reloadedHouse!
            .GetLocation(fromLocationId)
            .Items.Should().BeEmpty();

        reloadedHouse
            .GetLocation(toLocationId)
            .Items.Should().ContainSingle(i => i.Id == itemId);
    }
}