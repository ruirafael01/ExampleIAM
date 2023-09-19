using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using Visma.Common.UnitTests.Fixtures;
using Visma.IdentityServer.Application.Stores;
using Visma.IdentityServer.Security;

namespace Visma.IdentityServer.UnitTests.Security;

/// <summary>
/// Unit tests for the <see cref="CustomClientStore"/>.
/// </summary>
public sealed class CustomClientStoreTests : TestFixture
{
    private readonly Mock<IVismaClientStore> _mockVismaClientStore;
    private readonly IClientStore _clientStore;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomClientStoreTests"/>
    /// </summary>
    public CustomClientStoreTests()
    {
        _mockVismaClientStore = new();

        _clientStore = new CustomClientStore(_mockVismaClientStore.Object);
    }

    [Fact]
    public void Constructor_WithNullVismaStore_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CustomClientStore(null));
    }

    [Fact]
    public async Task FindClientById_WithValidClientId_ReturnsExpectedClient()
    {
        // Arrange
        var expected = Create<Client>();

        _mockVismaClientStore.Setup(x => x.FindByClientId(expected.ClientId))
            .Returns(expected);

        // Act
        var result = await _clientStore.FindClientByIdAsync(expected.ClientId);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task FindClientById_WithInvalidClientId_ReturnsNull()
    {
        // Arrange
        Client? nullClient = null;

        _mockVismaClientStore.Setup(x => x.FindByClientId(It.IsAny<string>()))
            .Returns(nullClient);

        // Act
        var result = await _clientStore.FindClientByIdAsync(It.IsAny<string>());

        // Assert
        result.Should().BeNull();
    }
}
