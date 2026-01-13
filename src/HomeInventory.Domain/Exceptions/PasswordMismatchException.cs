namespace HomeInventory.Domain.Exceptions;

public class PasswordMismatchException(string message = "Password mismatch") : DomainException(message);