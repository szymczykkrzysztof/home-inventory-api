using HomeInventory.Application.Tests.Infrastructure;
using HomeInventory.Domain.Aggregates.House;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Repositories;

namespace HomeInventory.Application.Tests.Houses.Queries;

public class GetItemsQueryTests
{
    [Fact]
    public async Task GetItemsReturnsItemsForGivenHouse()
    {
        await using var context = TestDbContextFactory.Create();

        var house = House.Create("Test House");

        var livingRoomId = house.AddLocation(Room.Create("Living Room"), null);
        var kitchenId = house.AddLocation(
            Room.Create("Kitchen"),
            Container.Create("Drawer"));

        house.GetLocation(livingRoomId)
            .AddItem("Laptop", "img1");

        house.GetLocation(kitchenId)
            .AddItem("Spoon", "img2");

        context.Houses.Add(house);
        await context.SaveChangesAsync();

        var repository = new HouseReadRepository(context);

        // Act
        var result = await repository.GetItems(
            house.Id,
            searchTerm: null,
            roomName: null,
            containerName: null,
            CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.Select(x => x.Name)
            .Should().Contain(new[] {"Laptop", "Spoon"});
    }

    [Fact]
    public async Task GetItemsFiltersByRoomContainerAndSearchTerm()
    {
        await using var context = TestDbContextFactory.Create();

        var house = House.Create("Test House");

        var livingRoomId = house.AddLocation(
            Room.Create("Living Room"),
            Container.Create("Shelf"));

        var kitchenDrawerId = house.AddLocation(
            Room.Create("Kitchen"),
            Container.Create("Drawer"));

        var kitchenShelfId = house.AddLocation(
            Room.Create("Kitchen"),
            Container.Create("Shelf"));

        house.GetLocation(livingRoomId)
            .AddItem("TV Remote", "img1");

        house.GetLocation(kitchenDrawerId)
            .AddItem("Spoon", "img2");

        house.GetLocation(kitchenShelfId)
            .AddItem("Coffee Spoon", "img3");

        context.Houses.Add(house);
        await context.SaveChangesAsync();

        var repository = new HouseReadRepository(context);

        // Act
        var result = await repository.GetItems(
            house.Id,
            searchTerm: "Spoon",
            roomName: "Kitchen",
            containerName: "Shelf",
            CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);

        var item = result.Single();
        item.Name.Should().Be("Coffee Spoon");
        item.RoomName.Should().Be("Kitchen");
        item.ContainerName.Should().Be("Shelf");
    }
}