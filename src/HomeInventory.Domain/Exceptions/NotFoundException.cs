namespace HomeInventory.Domain.Exceptions;

public class NotFoundException(string resourceName, object key) :
    DomainException($"Entity {resourceName} with key {key} was not found");