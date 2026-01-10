using System.Net.Http.Json;
using HomeInventory.API.Tests.Infrastructure;
using HomeInventory.API.Tests.Models;

namespace HomeInventory.API.Tests.House;

public abstract class BaseClass(HomeInventoryApiFactory factory) : IClassFixture<HomeInventoryApiFactory>
{
    protected readonly HttpClient Client = factory.CreateClient();

    protected async Task<Guid> CreateHouseAsync(string name)
    {
        var response = await Client.PostAsJsonAsync("/api/houses", new RegisterHouseCommand(name));
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    protected async Task<Guid> CreateLocationAsync(Guid houseId, string room, string? container)
    {
        var response = await Client.PostAsJsonAsync(
            $"/api/houses/{houseId}/locations",
            new AddLocationRequest(room, container));
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    protected async Task<Guid> CreateItemAsync(Guid houseId, Guid locationId, string name, string img)
    {
        var response = await Client.PostAsJsonAsync(
            $"/api/houses/{houseId}/locations/{locationId}/items",
            new AddItemRequest(name, img));
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    protected async Task<HouseDetailDto> GetHouseDetailAsync(Guid houseId)
    {
        return await Client.GetFromJsonAsync<HouseDetailDto>($"/api/houses/{houseId}")
               ?? throw new Exception("House not found");
    }
}