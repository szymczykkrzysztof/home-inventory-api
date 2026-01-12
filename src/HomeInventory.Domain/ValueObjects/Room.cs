using HomeInventory.Domain.Exceptions;

namespace HomeInventory.Domain.ValueObjects;

public sealed record Room
{
    public string Name { get; }
    private Room(string name) => Name = name;

    public static Room Create(string name)
    {
        return string.IsNullOrWhiteSpace(name)
            ? throw new BusinessRuleValidationException("Room name is required.")
            : new Room(name.Trim());
    }
}