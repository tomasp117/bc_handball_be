namespace bc_handball_be.Core.Exceptions;

/// <summary>
/// Exception thrown when a requested resource is not found.
/// Results in HTTP 404 response.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string resourceName, object key)
        : base($"{resourceName} with key '{key}' was not found.")
    {
    }
}
