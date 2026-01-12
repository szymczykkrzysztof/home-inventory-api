using FluentAssertions;
using HomeInventory.Domain.Aggregates.House;
using HomeInventory.Domain.Exceptions;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Tests.Aggregates;

public class LocationTests
{
    private readonly House _house;
    private readonly Guid _locationId;

    public LocationTests()
    {
        _house = House.Create("Test House");
        _locationId = _house.AddLocation(Room.Create("Living Room"), null);
    }

    [Fact]
    public void CanAddItemsToLocation()
    {
        var itemId = _house.GetLocation(_locationId).AddItem("Test Item", "https://example.com/test.jpg");
        _house.GetLocation(_locationId).Items.Should().Contain(i => i.Id == itemId);
    }

    [Fact]
    public void CannotAddItemWithEmptyNameAndEmptyImageUrl()
    {
        var act = () => _house.GetLocation(_locationId).AddItem("", "");
        act.Should().Throw<BusinessRuleValidationException>();
    }

    [Fact]
    public void CannotAddItemWithNullNameAndNullImageUrl()
    {
        var act = () => _house.GetLocation(_locationId).AddItem(null, null);
        act.Should().Throw<BusinessRuleValidationException>();
    }

    [Fact]
    public void CanUpdateItem()
    {
        var itemId = _house.GetLocation(_locationId).AddItem("Test Item", "https://example.com/test.jpg");
        _house.GetLocation(_locationId).UpdateItem(itemId, "New Test Item", "https://example.com/new-test.jpg");
        _house.GetLocation(_locationId).Items.Should().Contain(i => i.Id == itemId && i.Name == "New Test Item");
    }

    [Fact]
    public void CannotUpdateItemToEmptyNameAndEmptyImageUrl()
    {
        var itemId = _house.GetLocation(_locationId).AddItem("Test Item", "https://example.com/test.jpg");
        var act = () => _house.GetLocation(_locationId).UpdateItem(itemId, "", "");
        act.Should().Throw<BusinessRuleValidationException>();
    }

    [Fact]
    public void CannotUpdateNonExistingItem()
    {
        var act = () =>
            _house.GetLocation(_locationId).UpdateItem(Guid.NewGuid(), "Test Item", "https://example.com/test.jpg");
        act.Should().Throw<NotFoundException>();
    }

    [Fact]
    public void CanRemoveItem()
    {
        var itemId = _house.GetLocation(_locationId).AddItem("Test Item", "https://example.com/test.jpg");
        _house.GetLocation(_locationId).RemoveItem(itemId);
        _house.GetLocation(_locationId).Items.Should().NotContain(i => i.Id == itemId);
    }

    [Fact]
    public void CannotRemoveNonExistingItem()
    {
        var act = () => _house.GetLocation(_locationId).RemoveItem(Guid.NewGuid());
        act.Should().Throw<NotFoundException>();
    }

    [Fact]
    public void CanExtractItemFromLocation()
    {
        var itemId = _house.GetLocation(_locationId).AddItem("Test Item", "https://example.com/test.jpg");
        var extractedItem = _house.GetLocation(_locationId).ExtractItem(itemId);
        _house.GetLocation(_locationId).Items.Should().NotContain(i => i.Id == itemId);
        itemId.Should().Be(extractedItem.Id);
    }

    [Fact]
    public void CannotExtractNonExistingItem()
    {
        var act = () => _house.GetLocation(_locationId).ExtractItem(Guid.NewGuid());
        act.Should().Throw<NotFoundException>();
    }

    [Fact]
    public void CannotExtractItemFromNonExistingLocation()
    {
        var act = () => _house.GetLocation(Guid.NewGuid()).ExtractItem(Guid.NewGuid());
        act.Should().Throw<NotFoundException>();
    }

    [Fact]
    public void CanInsertExtractedItemToAnotherLocation()
    {
        var kitchenLocationId = _house.AddLocation(Room.Create("Kitchen"), null);
        var itemId = _house.GetLocation(_locationId).AddItem("Test Item", "https://example.com/test.jpg");
        var extractedItem = _house.GetLocation(_locationId).ExtractItem(itemId);
        _house.GetLocation(kitchenLocationId).InsertItem(extractedItem);
        _house.GetLocation(kitchenLocationId).Items.Should().Contain(i => i.Id == itemId);
        _house.GetLocation(_locationId).Items.Should().NotContain(i => i.Id == itemId);
    }
}