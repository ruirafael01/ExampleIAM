namespace Visma.Configurations.Models;

/// <summary>
/// Represents an endpoint configuration.
/// </summary>
public sealed record EndpointConfiguration
{
    /// <summary>
    /// Gets or sets the host name.
    /// </summary>
    public string Host { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the host port.
    /// </summary>
    public int? Port { get; set; }

    /// <summary>
    /// Gets or sets the host scheme.
    /// </summary>
    public string Scheme { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the store name for the HTTPS certificate.
    /// </summary>
    public string StoreName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the store location for the HTTPS certificate.
    /// </summary>
    public string StoreLocation { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the HTTPS certificate thumbprint.
    /// </summary>
    public string Thumbprint { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the path to the HTTPS certificate, this is a fallback in case <see cref="StoreName"/> 
    /// and <see cref="StoreLocation"/> are not provided.
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password for the HTTPS certificate, this is a fallback in case <see cref="StoreName"/> 
    /// and <see cref="StoreLocation"/> are not provided.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
