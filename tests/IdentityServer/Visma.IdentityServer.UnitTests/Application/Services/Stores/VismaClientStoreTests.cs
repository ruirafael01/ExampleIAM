using AutoFixture;
using Duende.IdentityServer.Models;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Visma.Common.UnitTests.Fixtures;
using Visma.IdentityServer.Application.Stores;
using Visma.IdentityServer.Domain.DomainOperations;
using Visma.IdentityServer.Models.Options;

namespace Visma.IdentityServer.UnitTests.Application.Services.Stores;

/// <summary>
/// Unit tests for the <see cref="IVismaClientStore"/>.
/// </summary>
public sealed class VismaClientStoreTests : TestFixture
{
    private readonly Mock<ICryptographer> _mockCryptographer;
    private readonly Mock<IOptions<ClientOptions>> _mockClientOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="VismaClientStoreTests"/>.
    /// </summary>
    public VismaClientStoreTests()
    {
        _mockCryptographer = new();
        _mockClientOptions = new();
    }

    [Fact]
    public void Constructor_WithNullCryptographer_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new VismaClientStore(null, Mock.Of<IOptions<ClientOptions>>()));
    }

    [Fact]
    public void Constructor_WithNullOptions_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new VismaClientStore(_mockCryptographer.Object, null));
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
        var vismaClientStore = new VismaClientStore(_mockCryptographer.Object, _mockClientOptions.Object);
        var response = vismaClientStore.Clients;

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
        var vismaClientStore = new VismaClientStore(_mockCryptographer.Object, _mockClientOptions.Object);
        var response = vismaClientStore.FindByClientId(client.ClientId);

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
        var vismaClientStore = new VismaClientStore(_mockCryptographer.Object, _mockClientOptions.Object);
        var response = vismaClientStore.FindByClientId(Create<string>());

        // Assert
        _mockCryptographer.Verify(x => x.Hash(It.IsAny<string>()), Times.Exactly(2));

        response.Should().BeNull();
    }
}