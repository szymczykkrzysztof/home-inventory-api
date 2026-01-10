using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HomeInventory.API.Tests.Infrastructure;
using HomeInventory.API.Tests.Models;

namespace HomeInventory.API.Tests.House;

public class LocationTests(HomeInventoryApiFactory factory) : BaseClass(factory)
{
    [Fact]
    public async Task AddLocation_ShouldReturnId()
    {
        var houseId = await CreateHouseAsync("Loc House");
        var request = new AddLocationRequest("Kitchen", "Fridge");

        var response = await Client.PostAsJsonAsync($"/api/houses/{houseId}/locations", request);

        response.EnsureSuccessStatusCode();
        var locationId = await response.Content.ReadFromJsonAsync<Guid>();
        locationId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetLocations_ShouldReturnLookupList()
    {
        var houseId = await CreateHouseAsync("Lookup House");
        await CreateLocationAsync(houseId, "Bedroom", "Closet");

        var response = await Client.GetAsync($"/api/houses/{houseId}/locations");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var locations = await response.Content.ReadFromJsonAsync<List<LocationLookupDto>>();

        locations.Should().ContainSingle(l => l.RoomName == "Bedroom" && l.ContainerName == "Closet");
    }

    [Fact]
    public async Task RenameLocation_ShouldUpdateName()
    {
        var houseId = await CreateHouseAsync("Rename House");
        var locationId = await CreateLocationAsync(houseId, "Old Room", null);
        var request = new RenameLocationRequest("New Room", "New Container");

        var response = await Client.PutAsJsonAsync($"/api/houses/{houseId}/locations/{locationId}", request);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);
        var detail = await GetHouseDetailAsync(houseId);
        var loc = detail.Locations.Single(l => l.LocationId == locationId);
        loc.RoomName.Should().Be("New Room");
        loc.ContainerName.Should().Be("New Container");
    }

    [Fact]
    public async Task RemoveLocation_ShouldDeleteIt()
    {
        var houseId = await CreateHouseAsync("Delete Loc House");
        var locationId = await CreateLocationAsync(houseId, "To Delete", null);

        var response = await Client.DeleteAsync($"/api/houses/{houseId}/locations/{locationId}");

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);
        var detail = await GetHouseDetailAsync(houseId);
        detail.Locations.Should().NotContain(l => l.LocationId == locationId);
    }
}