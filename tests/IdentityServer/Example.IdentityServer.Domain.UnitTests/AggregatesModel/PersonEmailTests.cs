using Example.IdentityServer.Domain.AggregatesModel.PersonAggregate;
using Example.IdentityServer.Domain.Exceptions;

namespace Example.IdentityServer.Domain.UnitTests.AggregatesModel;

/// <summary>
/// Unit tests for the <see cref="PersonEmail"/>.
/// </summary>
public sealed class PersonEmailTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("test-t")]
    public void Constructor_WithInvalidEmail_ThrowsDomainException(string email)
    {
        // Act & Assert
        Assert.Throws<PersonException>(() => new PersonEmail(email));
    }
}