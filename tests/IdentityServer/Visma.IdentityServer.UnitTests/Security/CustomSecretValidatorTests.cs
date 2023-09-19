using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;
using Visma.Common.UnitTests.Fixtures;
using Visma.IdentityServer.Domain.DomainOperations;
using Visma.IdentityServer.Security;

namespace Visma.IdentityServer.UnitTests.Security;

/// <summary>
/// Unit tests for the <see cref="CustomSecretValidator"/>.
/// </summary>
public sealed class CustomSecretValidatorTests : TestFixture
{
    private readonly Mock<ICryptographer> _mockCryptographer;
    private readonly ISecretValidator _secretValidator;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomSecretValidatorTests"/>.
    /// </summary>
    public CustomSecretValidatorTests()
    {
        _mockCryptographer = new();

        _secretValidator = new CustomSecretValidator(_mockCryptographer.Object);
    }

    [Fact]
    public void Constructor_WithNullCryptographer_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CustomSecretValidator(null));
    }

    [Fact]
    public void ValidateAsync_WithNullSecrets_ThrowsArgumentNullException()
    {
        // Arrange
        var parsedSecret = Create<ParsedSecret>();

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _secretValidator.ValidateAsync(null, parsedSecret));
    }

    [Fact]
    public void ValidateAsync_WithNullParsedSecret_ThrowsArgumentNullException()
    {
        // Arrange
        var secrets = CreateEnumeration<Secret>(2);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _secretValidator.ValidateAsync(secrets, null));
    }

    [Fact]
    public async Task ValidateAsync_WithValidSecret_ReturnsSucess()
    {
        // Arrange
        var secrets = CreateEnumeration<Secret>();
        var parsedSecret = Build<ParsedSecret>()
                          .With(x => x.Credential, Create<string>())
                          .Create();

        var expected = new SecretValidationResult()
        {
            IsError = false,
            Success = true
        };

        _mockCryptographer.Setup(x => x.ValidateHash(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);

        // Act
        var response = await _secretValidator.ValidateAsync(secrets.ToArray(), parsedSecret);

        // Assert
        response.Should().BeEquivalentTo(expected);

        _mockCryptographer.Verify(x => x.ValidateHash(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(1));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(20)]
    public async Task ValidateAsync_WithInvalidSecret_ReturnsFail(int numberOfSecrets)
    {
        // Arrange
        var secrets = CreateEnumeration<Secret>(numberOfSecrets);
        var parsedSecret = Build<ParsedSecret>()
                          .With(x => x.Credential, Create<string>())
                          .Create();

        var expected = new SecretValidationResult()
        {
            IsError = true,
            Success = false
        };

        _mockCryptographer.Setup(x => x.ValidateHash(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(false);

        // Act
        var response = await _secretValidator.ValidateAsync(secrets.ToArray(), parsedSecret);

        // Assert
        response.IsError.Should().Be(expected.IsError);
        response.Success.Should().Be(expected.Success);

        _mockCryptographer.Verify(x => x.ValidateHash(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(numberOfSecrets));
    }
}
