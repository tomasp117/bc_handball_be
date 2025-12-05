namespace bc_handball_be.Core.Exceptions;

/// <summary>
/// Exception thrown when user is not authenticated or token is invalid.
/// Results in HTTP 401 response.
/// </summary>
public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message)
    {
    }

    public UnauthorizedException()
        : base("You are not authorized to access this resource.")
    {
    }
}
