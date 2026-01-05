namespace HomeInventory.Domain.Exceptions;

public class DomainException(string message = "Room is required.") : Exception(message);
