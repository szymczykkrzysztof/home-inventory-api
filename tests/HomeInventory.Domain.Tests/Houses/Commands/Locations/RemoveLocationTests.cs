using FluentAssertions;
using HomeInventory.Application.Houses.Commands.Locations.RemoveLocation;
using HomeInventory.Domain.Aggregates.House;
using HomeInventory.Domain.Exceptions;
using HomeInventory.Domain.Tests.TestDoubles;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Tests.Houses.Commands.Locations;

public class RemoveLocationTests
{
    [Fact]
    public async Task ShouldRemoveLocationFromHouse()
    {
        var repository = new FakeHouseRepository();
        var handler = new RemoveLocationCommandHandler(repository);

        var house = House.Create("Test House");
        var locationId = house.AddLocation(Room.Create("Living Room"), null);
        await repository.Add(house, default);

        var command = new RemoveLocationCommand(house.Id, locationId);
        await handler.Handle(command, default);
        var updatedHouse = await repository.Get(house.Id, default);

        updatedHouse!.Locations
            .Should()
            .NotContain(l => l.Id == locationId);
    }

    [Fact]
    public async Task ShouldPersistChanges()
    {
        var repository = new FakeHouseRepository();
        var handler = new RemoveLocationCommandHandler(repository);

        var house = House.Create("Test House");
        var locationId = house.AddLocation(Room.Create("Living Room"), null);
        await repository.Add(house, default);

        var command = new RemoveLocationCommand(house.Id, locationId);
        await handler.Handle(command, default);

        var persistedHouse = await repository.Get(house.Id, default);
        persistedHouse!.Locations.Should().BeEmpty();
    }

    [Fact]
    public async Task ShouldThrowExceptionWhenHouseDoesNotExist()
    {
        var repository = new FakeHouseRepository();
        var handler = new RemoveLocationCommandHandler(repository);

        var act = async () => await handler.Handle(
            new RemoveLocationCommand(Guid.NewGuid(), Guid.NewGuid()),
            default);

        await act.Should().ThrowAsync<DomainException>();
    }
}