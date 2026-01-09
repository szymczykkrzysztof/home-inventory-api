using FluentAssertions;
using HomeInventory.Application.Houses.Commands.Items.UpdateItem;
using HomeInventory.Application.Tests.TestDoubles;
using HomeInventory.Domain.Aggregates.House;
using HomeInventory.Domain.Exceptions;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Application.Tests.Houses.Commands.Items;

public class UpdateItemTests
{
    [Fact]
    public async Task ShouldUpdateItemNameAndImage()
    {
        var repository = new FakeHouseRepository();
        var handler = new UpdateItemCommandHandler(repository);

        var house = House.Create("My House");
        var locationId = house.AddLocation(
            Room.Create("Living Room"),
            null);

        var itemId = house
            .GetLocation(locationId)
            .AddItem("Test Item", "https://example.com/test.jpg");

        await repository.Add(house, default);

        var command = new UpdateItemCommand(house.Id, locationId, itemId, "New Test Item",
            "https://example.com/new-test.jpg");
        await handler.Handle(command, default);

        var updatedHouse = await repository.Get(house.Id, default);
        var updatedItem = updatedHouse!
            .GetLocation(locationId)
            .GetItem(itemId);

        updatedItem.Name.Should().Be("New Test Item");
        updatedItem.ImageUrl.Should().Be("https://example.com/new-test.jpg");
    }

    [Fact]
    public async Task ShouldPersistChanges()
    {
        var repository = new FakeHouseRepository();
        var handler = new UpdateItemCommandHandler(repository);

        var house = House.Create("My House");
        var locationId = house.AddLocation(
            Room.Create("Living Room"),
            null);

        var itemId = house
            .GetLocation(locationId)
            .AddItem("Test Item", "https://example.com/test.jpg");

        await repository.Add(house, default);

        var command = new UpdateItemCommand(house.Id, locationId, itemId, "New Test Item",
            "https://example.com/new-test.jpg");
        await handler.Handle(command, default);

        var persistedHouse = await repository.Get(house.Id, default);

        persistedHouse!
            .GetLocation(locationId)
            .GetItem(itemId)
            .Name.Should().Be("New Test Item");
    }

    [Fact]
    public async Task ShouldThrowExceptionWhenHouseDoesNotExist()
    {
        var repository = new FakeHouseRepository();
        var handler = new UpdateItemCommandHandler(repository);
        var command = new UpdateItemCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Test Item",
            "https://example.com/test.jpg");

        var act = async () => await handler.Handle(command, default);

        await act.Should().ThrowAsync<DomainException>();
    }
}