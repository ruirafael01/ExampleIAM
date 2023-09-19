using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using Visma.IdentityServer.Application.Stores;

namespace Visma.IdentityServer.Security;

/// <summary>
/// Implements the <see cref="IClientStore"/>
/// </summary>
internal sealed class CustomClientStore : IClientStore
{
    private readonly IVismaClientStore _vismaClientStore;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomClientStore"/>
    /// </summary>
    public CustomClientStore(IVismaClientStore vismaClientStore)
    {
        _vismaClientStore = vismaClientStore ?? throw new ArgumentNullException(nameof(vismaClientStore));
    }

    /// <inheritdoc />
    public Task<Client?> FindClientByIdAsync(string clientId)
        => Task.FromResult(_vismaClientStore.FindByClientId(clientId));
}
