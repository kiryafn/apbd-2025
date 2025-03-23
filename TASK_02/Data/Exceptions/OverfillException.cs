namespace ContainerShipment.Domain.Exceptions;

public class OverfillException(string message) : Exception(message)
{
}