namespace HomeInventory.Domain.Exceptions;

public class UnauthorizedActionException(string message):DomainException(message);