namespace Example.IdentityServer.Models.Options;

/// <summary>
/// The database options model.
/// </summary>
public sealed record DatabaseOptions
{
    /// <summary>
    /// The connection string that the API will use.
    /// </summary>
    public string ConnectionString { get; init; } = string.Empty;
}