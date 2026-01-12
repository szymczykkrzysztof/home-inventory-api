using HomeInventory.Domain.Exceptions;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Aggregates.House;

public class Location
{
    public Guid Id { get; private set; }
    public Room Room { get; private set; } = null!;
    public Container? Container { get; private set; }
    private readonly List<Item> _items = [];
    public IReadOnlyCollection<Item> Items => _items.AsReadOnly();

    private Location()
    {
    }

    internal Location(Guid id, Room room, Container? container)
    {
        Id = id;
        Room = room;
        Container = container;
    }

    internal static Location Create(Room room, Container? container)
        => new Location(Guid.NewGuid(), room, container);

    public void Rename(Room newRoom, Container? newContainer)
    {
        Room = newRoom ?? throw new BusinessRuleValidationException("New room details cannot be null.");
        Container = newContainer;
    }

    public Guid AddItem(string? name, string? imageUrl)
    {
        var item = Item.Create(name, imageUrl);
        _items.Add(item);
        return item.Id;
    }

    public void RemoveItem(Guid itemId)
    {
        var item = GetItem(itemId);
        _items.Remove(item);
    }

    public void UpdateItem(Guid itemId, string? name, string? imageUrl)
    {
        var item = GetItem(itemId);
        item.UpdateName(name);
        item.UpdateImageUrl(imageUrl);
    }

    public Item ExtractItem(Guid itemId)
    {
        var item = GetItem(itemId);
        _items.Remove(item);
        return item;
    }

    public void InsertItem(Item item)
    {
        if (_items.Any(x => x.Id == item.Id))
        {
            throw new AlreadyExistsException("Item", item.Id.ToString());
        }

        _items.Add(item);
    }

    public Item GetItem(Guid itemId)
    {
        return _items.SingleOrDefault(x => x.Id == itemId) ??
               throw new NotFoundException($"Item", itemId);
    }

    public void EnsureCanAdd(Item item)
    {
        if (_items.Any(x => x.Id == item.Id))
        {
            throw new AlreadyExistsException("Item", item.Id);
        }
    }
}