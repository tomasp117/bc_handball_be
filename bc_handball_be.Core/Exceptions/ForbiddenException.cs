namespace bc_handball_be.Core.Exceptions;

/// <summary>
/// Exception thrown when user lacks permission to access a resource.
/// Results in HTTP 403 response.
/// </summary>
public class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message)
    {
    }

    public ForbiddenException()
        : base("You do not have permission to access this resource.")
    {
    }
}
