namespace HomeInventory.Domain.Exceptions;

public class UserNotAuthorizedException(string message = "User not authorized") : DomainException(message);