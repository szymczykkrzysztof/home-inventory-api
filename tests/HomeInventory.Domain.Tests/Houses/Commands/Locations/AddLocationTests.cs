using FluentAssertions;
using HomeInventory.Application.Houses.Commands.Locations.AddLocation;
using HomeInventory.Domain.Aggregates.House;
using HomeInventory.Domain.Exceptions;
using HomeInventory.Domain.Tests.TestDoubles;

namespace HomeInventory.Domain.Tests.Houses.Commands.Locations;

public class AddLocationTests
{
    [Fact]
    public async Task ShouldAddLocationToHouse()
    {
        var repository = new FakeHouseRepository();
        var handler = new AddLocationCommandHandler(repository);

        var house = House.Create("Test House");
        await repository.Add(house, default);

        var command = new AddLocationCommand(
            house.Id,
            "Living Room",
            null);
        var locationId = await handler.Handle(command, default);
        var updatedHouse = await repository.Get(house.Id, default);
        updatedHouse!.Locations.Should().Contain(l => l.Id == locationId);
    }

    [Fact]
    public async Task ShouldPersistChanges()
    {
        var repository = new FakeHouseRepository();
        var handler = new AddLocationCommandHandler(repository);
        var house = House.Create("Test House");
        await repository.Add(house, default);

        var command = new AddLocationCommand(
            house.Id,
            "Living Room",
            null);
        await handler.Handle(command, default);
        var persistedHouse = await repository.Get(house.Id, default);
        persistedHouse!.Locations.Should().HaveCount(1);
    }

    [Fact]
    public async Task ShouldThrowExceptionWhenHouseDoesnotExist()
    {
        var repository = new FakeHouseRepository();
        var handler = new AddLocationCommandHandler(repository);

        var act = async () =>
            await handler.Handle(new AddLocationCommand(Guid.NewGuid(), "Living Room", null), default);

        await act.Should().ThrowAsync<DomainException>();
    }
}