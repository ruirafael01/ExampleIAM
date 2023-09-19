namespace Visma.IdentityServer.Infrastructure.Services;

/// <summary>
/// Implements <see cref="IDateTimeProvider"/>.
/// </summary>
public sealed class DateTimeProvider : IDateTimeProvider
{
    /// <inheritdoc/>
    public DateTimeOffset DateTimeOffsetUtcNow => DateTimeOffset.UtcNow;
}