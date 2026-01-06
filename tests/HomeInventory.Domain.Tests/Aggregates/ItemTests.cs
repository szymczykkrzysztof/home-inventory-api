using FluentAssertions;
using HomeInventory.Domain.Aggregates.House;
using HomeInventory.Domain.Exceptions;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Tests.Aggregates;

public class ItemTests
{
    private readonly House _house;
    private readonly Guid _locationId;

    public ItemTests()
    {
        _house = House.Create("Test House");
        _locationId = _house.AddLocation(Room.Create("Living Room"), null);
    }

    [Fact]
    public void CanCreateItemWithValidNameAndImageUrl()
    {
        var itemId = _house.GetLocation(_locationId).AddItem("Test Item", "https://example.com/test.jpg");
        var item = _house.GetLocation(_locationId).GetItem(itemId);
        item.Name.Should().Be("Test Item");
        item.ImageUrl.Should().Be("https://example.com/test.jpg");
    }

    [Fact]
    public void CannotCreateItemWithEmptyNameAndEmptyImageUrl()
    {
        var act = () => _house.GetLocation(_locationId).AddItem("", "");
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void CannotCreateItemWithNullNameAndNullImageUrl()
    {
        var act = () => _house.GetLocation(_locationId).AddItem(null, null);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void ItemHasUniqueIdentifier()
    {
        var itemId1 = _house.GetLocation(_locationId).AddItem("Test Item", "https://example.com/test.jpg");
        var itemId2 = _house.GetLocation(_locationId).AddItem("Test Item 2", "https://example.com/test2.jpg");
        itemId1.Should().NotBe(itemId2);
        var item1 = _house.GetLocation(_locationId).GetItem(itemId1);
        var item2 = _house.GetLocation(_locationId).GetItem(itemId2);
        item1.Id.Should().NotBe(Guid.Empty);
        item2.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void CanCreateItemsWithTheSameName()
    {
        var itemId1 = _house.GetLocation(_locationId).AddItem("Test Item", "https://example.com/test.jpg");
        var itemId2 = _house.GetLocation(_locationId).AddItem("Test Item", "https://example.com/test2.jpg");
        itemId1.Should().NotBe(itemId2);
    }

    [Fact]
    public void CanUpdateItem()
    {
        var itemId = _house.GetLocation(_locationId).AddItem("Test Item", "https://example.com/test.jpg");
        _house.GetLocation(_locationId).UpdateItem(itemId, "New Test Item", "https://example.com/new-test.jpg");
        var item = _house.GetLocation(_locationId).GetItem(itemId);
        item.Name.Should().Be("New Test Item");
        item.ImageUrl.Should().Be("https://example.com/new-test.jpg");
    }

    [Fact]
    public void CannotUpdateNonExistingItem()
    {
        var act = () =>
            _house.GetLocation(_locationId).UpdateItem(Guid.NewGuid(), "Test Item", "https://example.com/test.jpg");
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void CannotUpdateToEmptyNameAndEmptyImageUrl()
    {
        var itemId = _house.GetLocation(_locationId).AddItem("Test Item", "https://example.com/test.jpg");
        var act = () => _house.GetLocation(_locationId).UpdateItem(itemId, "", "");
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void CannotUpdateToNullNameAndNullImageUrl()
    {
        var itemId = _house.GetLocation(_locationId).AddItem("Test Item", "https://example.com/test.jpg");
        var act = () => _house.GetLocation(_locationId).UpdateItem(itemId, null, null);
        act.Should().Throw<DomainException>();
    }
}