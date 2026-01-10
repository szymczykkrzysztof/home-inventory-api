using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HomeInventory.API.Tests.Infrastructure;
using HomeInventory.API.Tests.Models;

namespace HomeInventory.API.Tests.House;

public class HouseTests(HomeInventoryApiFactory factory) : IClassFixture<HomeInventoryApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task RegisterHouse_ShouldReturnCreatedAndId()
    {
        // Arrange
        var request = new RegisterHouseCommand("My Test House");

        // Act
        var response = await _client.PostAsJsonAsync("/api/houses", request);

        // Assert
        response.EnsureSuccessStatusCode(); // 200 or 201
        var houseId = await response.Content.ReadFromJsonAsync<Guid>();
        houseId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetHouses_ShouldReturnList()
    {
        // Arrange
        await CreateHouseAsync("House A");
        await CreateHouseAsync("House B");

        // Act
        var response = await _client.GetAsync("/api/houses");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var houses = await response.Content.ReadFromJsonAsync<List<HouseLookupDto>>();

        houses.Should().NotBeNull();
        houses!.Should().Contain(h => h.Name == "House A");
        houses!.Should().Contain(h => h.Name == "House B");
    }

    [Fact]
    public async Task GetHouseDetail_ShouldReturnDetails()
    {
        // Arrange
        var houseId = await CreateHouseAsync("Detail House");

        // Act
        var response = await _client.GetAsync($"/api/houses/{houseId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var detail = await response.Content.ReadFromJsonAsync<HouseDetailDto>();

        detail.Should().NotBeNull();
        detail!.HouseId.Should().Be(houseId);
        detail.Name.Should().Be("Detail House");
    }

    private async Task<Guid> CreateHouseAsync(string name)
    {
        var response = await _client.PostAsJsonAsync("/api/houses", new RegisterHouseCommand(name));
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Guid>();
    }
}