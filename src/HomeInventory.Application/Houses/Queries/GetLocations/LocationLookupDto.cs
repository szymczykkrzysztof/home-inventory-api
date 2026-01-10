namespace HomeInventory.Application.Houses.Queries.GetLocations;

public record LocationLookupDto(Guid Id, string RoomName, string? ContainerName);