using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using Example.IdentityServer.Application.Stores;

namespace Example.IdentityServer.Security;

/// <summary>
/// Implements the <see cref="IClientStore"/>
/// </summary>
internal sealed class CustomClientStore : IClientStore
{
    private readonly IExampleClientStore _ExampleClientStore;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomClientStore"/>
    /// </summary>
    public CustomClientStore(IExampleClientStore ExampleClientStore)
    {
        _ExampleClientStore = ExampleClientStore ?? throw new ArgumentNullException(nameof(ExampleClientStore));
    }

    /// <inheritdoc />
    public Task<Client?> FindClientByIdAsync(string clientId)
        => Task.FromResult(_ExampleClientStore.FindByClientId(clientId));
}
