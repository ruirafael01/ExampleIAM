using IdentityModel;
using System.Security.Claims;
using Example.Common.UnitTests.Fixtures;
using Example.IdentityServer.Domain.AggregatesModel.PersonAggregate;
using Example.IdentityServer.Extensions;

namespace Example.IdentityServer.UnitTests.Extensions;

/// <summary>
/// Unit tests for the extension methods for <see cref="PersonExtensions"/>.
/// </summary>
public sealed class PersonExtensionsTests : TestFixture
{
    [Fact]
    public void AsClaims_WithNullPerson_ThrowsArgumentNullException()
    {
        // Arrange
        Person? nullPerson = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => nullPerson.AsClaims());
    }

    [Fact]
    public void AsClaims_WithValidPerson_ReturnsExpectedListOfClaims()
    {
        // Arrange
        var person = Create<Person>();

        var expected = new List<Claim>()
        {
            new Claim(JwtClaimTypes.Subject, person.Id.ToString()),
            new Claim(JwtClaimTypes.Name, person.Name.ToString()),
            new Claim(JwtClaimTypes.Email, person.Email.Value),
            new Claim(JwtClaimTypes.Role, person.Role.ToString()),
            new Claim("canviewaccountingrecords", person.CanViewAccountingRecords.ToString())
        };

        // Act
        IReadOnlyList<Claim> response = person.AsClaims();

        // Assert
        response.Should().BeEquivalentTo(expected);
    }
}