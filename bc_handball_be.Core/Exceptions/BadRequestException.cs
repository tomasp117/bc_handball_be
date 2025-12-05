namespace bc_handball_be.Core.Exceptions;

/// <summary>
/// Exception thrown when a request contains invalid data.
/// Results in HTTP 400 response.
/// </summary>
public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message)
    {
    }
}
