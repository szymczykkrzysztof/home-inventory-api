using FluentAssertions;
using HomeInventory.Application.Houses.Commands.Items.MoveItem;
using HomeInventory.Domain.Aggregates.House;
using HomeInventory.Domain.Tests.TestDoubles;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Tests.Houses.Commands.Items;

public class MoveItemTests
{
    [Fact]
    public async Task ShouldMoveItemBetweenLocations()
    {
        var repository = new FakeHouseRepository();
        var handler = new MoveItemCommandHandler(repository);

        var house = House.Create("Test House");
        var fromId = house.AddLocation(Room.Create("Living"), null);
        var toId = house.AddLocation(Room.Create("Kitchen"), null);

        var itemId = house.GetLocation(fromId).AddItem("Test Item", "https://example.com/test.jpg");
        await repository.Add(house, default);
        var command = new MoveItemCommand(house.Id, itemId, fromId, toId);
        await handler.Handle(command, default);

        var updatedHouse = await repository.Get(house.Id, default);

        updatedHouse!.GetLocation(fromId)
            .Items.Should().NotContain(i => i.Id == itemId);

        updatedHouse!.GetLocation(toId)
            .Items.Should().Contain(i => i.Id == itemId);
    }
}