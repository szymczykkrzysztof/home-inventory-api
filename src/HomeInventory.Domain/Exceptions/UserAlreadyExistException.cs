namespace HomeInventory.Domain.Exceptions;

public class UserAlreadyExistException(string message = "User already exist") : DomainException(message);