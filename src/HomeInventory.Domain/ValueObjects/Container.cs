using HomeInventory.Domain.Exceptions;

namespace HomeInventory.Domain.ValueObjects;

public sealed record Container
{
    public string Name { get; }
    private Container(string name) => Name = name;

    public static Container Create(string name)
    {
        return string.IsNullOrWhiteSpace(name)
            ? throw new DomainException("Container name is required")
            : new Container(name.Trim());
    }
};