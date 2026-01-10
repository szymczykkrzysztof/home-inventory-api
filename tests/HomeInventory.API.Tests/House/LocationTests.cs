using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HomeInventory.API.Tests.Infrastructure;
using HomeInventory.API.Tests.Models;

namespace HomeInventory.API.Tests.House;

public class LocationTests(HomeInventoryApiFactory factory) : IClassFixture<HomeInventoryApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task AddLocation_ShouldReturnId()
    {
        // Arrange
        var houseId = await CreateHouseAsync("Loc House");
        var request = new AddLocationRequest("Kitchen", "Fridge");

        // Act
        var response = await _client.PostAsJsonAsync($"/api/houses/{houseId}/locations", request);

        // Assert
        // Swagger mówi 200, ale zmieniliśmy na 201 w kodzie - oba są Success
        response.EnsureSuccessStatusCode();
        var locationId = await response.Content.ReadFromJsonAsync<Guid>();
        locationId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetLocations_ShouldReturnLookupList()
    {
        // Arrange
        var houseId = await CreateHouseAsync("Lookup House");
        await CreateLocationAsync(houseId, "Bedroom", "Closet");

        // Act
        var response = await _client.GetAsync($"/api/houses/{houseId}/locations");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var locations = await response.Content.ReadFromJsonAsync<List<LocationLookupDto>>();

        locations.Should().ContainSingle(l => l.RoomName == "Bedroom" && l.ContainerName == "Closet");
    }

    [Fact]
    public async Task RenameLocation_ShouldUpdateName()
    {
        // Arrange
        var houseId = await CreateHouseAsync("Rename House");
        var locationId = await CreateLocationAsync(houseId, "Old Room", null);
        var request = new RenameLocationRequest("New Room", "New Container");

        // Act
        var response = await _client.PutAsJsonAsync($"/api/houses/{houseId}/locations/{locationId}", request);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);
        // Verify state
        var detail = await GetHouseDetailAsync(houseId);
        var loc = detail.Locations.Single(l => l.LocationId == locationId);
        loc.RoomName.Should().Be("New Room");
        loc.ContainerName.Should().Be("New Container");
    }

    [Fact]
    public async Task RemoveLocation_ShouldDeleteIt()
    {
        // Arrange
        var houseId = await CreateHouseAsync("Delete Loc House");
        var locationId = await CreateLocationAsync(houseId, "To Delete", null);

        // Act
        var response = await _client.DeleteAsync($"/api/houses/{houseId}/locations/{locationId}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);
        // Verify
        var detail = await GetHouseDetailAsync(houseId);
        detail.Locations.Should().NotContain(l => l.LocationId == locationId);
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

    private async Task<HouseDetailDto> GetHouseDetailAsync(Guid houseId)
    {
        return await _client.GetFromJsonAsync<HouseDetailDto>($"/api/houses/{houseId}")
               ?? throw new Exception("House not found");
    }
}