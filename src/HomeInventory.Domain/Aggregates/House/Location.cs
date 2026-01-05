using HomeInventory.Domain.Exceptions;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Aggregates.House;

public class Location
{
    public Guid Id { get; }
    public Room Room { get; private set; }
    public Container? Container { get; private set; }
    private readonly List<Item> _items = [];
    public IReadOnlyCollection<Item> Items => _items.AsReadOnly();

    private Location()
    {
        
    }

    private Location(Guid id, Room room, Container? container)
    {
        Id = id;
        Room = room;
        Container = container;
    }

    internal static Location Create(Room room, Container? container)
    {
        return room is null
            ? throw new DomainException("Room is required")
            : new Location(Guid.NewGuid(), room, container);
    }

    internal void Rename(Room newRoom, Container? newContainer)
    {
        Room = newRoom ?? throw new DomainException();
        Container = newContainer;
    }

    internal Guid AddItem(string name, string imageUrl)
    {
        var item = Item.Create(name, imageUrl);
        _items.Add(item);
        return item.Id;
    }

    internal void RemoveItem(Guid itemId)
    {
        var item = GetItem(itemId);
        _items.Remove(item);
    }

    internal void UpdateItem(Guid itemId, string name, string imageUrl)
    {
        var item = GetItem(itemId);
        item.UpdateName(name);
        item.UpdateImageUrl(imageUrl);
    }

    internal Item ExtractItem(Guid itemId)
    {
        var item = GetItem(itemId);
        _items.Remove(item);
        return item;
    }

    internal void InsertItem(Item item)
    {
        if (_items.Any(x => x.Id == item.Id))
        {
            throw new DomainException($"Item with id {item.Id} already exists.");
        }

        _items.Add(item);
    }

    private Item GetItem(Guid itemId)
    {
        return _items.SingleOrDefault(x => x.Id == itemId) ??
               throw new DomainException($"Item with id {itemId} not found.");
    }
}