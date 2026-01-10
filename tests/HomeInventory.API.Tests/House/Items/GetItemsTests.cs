using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HomeInventory.API.Tests.Infrastructure;
using HomeInventory.Application.Houses.Queries.GetItems;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.API.Tests.House.Items;

public class GetItemsTests : IClassFixture<HomeInventoryApiFactory>
{
    private readonly HttpClient _client;
    private readonly Guid _houseId;

    public GetItemsTests(HomeInventoryApiFactory factory)
    {
        _client = factory.CreateClient();

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider
            .GetRequiredService<HomeInventoryDbContext>();

        // --- Seed domain data ---
        var house = Domain.Aggregates.House.House.Create("Test House");

        var kitchenId = house.AddLocation(
            Room.Create("Kitchen"),
            Container.Create("Drawer"));

        var livingRoomId = house.AddLocation(
            Room.Create("Living Room"),
            null);

        house.GetLocation(kitchenId)
            .AddItem("Spoon", "img-spoon");

        house.GetLocation(kitchenId)
            .AddItem("Fork", "img-fork");

        house.GetLocation(livingRoomId)
            .AddItem("Laptop", "img-laptop");

        dbContext.Houses.Add(house);
        dbContext.SaveChanges();

        _houseId = house.Id;
    }

    [Fact]
    public async Task GetItemsEndpoint_ReturnsFilteredItems_ByRoom()
    {
        // Act
        var response = await _client.GetAsync(
            $"/api/houses/{_houseId}/items?room=Kitchen");

        // Assert – HTTP
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var items = await response.Content
            .ReadFromJsonAsync<List<ItemDto>>();

        items.Should().NotBeNull();
        items.Should().HaveCount(2);

        items!.Select(i => i.Name)
            .Should()
            .BeEquivalentTo("Spoon", "Fork");

        items.All(i => i.RoomName == "Kitchen")
            .Should()
            .BeTrue();
    }

    [Fact]
    public async Task GetItemsEndpoint_ReturnsFilteredItems_ByContainer()
    {
        var response = await _client.GetAsync(
            $"/api/houses/{_houseId}/items?container=Drawer");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var items = await response.Content
            .ReadFromJsonAsync<List<ItemDto>>();

        items.Should().NotBeNull();
        items.Should().HaveCount(2);

        items!.All(i => i.ContainerName == "Drawer")
            .Should()
            .BeTrue();
    }

    [Fact]
    public async Task GetItemsEndpoint_ReturnsFilteredItems_BySearch()
    {
        var response = await _client.GetAsync(
            $"/api/houses/{_houseId}/items?search=Lap");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var items = await response.Content
            .ReadFromJsonAsync<List<ItemDto>>();

        items.Should().ContainSingle();
        items!.Single().Name.Should().Be("Laptop");
    }

    [Fact]
    public async Task GetItemsEndpoint_ReturnsAllItems_WhenNoFiltersProvided()
    {
        var response = await _client.GetAsync(
            $"/api/houses/{_houseId}/items");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var items = await response.Content
            .ReadFromJsonAsync<List<ItemDto>>();

        items.Should().NotBeNull();
        items.Should().HaveCount(3);
    }
}