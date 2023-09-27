using AutoFixture;
using Duende.IdentityServer.Models;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Example.Common.UnitTests.Fixtures;
using Example.IdentityServer.Application.Stores;
using Example.IdentityServer.Domain.DomainOperations;
using Example.IdentityServer.Models.Options;

namespace Example.IdentityServer.UnitTests.Application.Services.Stores;

/// <summary>
/// Unit tests for the <see cref="IExampleClientStore"/>.
/// </summary>
public sealed class ExampleClientStoreTests : TestFixture
{
    private readonly Mock<ICryptographer> _mockCryptographer;
    private readonly Mock<IOptions<ClientOptions>> _mockClientOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExampleClientStoreTests"/>.
    /// </summary>
    public ExampleClientStoreTests()
    {
        _mockCryptographer = new();
        _mockClientOptions = new();
    }

    [Fact]
    public void Constructor_WithNullCryptographer_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ExampleClientStore(null, Mock.Of<IOptions<ClientOptions>>()));
    }

    [Fact]
    public void Constructor_WithNullOptions_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ExampleClientStore(_mockCryptographer.Object, null));
    }

    [Theory]
    [InlineData(2)]
    [InlineData(100)]
    public void Constructor_WithClientSecretsInOptions_HashesTheClientSecretsAndReturnsValidClientList(int numberOfSecrets)
    {
        // Arrange
        ICollection<Secret> secrets = CreateEnumeration<Secret>(numberOfSecrets).ToList();

        var client = Build<Client>()
                      .With(x => x.ClientSecrets, secrets)
                      .Create();

        var clientOptions = Build<ClientOptions>()
                    .With(x => x.Clients, new List<Client>() { client })
                    .Create();

        _mockClientOptions.SetupGet(x => x.Value)
            .Returns(clientOptions);

        _mockCryptographer.Setup(x => x.Hash(It.IsAny<string>()))
                           .Returns(new string(CreateEnumeration<char>(200).ToArray()));

        // Act
        var ExampleClientStore = new ExampleClientStore(_mockCryptographer.Object, _mockClientOptions.Object);
        var response = ExampleClientStore.Clients;

        // Assert
        _mockCryptographer.Verify(x => x.Hash(It.IsAny<string>()), Times.Exactly(numberOfSecrets));

        response.Count().Should().Be(1);
    }

    [Fact]
    public void FindByClientId_WithValidClient_ReturnsValidClient()
    {
        // Arrange
        ICollection<Secret> secrets = CreateEnumeration<Secret>(2).ToList();

        var client = Build<Client>()
                      .With(x => x.ClientSecrets, secrets)
                      .Create();

        var clientOptions = Build<ClientOptions>()
                    .With(x => x.Clients, new List<Client>() { client })
                    .Create();

        _mockClientOptions.SetupGet(x => x.Value)
            .Returns(clientOptions);

        _mockCryptographer.Setup(x => x.Hash(It.IsAny<string>()))
                           .Returns(new string(CreateEnumeration<char>(200).ToArray()));

        // Act
        var ExampleClientStore = new ExampleClientStore(_mockCryptographer.Object, _mockClientOptions.Object);
        var response = ExampleClientStore.FindByClientId(client.ClientId);

        // Assert
        _mockCryptographer.Verify(x => x.Hash(It.IsAny<string>()), Times.Exactly(2));

        response.Should().BeEquivalentTo(client);
    }

    [Fact]
    public void FindByClientId_WithInvalidClient_ReturnsNull()
    {
        // Arrange
        ICollection<Secret> secrets = CreateEnumeration<Secret>(2).ToList();

        var client = Build<Client>()
                      .With(x => x.ClientSecrets, secrets)
                      .Create();

        var clientOptions = Build<ClientOptions>()
                    .With(x => x.Clients, new List<Client>() { client })
                    .Create();

        _mockClientOptions.SetupGet(x => x.Value)
            .Returns(clientOptions);

        _mockCryptographer.Setup(x => x.Hash(It.IsAny<string>()))
                           .Returns(new string(CreateEnumeration<char>(200).ToArray()));

        // Act
        var ExampleClientStore = new ExampleClientStore(_mockCryptographer.Object, _mockClientOptions.Object);
        var response = ExampleClientStore.FindByClientId(Create<string>());

        // Assert
        _mockCryptographer.Verify(x => x.Hash(It.IsAny<string>()), Times.Exactly(2));

        response.Should().BeNull();
    }
}