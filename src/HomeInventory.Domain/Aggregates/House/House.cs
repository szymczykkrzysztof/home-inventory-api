using HomeInventory.Domain.Exceptions;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Aggregates.House;

public class House
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    private readonly List<Location> _locations = [];
    public IReadOnlyCollection<Location> Locations => _locations.AsReadOnly();

    private House()
    {
    }

    private House(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public static House Create(string? name)
    {
        return string.IsNullOrWhiteSpace(name)
            ? throw new BusinessRuleValidationException("House name is required.")
            : new House(Guid.NewGuid(), name.Trim());
    }

    public Guid AddLocation(Room? room, Container? container)
    {
        if (room is null)
            throw new BusinessRuleValidationException("Room is required");
        EnsureUniqueLocation(room, container, exceptLocationId: null);
        var location = Location.Create(room, container);

        _locations.Add(location);
        return location.Id;
    }

    public Location GetLocation(Guid locationId)
    {
        return _locations.SingleOrDefault(x => x.Id == locationId) ??
               throw new NotFoundException("Location",locationId);
    }

    public void UpdateLocation(Guid locationId, Room newRoom, Container? newContainer)
    {
        var location = GetLocation(locationId);
        EnsureUniqueLocation(newRoom, newContainer, exceptLocationId: locationId);
        location.Rename(newRoom, newContainer);
    }

    public void RemoveLocation(Guid locationId)
    {
        var location = GetLocation(locationId);
        if (location.Items.Any()) throw new BusinessRuleValidationException("Cannot remove location with items.");
        _locations.Remove(location);
    }

    public void MoveItem(Guid itemId, Guid fromLocationId, Guid toLocationId)
    {
        var from = GetLocation(fromLocationId);
        var to = GetLocation(toLocationId);
        if (from == to)
        {
            throw new BusinessRuleValidationException("Cannot move item to the same location.");
        }

        var item = from.GetItem(itemId);
        to.EnsureCanAdd(item);

        from.ExtractItem(itemId);
        to.InsertItem(item);
    }

    private void EnsureUniqueLocation(Room room, Container? container, Guid? exceptLocationId)
    {
        var duplicate = _locations.Any(l =>
            (exceptLocationId == null || l.Id != exceptLocationId.Value) &&
            l.Room.Name.Equals(room.Name, StringComparison.OrdinalIgnoreCase) &&
            ContainerEquals(l.Container, container));
        if (duplicate) throw new AlreadyExistsException("Location",$"{room.Name} > {container?.Name}");
    }

    private static bool ContainerEquals(Container? a, Container? b)
    {
        if (a is null && b is null) return true;
        if (a is null || b is null) return false;
        return a.Name.Equals(b.Name, StringComparison.OrdinalIgnoreCase);
    }
}