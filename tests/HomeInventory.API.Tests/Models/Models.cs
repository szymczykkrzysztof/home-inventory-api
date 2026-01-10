namespace HomeInventory.API.Tests.Models;

public record RegisterHouseCommand(string Name);

public record AddLocationRequest(string RoomName, string? ContainerName);

public record AddItemRequest(string Name, string ImageUrl);

public record RenameLocationRequest(string RoomName, string? ContainerName);

public record UpdateItemRequest(string Name, string ImageUrl);

public record MoveItemRequest(Guid FromLocationId, Guid ToLocationId);

public record HouseLookupDto(Guid Id, string Name);

public record LocationLookupDto(Guid Id, string RoomName, string? ContainerName);

public record HouseDetailDto(Guid HouseId, string Name, List<LocationDto> Locations);

public record LocationDto(Guid LocationId, string RoomName, string? ContainerName, List<ItemDto> Items);

public record ItemDto(Guid ItemId, string Name, string ImageUrl);