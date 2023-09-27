using Example.Common.UnitTests.Extensions;
using Example.Common.UnitTests.Fixtures;
using Example.IdentityServer.Domain.AggregatesModel.PersonAggregate;
using Example.IdentityServer.Domain.Exceptions;

namespace Example.IdentityServer.Domain.UnitTests.AggregatesModel;

/// <summary>
/// Unit tests for the <see cref="Person"/>.
/// </summary>
public sealed class PersonTests : TestFixture
{
    [Fact]
    public void Constructor_WithNullPassword_ThrowsDomainException()
    {
        // Act & Assert
        Assert.Throws<PersonException>(() => new Person(null,
                                                        Create<PersonName>(),
                                                        Create<PersonRole>(),
                                                        Create<PersonLanguage>(),
                                                        Create<PersonEmail>()));
    }

    [Fact]
    public void Constructor_WithNullName_ThrowsDomainException()
    {
        // Act & Assert
        Assert.Throws<PersonException>(() => new Person(Create<PersonPassword>(),
                                                        null,
                                                        Create<PersonRole>(),
                                                        Create<PersonLanguage>(),
                                                        Create<PersonEmail>()));
    }

    [Fact]
    public void Constructor_WithNullRole_ThrowsDomainException()
    {
        // Act & Assert
        Assert.Throws<PersonException>(() => new Person(Create<PersonPassword>(),
                                                        Create<PersonName>(),
                                                        null,
                                                        Create<PersonLanguage>(),
                                                        Create<PersonEmail>()));
    }

    [Fact]
    public void Constructor_WithNullLanguage_ThrowsDomainException()
    {
        // Act & Assert
        Assert.Throws<PersonException>(() => new Person(Create<PersonPassword>(),
                                                        Create<PersonName>(),
                                                        Create<PersonRole>(),
                                                        null,
                                                        Create<PersonEmail>()));
    }

    [Fact]
    public void Constructor_WithNullEmail_ThrowsDomainException()
    {
        // Act & Assert
        Assert.Throws<PersonException>(() => new Person(Create<PersonPassword>(),
                                                        Create<PersonName>(),
                                                        Create<PersonRole>(),
                                                        Create<PersonLanguage>(),
                                                        null));
    }

    [Fact]
    public void ChangeRole_WithAdministratorRole_EnforcesAccountingViewingRecords()
    {
        // Arrange
        var person = Build<Person>()
                     .FromFactory(() => Create<Person>())
                     .WithCustomPropertyValue(x => x.Role, PersonRole.Standard)
                     .WithCustomPropertyValue(x => x.CanViewAccountingRecords, false)
                     .Create();

        // Act
        person.ChangeRole(PersonRole.Administrator);

        // Assert
        person.Role.Should().Be(PersonRole.Administrator);
        person.CanViewAccountingRecords.Should().BeTrue();
    }
}