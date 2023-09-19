namespace Visma.IdentityServer.Infrastructure.Services;

/// <summary>
/// Date/Time provider.
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// Returns the current UTC time.
    /// </summary>
    DateTimeOffset DateTimeOffsetUtcNow { get; }
}