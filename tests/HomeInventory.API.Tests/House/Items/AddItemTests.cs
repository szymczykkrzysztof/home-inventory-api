using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HomeInventory.API.Dtos.Houses;
using HomeInventory.API.Tests.Infrastructure;
using HomeInventory.Application.Houses.Queries.GetDetail;

namespace HomeInventory.API.Tests.House.Items;

public class AddItemTests(HomeInventoryApiFactory factory) : IClassFixture<HomeInventoryApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task AddItemEndpoint_AddsItemToLocation()
    {
        var registerHouseResponse = await _client.PostAsJsonAsync(
            "/api/houses",
            new {name = "Test House"});

        registerHouseResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var houseId = await registerHouseResponse
            .Content
            .ReadFromJsonAsync<Guid>();

        houseId.Should().NotBeEmpty();


        var addLocationResponse = await _client.PostAsJsonAsync(
            $"/api/houses/{houseId}/locations",
            new AddLocationRequest
                ("Kitchen", "Drawer"));

        addLocationResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var locationId = await addLocationResponse
            .Content
            .ReadFromJsonAsync<Guid>();

        locationId.Should().NotBeEmpty();

        // =====================
        // ACT – add item
        // =====================

        var addItemResponse = await _client.PostAsJsonAsync(
            $"/api/houses/{houseId}/locations/{locationId}/items",
            new AddItemRequest
            {
                Name = "Spoon",
                ImageUrl = "img-spoon"
            });

        // =====================
        // ASSERT – HTTP
        // =====================

        addItemResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var itemId = await addItemResponse
            .Content
            .ReadFromJsonAsync<Guid>();

        itemId.Should().NotBeEmpty();

        // =====================
        // ASSERT – state via API
        // =====================

        var houseDetailResponse = await _client.GetAsync(
            $"/api/houses/{houseId}");

        houseDetailResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var house = await houseDetailResponse
            .Content
            .ReadFromJsonAsync<HouseDetailDto>();

        house.Should().NotBeNull();
        house!.Locations.Should().ContainSingle();

        var location = house.Locations.Single();
        location.RoomName.Should().Be("Kitchen");

        location.Items.Should().ContainSingle();

        var item = location.Items.Single();
        item.ItemId.Should().Be(itemId);
        item.Name.Should().Be("Spoon");
        item.ImageUrl.Should().Be("img-spoon");
    }
}