using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Example.IdentityServer.Models.Options;
using Example.IdentityServer.Security;

namespace Example.IdentityServer.UnitTests.Security;

/// <summary>
/// Unit tests for the <see cref="CustomCorsPolicyService"/>.
/// </summary>
public sealed class CustomCorsPolicyServiceTests
{
    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CustomCorsPolicyService(null, Mock.Of<IOptions<ClientOptions>>()));
    }

    [Fact]
    public void Constructor_WithNullOptions_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CustomCorsPolicyService(Mock.Of<ILogger<CustomCorsPolicyService>>(), null));
    }
}
