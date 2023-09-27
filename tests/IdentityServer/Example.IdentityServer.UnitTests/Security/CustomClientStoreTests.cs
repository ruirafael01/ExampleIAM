using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using Example.Common.UnitTests.Fixtures;
using Example.IdentityServer.Application.Stores;
using Example.IdentityServer.Security;

namespace Example.IdentityServer.UnitTests.Security;

/// <summary>
/// Unit tests for the <see cref="CustomClientStore"/>.
/// </summary>
public sealed class CustomClientStoreTests : TestFixture
{
    private readonly Mock<IExampleClientStore> _mockExampleClientStore;
    private readonly IClientStore _clientStore;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomClientStoreTests"/>
    /// </summary>
    public CustomClientStoreTests()
    {
        _mockExampleClientStore = new();

        _clientStore = new CustomClientStore(_mockExampleClientStore.Object);
    }

    [Fact]
    public void Constructor_WithNullExampleStore_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CustomClientStore(null));
    }

    [Fact]
    public async Task FindClientById_WithValidClientId_ReturnsExpectedClient()
    {
        // Arrange
        var expected = Create<Client>();

        _mockExampleClientStore.Setup(x => x.FindByClientId(expected.ClientId))
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

        _mockExampleClientStore.Setup(x => x.FindByClientId(It.IsAny<string>()))
            .Returns(nullClient);

        // Act
        var result = await _clientStore.FindClientByIdAsync(It.IsAny<string>());

        // Assert
        result.Should().BeNull();
    }
}
