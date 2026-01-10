using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HomeInventory.API.Tests.Infrastructure;
using HomeInventory.API.Tests.Models;

namespace HomeInventory.API.Tests.House;

public class ItemsTests(HomeInventoryApiFactory factory) : BaseClass(factory)
{
    [Fact]
    public async Task AddItem_ShouldReturnId()
    {
        var houseId = await CreateHouseAsync("Item House");
        var locationId = await CreateLocationAsync(houseId, "Office", "Desk");
        var request = new AddItemRequest("Pen", "pen.jpg");
        
        var response = await Client.PostAsJsonAsync($"/api/houses/{houseId}/locations/{locationId}/items", request);
        
        response.EnsureSuccessStatusCode();
        var itemId = await response.Content.ReadFromJsonAsync<Guid>();
        itemId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task UpdateItem_ShouldChangeProperties()
    {
        var houseId = await CreateHouseAsync("Update Item House");
        var locationId = await CreateLocationAsync(houseId, "Garage", null);
        var itemId = await CreateItemAsync(houseId, locationId, "Old Drill", "old.jpg");

        var request = new UpdateItemRequest("New Drill", "new.jpg");
        
        var response = await Client.PutAsJsonAsync(
            $"/api/houses/{houseId}/locations/{locationId}/items/{itemId}",
            request);
        
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);
       
        var detail = await GetHouseDetailAsync(houseId);
        var item = detail.Locations.Single().Items.Single(i => i.ItemId == itemId);
        item.Name.Should().Be("New Drill");
        item.ImageUrl.Should().Be("new.jpg");
    }

    [Fact]
    public async Task MoveItem_ShouldChangeLocation()
    {
        var houseId = await CreateHouseAsync("Move House");
        var sourceLocId = await CreateLocationAsync(houseId, "Source", null);
        var targetLocId = await CreateLocationAsync(houseId, "Target", null);
        var itemId = await CreateItemAsync(houseId, sourceLocId, "Moving Box", "box.png");

        var request = new MoveItemRequest(sourceLocId, targetLocId);
        
        var response = await Client.PostAsJsonAsync(
            $"/api/houses/{houseId}/items/{itemId}/move",
            request);
        
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);
        
        var detail = await GetHouseDetailAsync(houseId);
        
        detail.Locations.Single(l => l.LocationId == sourceLocId).Items.Should().BeEmpty();
        
        var targetLoc = detail.Locations.Single(l => l.LocationId == targetLocId);
        targetLoc.Items.Should().Contain(i => i.ItemId == itemId);
    }

    [Fact]
    public async Task RemoveItem_ShouldDeleteIt()
    {
        var houseId = await CreateHouseAsync("Delete Item House");
        var locationId = await CreateLocationAsync(houseId, "Room", null);
        var itemId = await CreateItemAsync(houseId, locationId, "Trash", "trash.jpg");
        
        var response = await Client.DeleteAsync(
            $"/api/houses/{houseId}/locations/{locationId}/items/{itemId}");
        
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);
       
        var detail = await GetHouseDetailAsync(houseId);
        detail.Locations.Single().Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetItems_WithFilters_ShouldReturnFilteredResults()
    {
        var houseId = await CreateHouseAsync("Search House");
        var kitchenId = await CreateLocationAsync(houseId, "Kitchen", "Drawer");
        var atticId = await CreateLocationAsync(houseId, "Attic", null);

        await CreateItemAsync(houseId, kitchenId, "Silver Spoon", "spoon.jpg");
        await CreateItemAsync(houseId, kitchenId, "Silver Fork", "fork.jpg");
        await CreateItemAsync(houseId, atticId, "Old Spoon", "dusty_spoon.jpg");
        
        var searchResp = await Client.GetAsync($"/api/houses/{houseId}/items?search=Spoon");
        var searchResults = await searchResp.Content.ReadFromJsonAsync<List<ItemDto>>();
        searchResults.Should().HaveCount(2); 
        
        var roomResp = await Client.GetAsync($"/api/houses/{houseId}/items?room=Kitchen");
        var roomResults = await roomResp.Content.ReadFromJsonAsync<List<ItemDto>>();
        roomResults.Should().HaveCount(2); 
        
        var contResp = await Client.GetAsync($"/api/houses/{houseId}/items?container=Drawer");
        var contResults = await contResp.Content.ReadFromJsonAsync<List<ItemDto>>();
        contResults.Should().HaveCount(2);
    }
}