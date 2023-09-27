namespace Example.IdentityServer.Domain.Models;

/// <summary>
/// Represents an error that occurred during request handling.
/// </summary>
public sealed class DomainError
{
    /// <summary>
    /// Gets a human readable message for this error, can be null.
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Gets the error code for this error. The value 0 usually indicates no error.
    /// </summary>
    public int Code { get; init; }

    /// <summary>
    /// The exception, if any.
    /// </summary>
    public Exception? Exception { get; init; }
}