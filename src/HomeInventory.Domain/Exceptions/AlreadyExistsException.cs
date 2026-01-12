namespace HomeInventory.Domain.Exceptions;

public class AlreadyExistsException(string resourceName, object key):
    DomainException($"Entity {resourceName} with key {key} already exists.")
{
    
}