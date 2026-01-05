using HomeInventory.Domain.Exceptions;

namespace HomeInventory.Domain.Aggregates.House;

public class Item
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string ImageUrl { get; private set; } = null!;

    private Item()
    {
    }

    internal Item(Guid id, string name, string imageUrl)
    {
        Id = id;
        SetName(name);
        SetImageUrl(imageUrl);
    }

    internal static Item Create(string name, string imageUrl)
    {
        return new Item { Id = Guid.NewGuid(), Name = name, ImageUrl = imageUrl };
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Item name is required.");
        }

        Name = name.Trim();
    }

    private void SetImageUrl(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            throw new DomainException("ImageUrl is required.");
        }

        ImageUrl = imageUrl.Trim();
    }

    internal void UpdateName(string name)
    {
        SetName(name);
    }

    internal void UpdateImageUrl(string imageUrl)
    {
        SetImageUrl(imageUrl);
    }
}