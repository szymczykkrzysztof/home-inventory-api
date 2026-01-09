namespace HomeInventory.Infrastructure.Persistence.ReadModels;

public class ItemReadModel
{
    public Guid ItemId { get; set; }
    public string Name { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;

    public Guid HouseId { get; set; }

    public string RoomName { get; set; } = null!;
    public string? ContainerName { get; set; }
}