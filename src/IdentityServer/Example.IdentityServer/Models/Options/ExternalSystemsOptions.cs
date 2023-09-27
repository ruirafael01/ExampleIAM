namespace Example.IdentityServer.Models.Options;

/// <summary>
/// The options for communication with external systems.
/// </summary>
public sealed record ExternalSystemsOptions
{
    /// <summary>
    /// The connection string to redis.
    /// </summary>
    public string RedisConnectionString { get; init; } = string.Empty;
}