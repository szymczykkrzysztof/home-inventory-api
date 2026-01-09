using FluentAssertions;
using HomeInventory.Application.Houses.Commands.Items.RemoveItem;
using HomeInventory.Application.Tests.TestDoubles;
using HomeInventory.Domain.Aggregates.House;
using HomeInventory.Domain.Exceptions;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Application.Tests.Houses.Commands.Items;

public class RemoveItemTests
{
    [Fact]
    public async Task ShouldRemoveItemFromHouse()
    {
        var repository = new FakeHouseRepository();
        var handler = new RemoveItemCommandHandler(repository);

        var house = House.Create("My House");
        var locationId = house.AddLocation(
            Room.Create("Living Room"),
            null);

        var itemId = house
            .GetLocation(locationId)
            .AddItem("Test Item", "https://example.com/test.jpg");
        await repository.Add(house, default);

        var command = new RemoveItemCommand(house.Id, locationId, itemId);
        await handler.Handle(command, default);

        var updatedHouse = await repository.Get(house.Id, default);

        updatedHouse!
            .GetLocation(locationId)
            .Items.Should()
            .NotContain(i => i.Id == itemId);
    }

    [Fact]
    public async Task ShouldPersistChanges()
    {
        var repository = new FakeHouseRepository();
        var handler = new RemoveItemCommandHandler(repository);
        var house = House.Create("My House");
        var locationId = house.AddLocation(
            Room.Create("Living Room"),
            null);

        var itemId = house
            .GetLocation(locationId)
            .AddItem("Test Item", "https://example.com/test.jpg");
        await repository.Add(house, default);

        var command = new RemoveItemCommand(house.Id, locationId, itemId);
        await handler.Handle(command, default);

        var persistedHouse = await repository.Get(house.Id, default);
        persistedHouse!
            .GetLocation(locationId)
            .Items.Should()
            .BeEmpty();
    }

    [Fact]
    public async Task ShouldThrowExceptionWhenHouseDoesNotExist()
    {
        var repository = new FakeHouseRepository();
        var handler = new RemoveItemCommandHandler(repository);
        var command = new RemoveItemCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        var act = async () => await handler.Handle(command, default);

        await act.Should().ThrowAsync<DomainException>();
    }
}