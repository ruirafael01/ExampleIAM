using Duende.IdentityServer.Models;

namespace Visma.IdentityServer.Models.Options;

/// <summary>
/// Options for list of clients.
/// </summary>
public sealed record ClientOptions
{
    /// <summary>
    /// The list of defined clients.
    /// </summary>
    public List<Client> Clients { get; init; } = new List<Client>();
}
