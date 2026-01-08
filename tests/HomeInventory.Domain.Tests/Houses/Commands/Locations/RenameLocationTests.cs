using FluentAssertions;
using HomeInventory.Application.Houses.Commands.Locations.RenameLocation;
using HomeInventory.Domain.Aggregates.House;
using HomeInventory.Domain.Exceptions;
using HomeInventory.Domain.Tests.TestDoubles;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Tests.Houses.Commands.Locations;

public class RenameLocationTests
{
    [Fact]
    public async Task ShouldRenameLocation()
    {
        var repository = new FakeHouseRepository();
        var handler = new RenameLocationCommandHandler(repository);

        var house = House.Create("Test House");
        var locationId = house.AddLocation(Room.Create("Living Room"), Container.Create("Drawer"));

        await repository.Add(house, default);

        await handler.Handle(new RenameLocationCommand(house.Id, locationId, "Kitchen", "Cabinet"), default);

        var updatedHouse = await repository.Get(house.Id, default);
        var location = updatedHouse!.GetLocation(locationId);

        location.Room.Name.Should().Be("Kitchen");
        location.Container?.Name.Should().Be("Cabinet");
    }

    [Fact]
    public async Task ShouldRemoveContainerWhenNullPassed()
    {
        var repository = new FakeHouseRepository();
        var handler = new RenameLocationCommandHandler(repository);
        var house = House.Create("Test House");
        var locationId = house.AddLocation(Room.Create("Living Room"), Container.Create("Drawer"));

        await repository.Add(house, default);

        await handler.Handle(new RenameLocationCommand(house.Id, locationId, "Kitchen", null), default);

        var updatedHouse = await repository.Get(house.Id, default);
        var location = updatedHouse!.GetLocation(locationId);

        location.Container.Should().BeNull();
    }

    [Fact]
    public async Task ShouldThrowExceptionWhenHouseDoesNotExist()
    {
        var repository = new FakeHouseRepository();
        var handler = new RenameLocationCommandHandler(repository);
        var command = new RenameLocationCommand(Guid.NewGuid(), Guid.NewGuid(), "Kitchen", null);
        var act = async () => await handler.Handle(command, default);
        await act.Should().ThrowAsync<DomainException>();
    }
}