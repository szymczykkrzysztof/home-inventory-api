using FluentAssertions;
using HomeInventory.Application.Houses.Commands.Items.AddItem;
using HomeInventory.Application.Tests.TestDoubles;
using HomeInventory.Domain.Aggregates.House;
using HomeInventory.Domain.Exceptions;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Application.Tests.Houses.Commands.Items;

public class AddItemTests
{
    [Fact]
    public async Task ShouldAddItemToHouse()
    {
        var repository = new FakeHouseRepository();
        var handler = new AddItemCommandHandler(repository);

        var house = House.Create("Test House");
        var locationId = house.AddLocation(Room.Create("Living Room"), null);
        await repository.Add(house, default);

        var command = new AddItemCommand(house.Id, locationId, "Test Item", "https://example.com/test.jpg");
        var itemId = await handler.Handle(command, default);
        var updatedHouse = await repository.Get(house.Id, default);
        updatedHouse!
            .GetLocation(locationId)
            .Items.Should()
            .Contain(i => i.Id == itemId);
    }

    [Fact]
    public async Task ShouldPersisChanges()
    {
        var repository = new FakeHouseRepository();
        var handler = new AddItemCommandHandler(repository);

        var house = House.Create("Test House");
        var locationId = house.AddLocation(
            Room.Create("Living Room"),
            null);
        await repository.Add(house, default);

        var command = new AddItemCommand(house.Id, locationId, "Test Item", "https://example.com/test.jpg");
        await handler.Handle(command, default);

        var persistedHouse = await repository.Get(house.Id, default);

        persistedHouse!
            .GetLocation(locationId)
            .Items.Should()
            .HaveCount(1);
    }

    [Fact]
    public async Task ShouldThrowExceptionWhenHouseDoesNotExist()
    {
        var repository = new FakeHouseRepository();
        var handler = new AddItemCommandHandler(repository);
        var command = new AddItemCommand(Guid.NewGuid(), Guid.NewGuid(), "Test Item", "https://example.com/test.jpg");

        var act = async () => await handler.Handle(command, default);
        await act.Should().ThrowAsync<DomainException>();
    }
}