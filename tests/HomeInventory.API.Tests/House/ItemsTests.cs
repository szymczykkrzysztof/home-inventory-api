using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HomeInventory.API.Tests.Infrastructure;
using HomeInventory.API.Tests.Models;

namespace HomeInventory.API.Tests.House;

public class ItemsTests(HomeInventoryApiFactory factory) : IClassFixture<HomeInventoryApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task AddItem_ShouldReturnId()
    {
        // Arrange
        var houseId = await CreateHouseAsync("Item House");
        var locationId = await CreateLocationAsync(houseId, "Office", "Desk");
        var request = new AddItemRequest("Pen", "pen.jpg");

        // Act
        var response = await _client.PostAsJsonAsync($"/api/houses/{houseId}/locations/{locationId}/items", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var itemId = await response.Content.ReadFromJsonAsync<Guid>();
        itemId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task UpdateItem_ShouldChangeProperties()
    {
        // Arrange
        var houseId = await CreateHouseAsync("Update Item House");
        var locationId = await CreateLocationAsync(houseId, "Garage", null);
        var itemId = await CreateItemAsync(houseId, locationId, "Old Drill", "old.jpg");

        var request = new UpdateItemRequest("New Drill", "new.jpg");

        // Act
        var response = await _client.PutAsJsonAsync(
            $"/api/houses/{houseId}/locations/{locationId}/items/{itemId}",
            request);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);
        // Verify
        var detail = await GetHouseDetailAsync(houseId);
        var item = detail.Locations.Single().Items.Single(i => i.ItemId == itemId);
        item.Name.Should().Be("New Drill");
        item.ImageUrl.Should().Be("new.jpg");
    }

    [Fact]
    public async Task MoveItem_ShouldChangeLocation()
    {
        // Arrange
        var houseId = await CreateHouseAsync("Move House");
        var sourceLocId = await CreateLocationAsync(houseId, "Source", null);
        var targetLocId = await CreateLocationAsync(houseId, "Target", null);
        var itemId = await CreateItemAsync(houseId, sourceLocId, "Moving Box", "box.png");

        var request = new MoveItemRequest(sourceLocId, targetLocId);

        // Act
        var response = await _client.PostAsJsonAsync(
            $"/api/houses/{houseId}/items/{itemId}/move",
            request);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);
        // Verify
        var detail = await GetHouseDetailAsync(houseId);

        // Should not be in Source
        detail.Locations.Single(l => l.LocationId == sourceLocId).Items.Should().BeEmpty();

        // Should be in Target
        var targetLoc = detail.Locations.Single(l => l.LocationId == targetLocId);
        targetLoc.Items.Should().Contain(i => i.ItemId == itemId);
    }

    [Fact]
    public async Task RemoveItem_ShouldDeleteIt()
    {
        // Arrange
        var houseId = await CreateHouseAsync("Delete Item House");
        var locationId = await CreateLocationAsync(houseId, "Room", null);
        var itemId = await CreateItemAsync(houseId, locationId, "Trash", "trash.jpg");

        // Act
        var response = await _client.DeleteAsync(
            $"/api/houses/{houseId}/locations/{locationId}/items/{itemId}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);
        // Verify
        var detail = await GetHouseDetailAsync(houseId);
        detail.Locations.Single().Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetItems_WithFilters_ShouldReturnFilteredResults()
    {
        // Arrange
        var houseId = await CreateHouseAsync("Search House");
        var kitchenId = await CreateLocationAsync(houseId, "Kitchen", "Drawer");
        var atticId = await CreateLocationAsync(houseId, "Attic", null);

        await CreateItemAsync(houseId, kitchenId, "Silver Spoon", "spoon.jpg");
        await CreateItemAsync(houseId, kitchenId, "Silver Fork", "fork.jpg");
        await CreateItemAsync(houseId, atticId, "Old Spoon", "dusty_spoon.jpg");

        // Act 1: Search by Name "Spoon"
        var searchResp = await _client.GetAsync($"/api/houses/{houseId}/items?search=Spoon");
        var searchResults = await searchResp.Content.ReadFromJsonAsync<List<ItemDto>>();
        searchResults.Should().HaveCount(2); // Silver Spoon, Old Spoon

        // Act 2: Filter by Room "Kitchen"
        var roomResp = await _client.GetAsync($"/api/houses/{houseId}/items?room=Kitchen");
        var roomResults = await roomResp.Content.ReadFromJsonAsync<List<ItemDto>>();
        roomResults.Should().HaveCount(2); // Silver Spoon, Silver Fork

        // Act 3: Filter by Container "Drawer"
        var contResp = await _client.GetAsync($"/api/houses/{houseId}/items?container=Drawer");
        var contResults = await contResp.Content.ReadFromJsonAsync<List<ItemDto>>();
        contResults.Should().HaveCount(2);
    }

    private async Task<Guid> CreateHouseAsync(string name)
    {
        var response = await _client.PostAsJsonAsync("/api/houses", new RegisterHouseCommand(name));
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    private async Task<Guid> CreateLocationAsync(Guid houseId, string room, string? container)
    {
        var response = await _client.PostAsJsonAsync(
            $"/api/houses/{houseId}/locations",
            new AddLocationRequest(room, container));
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    private async Task<Guid> CreateItemAsync(Guid houseId, Guid locationId, string name, string img)
    {
        var response = await _client.PostAsJsonAsync(
            $"/api/houses/{houseId}/locations/{locationId}/items",
            new AddItemRequest(name, img));
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    private async Task<HouseDetailDto> GetHouseDetailAsync(Guid houseId)
    {
        return await _client.GetFromJsonAsync<HouseDetailDto>($"/api/houses/{houseId}")
               ?? throw new Exception("House not found");
    }
}