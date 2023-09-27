using Microsoft.AspNetCore.Identity;
using Example.Common.UnitTests.Fixtures;
using Example.IdentityServer.Domain.DomainOperations;

namespace Example.IdentityServer.Infrastructure.Operations;

/// <summary>
/// Unit tests for the <see cref="Cryptographer"/>.
/// </summary>
public sealed class CryptographerTests : TestFixture
{
    private readonly Mock<IPasswordHasher<string>> _mockHasher;
    private readonly ICryptographer _cryptographer;

    /// <summary>
    /// Initializes a new instance of the <see cref="CryptographerTests"/>
    /// </summary>
    public CryptographerTests()
    {
        _mockHasher = new();

        _cryptographer = new Cryptographer(_mockHasher.Object);
    }

    [Fact]
    public void Constructor_WithNullPasswordHasher_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Cryptographer(null));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Hash_WithInvalidInputString_ThrowsArgumentException(string input)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _cryptographer.Hash(input));
    }

    [Theory]
    [InlineData(null, "Test")]
    [InlineData("TESTADASDAS", null)]
    [InlineData("", "Test")]
    [InlineData("TESTADASDAS", "")]
    public void ValidateHash_WithInvalidInputStrings_ThrowsArgumentException(string hashedContent, string plainText)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _cryptographer.ValidateHash(hashedContent, plainText));
    }

    [Theory]
    [InlineData("Test")]
    [InlineData("PASSWORD123")]
    public void Hash_WithValidInputString_ReturnsHashedContent(string inputString)
    {
        // Arrange
        var expected = new string(CreateEnumeration<char>(100) + inputString);

        _mockHasher.Setup(x => x.HashPassword(string.Empty, inputString))
            .Returns(expected);
        
        // Act
        var result = _cryptographer.Hash(inputString);

        // Assert
        result.Should().BeEquivalentTo(expected);

        _mockHasher.Verify(x => x.HashPassword(string.Empty, inputString), Times.Exactly(1));
    }

    [Fact]
    public void ValidateHash_WithMismatch_ReturnsFalse()
    {
        // Arrange
        var hashedContent = Create<string>();
        var plainText = Create<string>();

        _mockHasher.Setup(x => x.VerifyHashedPassword(string.Empty, hashedContent, plainText))
            .Returns(PasswordVerificationResult.Failed);

        // Act
        var result = _cryptographer.ValidateHash(hashedContent, plainText);

        // Assert
        result.Should().BeFalse();

        _mockHasher.Verify(x => x.VerifyHashedPassword(string.Empty, hashedContent, plainText), Times.Exactly(1));
    }

    [Fact]
    public void ValidateHash_WithValidPassword_ReturnsTrue()
    {
        // Arrange
        var hashedContent = Create<string>();
        var plainText = Create<string>();

        _mockHasher.Setup(x => x.VerifyHashedPassword(string.Empty, hashedContent, plainText))
            .Returns(PasswordVerificationResult.Success);

        // Act
        var result = _cryptographer.ValidateHash(hashedContent, plainText);

        // Assert
        result.Should().BeTrue();

        _mockHasher.Verify(x => x.VerifyHashedPassword(string.Empty, hashedContent, plainText), Times.Exactly(1));
    }
}