﻿using Duende.IdentityServer.Models;
using Example.IdentityServer.Domain.DomainOperations;
using Example.IdentityServer.Models.Options;
using Microsoft.Extensions.Options;

namespace Example.IdentityServer.Application.Stores;

/// <summary>
/// Implements the <see cref="IExampleClientStore"/>
/// </summary>
public sealed class ExampleClientStore : IExampleClientStore
{
    private readonly ICryptographer _cryptographer;
    private readonly IReadOnlyList<Client> _clients;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomClientStore"/>
    /// </summary>
    public ExampleClientStore(ICryptographer cryptographer, IOptions<ClientOptions> options)
    {
        _cryptographer = cryptographer ?? throw new ArgumentNullException(nameof(cryptographer));
        _ = options ?? throw new ArgumentNullException(nameof(options));
        _clients = ValidateAndReturnClients(options);
    }

    /// <inheritdoc />
    public IReadOnlyList<Client> Clients => _clients;

    /// <inheritdoc />
    public Client? FindByClientId(string clientId)
        => _clients.FirstOrDefault(x => x.ClientId.Equals(clientId));

    private IReadOnlyList<Client> ValidateAndReturnClients(IOptions<ClientOptions> options)
    {
        var clientsSecrets = options
                                  .Value
                                  .Clients
                                  .Where(x => x.ClientSecrets is not null && x.ClientSecrets.Any())
                                  .Select(x => x.ClientSecrets)
                                  .SelectMany(x => x);

        foreach (var client in clientsSecrets)
            client.Value = _cryptographer.Hash(client.Value);

        return options.Value.Clients;
    }
}