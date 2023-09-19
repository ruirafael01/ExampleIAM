namespace Visma.IdentityServer.Domain.Models;

/// <summary>
/// Domain response to a request.
/// </summary>
public sealed class DomainResponse
{
    /// <summary>
    /// Indicates whether the request was successfully handled.
    /// </summary>
    public bool Successful { get; set; }

    /// <summary>
    /// The error if the request was not handled successfully. Null if it was successful.
    /// </summary>
    public DomainError? Error { get; set; }

    /// <summary>
    /// Indicates a successful response.
    /// </summary>
    public static DomainResponse Success => new DomainResponse { Successful = true };

    /// <summary>
    /// Indicates a failed response.
    /// </summary>
    public static DomainResponse Failed(int code,
                                        Exception exception)
    {
        return new DomainResponse
        {
            Successful = false,

            Error = new DomainError
            {
                Code = code,
                Exception = exception,
                Message = exception?.Message ?? "Unknown error"
            }
        };
    }
}