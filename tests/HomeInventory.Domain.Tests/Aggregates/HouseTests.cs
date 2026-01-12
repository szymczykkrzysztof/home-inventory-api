using FluentAssertions;
using HomeInventory.Domain.Aggregates.House;
using HomeInventory.Domain.Exceptions;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Tests.Aggregates;

public class HouseTests
{
    [Fact]
    public void CanCreateHouseWithValidName()
    {
        var house = House.Create("Test House");
        house.Name.Should().Be("Test House");
    }

    [Fact]
    public void CannotCreateHouseWithEmptyName()
    {
        Action act = () => House.Create("");
        act.Should().Throw<DomainException>();
        act.Should().Throw<DomainException>("House name is required.");
    }

    [Fact]
    public void CannotCreateHouseWithNullName()
    {
        var act = () => House.Create(null);
        act.Should().Throw<BusinessRuleValidationException>();
        act.Should().Throw<BusinessRuleValidationException>("House name is required.");
    }

    [Fact]
    public void CannotCreateHouseWithWhiteSpaceName()
    {
        Action act = () => House.Create(" ");
        act.Should().Throw<BusinessRuleValidationException>();
        act.Should().Throw<BusinessRuleValidationException>("House name is required.");
    }

    [Fact]
    public void CannotCreateHouseWithStringEmptyName()
    {
        Action act = () => House.Create(string.Empty);
        act.Should().Throw<BusinessRuleValidationException>();
        act.Should().Throw<BusinessRuleValidationException>("House name is required.");
    }

    [Fact]
    public void CanAddLocationWithRoomOnly()
    {
        var house = House.Create("Test House");
        var locationId = house.AddLocation(Room.Create("Living Room"), null);

        house.Locations.Should().Contain(l => l.Id == locationId);
    }

    [Fact]
    public void CanAddLocationWithRoomAndContainer()
    {
        var house = House.Create("Test House");
        var locationId = house.AddLocation(Room.Create("Living Room"), Container.Create("Drawer"));

        house.Locations.Should().Contain(l => l.Id == locationId);
    }

    [Fact]
    public void CannotAddLocationWithNullRoom()
    {
        var house = House.Create("Test House");
        var act = () => house.AddLocation(null, null);
        act.Should().Throw<BusinessRuleValidationException>("Room is required");
    }

    [Fact]
    public void CannotAddDuplicateLocation()
    {
        var house = House.Create("Test House");
        house.AddLocation(Room.Create("Living Room"), null);

        Action act = () => house.AddLocation(Room.Create("Living Room"), null);
        act.Should().Throw<AlreadyExistsException>();
        act.Should().Throw<AlreadyExistsException>("Location already exists in this house (Room + Container).");
    }

    [Fact]
    public void CannotAddLocationWithSameRoomAndContainer()
    {
        var house = House.Create("Test House");
        house.AddLocation(Room.Create("Living Room"), Container.Create("Drawer"));
        var act = () => house.AddLocation(Room.Create("Living Room"), Container.Create("Drawer"));
        act.Should().Throw<AlreadyExistsException>();
        act.Should().Throw<AlreadyExistsException>("Location already exists in this house (Room + Container).");
    }

    [Fact]
    public void CanAddLocationsWithSameRoomDifferentContainers()
    {
        var house = House.Create("Test House");
        var drawerLocationId = house.AddLocation(Room.Create("Living Room"), Container.Create("Drawer"));
        var backpackLocationId = house.AddLocation(Room.Create("Living Room"), Container.Create("Backpack"));
        house.Locations.Should().HaveCount(2);
        house.Locations.Should().Contain(l => l.Id == drawerLocationId);
        house.Locations.Should().Contain(l => l.Id == backpackLocationId);
    }

    [Fact]
    public void CanGetExistingLocationById()
    {
        var house = House.Create("Test House");
        var locationId = house.AddLocation(Room.Create("Living Room"), null);
        var location = house.GetLocation(locationId);
        location.Id.Should().Be(locationId);
    }

    [Fact]
    public void CannotGetNonExistingLocationById()
    {
        var house = House.Create("Test House");
        var act = () => house.GetLocation(Guid.NewGuid());
        act.Should().Throw<NotFoundException>();
        act.Should().Throw<NotFoundException>("Location not found.");
    }

    [Fact]
    public void CanChangeContainerOfLocation()
    {
        var house = House.Create("Test House");
        var locationId = house.AddLocation(Room.Create("Living Room"), null);
        house.UpdateLocation(locationId, Room.Create("Living Room"), Container.Create("Drawer"));
        house.GetLocation(locationId).Container?.Name.Should().Be("Drawer");
        house.GetLocation(locationId).Room.Name.Should().Be("Living Room");
    }

    [Fact]
    public void CanRemoveContainerOfLocation()
    {
        var house = House.Create("Test House");
        var locationId = house.AddLocation(Room.Create("Living Room"), Container.Create("Drawer"));
        house.UpdateLocation(locationId, Room.Create("Living Room"), null);
        house.GetLocation(locationId).Container.Should().BeNull();
    }

    [Fact]
    public void CanRenameLocation()
    {
        var house = House.Create("Test House");
        var locationId = house.AddLocation(Room.Create("Living Room"), null);
        house.UpdateLocation(locationId, Room.Create("Kitchen"), null);
        house.Locations.Should().Contain(l => l.Id == locationId && l.Room.Name == "Kitchen");
        house.Locations.Should().NotContain(l => l.Id == locationId && l.Room.Name == "Living Room");
        house.GetLocation(locationId).Room.Name.Should().Be("Kitchen");
        house.GetLocation(locationId).Container.Should().NotBe("Living Room");
    }

    [Fact]
    public void CannotRenameLocationToDuplicate()
    {
        var house = House.Create("Test House");
        var locationId = house.AddLocation(Room.Create("Living Room"), null);
        house.AddLocation(Room.Create("Kitchen"), null);

        var act = () => house.UpdateLocation(locationId, Room.Create("Kitchen"), null);
        act.Should().Throw<AlreadyExistsException>();
    }

    [Fact]
    public void CannotRenameLocationToEmptyRoomName()
    {
        var house = House.Create("Test House");
        var locationId = house.AddLocation(Room.Create("Living Room"), null);
        var act = () => house.UpdateLocation(locationId, Room.Create(""), null);
        act.Should().Throw<BusinessRuleValidationException>();
    }

    [Fact]
    public void CanRemoveEmptyLocation()
    {
        var house = House.Create("Test House");
        var locationId = house.AddLocation(Room.Create("Living Room"), null);
        house.RemoveLocation(locationId);
        house.Locations.Should().NotContain(l => l.Id == locationId);
    }

    [Fact]
    public void CannotRemoveLocationWithItems()
    {
        var house = House.Create("Test House");
        var locationId = house.AddLocation(Room.Create("Living Room"), null);
        house.GetLocation(locationId).AddItem("Test Item", "https://example.com/test.jpg");

        var act = () => house.RemoveLocation(locationId);
        act.Should().Throw<BusinessRuleValidationException>();
    }

    [Fact]
    public void CannotRemoveNonExistingLocation()
    {
        var house = House.Create("Test House");
        var act = () => house.RemoveLocation(Guid.NewGuid());
        act.Should().Throw<NotFoundException>();
    }

    [Fact]
    public void CanMoveItemBetweenLocations()
    {
        var house = House.Create("Test House");
        var locationId1 = house.AddLocation(Room.Create("Living Room"), null);
        var locationId2 = house.AddLocation(Room.Create("Kitchen"), null);
        var itemId = house.GetLocation(locationId1).AddItem("Test Item", "https://example.com/test.jpg");
        house.MoveItem(itemId, locationId1, locationId2);
        house.GetLocation(locationId2).Items.Should().Contain(i => i.Id == itemId);
    }

    [Fact]
    public void CannotMoveItemToTheSameLocation()
    {
        var house = House.Create("Test House");
        var locationId = house.AddLocation(Room.Create("Living Room"), null);
        var itemId = house.GetLocation(locationId).AddItem("Test Item", "https://example.com/test.jpg");
        var act = () => house.MoveItem(itemId, locationId, locationId);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void CannotMoveNonExistingItem()
    {
        var house = House.Create("Test House");
        var locationId1 = house.AddLocation(Room.Create("Living Room"), null);
        var locationId2 = house.AddLocation(Room.Create("Kitchen"), null);
        house.GetLocation(locationId1).AddItem("Test Item", "https://example.com/test.jpg");
        var act = () => house.MoveItem(Guid.NewGuid(), locationId1, locationId2);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void CannotMoveItemFromNonExistingLocation()
    {
        var house = House.Create("Test House");
        var locationId1 = house.AddLocation(Room.Create("Living Room"), null);
        var locationId2 = house.AddLocation(Room.Create("Kitchen"), null);
        var itemId = house.GetLocation(locationId1).AddItem("Test Item", "https://example.com/test.jpg");

        var act = () => house.MoveItem(itemId, Guid.NewGuid(), locationId2);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void CannotMoveItemToNonExistingLocation()
    {
        var house = House.Create("Test House");
        var locationId1 = house.AddLocation(Room.Create("Living Room"), null);
        house.AddLocation(Room.Create("Kitchen"), null);
        var itemId = house.GetLocation(locationId1).AddItem("Test Item", "https://example.com/test.jpg");
        var act = () => house.MoveItem(itemId, locationId1, Guid.NewGuid());
        act.Should().Throw<NotFoundException>();
    }
}