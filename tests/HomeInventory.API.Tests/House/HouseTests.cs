using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HomeInventory.API.Tests.Infrastructure;
using HomeInventory.API.Tests.Models;

namespace HomeInventory.API.Tests.House;

public class HouseTests(HomeInventoryApiFactory factory) : BaseClass(factory)
{
    [Fact]
    public async Task RegisterHouse_ShouldReturnCreatedAndId()
    {
        var request = new RegisterHouseCommand("My Test House");

        var response = await Client.PostAsJsonAsync("/api/houses", request);

        response.EnsureSuccessStatusCode(); // 200 or 201
        var houseId = await response.Content.ReadFromJsonAsync<Guid>();
        houseId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetHouses_ShouldReturnList()
    {
        await CreateHouseAsync("House A");
        await CreateHouseAsync("House B");

        var response = await Client.GetAsync("/api/houses");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var houses = await response.Content.ReadFromJsonAsync<List<HouseLookupDto>>();

        houses.Should().NotBeNull();
        houses.Should().Contain(h => h.Name == "House A");
        houses.Should().Contain(h => h.Name == "House B");
    }

    [Fact]
    public async Task GetHouseDetail_ShouldReturnDetails()
    {
        var houseId = await CreateHouseAsync("Detail House");

        var response = await Client.GetAsync($"/api/houses/{houseId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var detail = await response.Content.ReadFromJsonAsync<HouseDetailDto>();

        detail.Should().NotBeNull();
        detail.HouseId.Should().Be(houseId);
        detail.Name.Should().Be("Detail House");
    }
}