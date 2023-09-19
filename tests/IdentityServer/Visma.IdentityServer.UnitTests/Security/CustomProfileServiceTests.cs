using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Visma.Common.UnitTests.Fixtures;
using Visma.IdentityServer.Domain.AggregatesModel.PersonAggregate;
using Visma.IdentityServer.Domain.DomainServices;
using Visma.IdentityServer.Security;

namespace Visma.IdentityServer.UnitTests.Security;

/// <summary>
/// Unit tests for the <see cref="CustomProfileServiceTests"/>.
/// </summary>
public sealed class CustomProfileServiceTests : TestFixture
{
    private readonly Mock<ILogger<CustomProfileService>> _mockLogger;
    private readonly Mock<IPersonService> _mockPersonService;
    private readonly IProfileService _profileService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomProfileServiceTests"/>.
    /// </summary>
    public CustomProfileServiceTests()
    {
        _mockLogger = new();
        _mockPersonService = new();

        _profileService = new CustomProfileService(_mockLogger.Object, _mockPersonService.Object);
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CustomProfileService(null, _mockPersonService.Object));
    }

    [Fact]
    public void Constructor_WithNullPersonService_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CustomProfileService(_mockLogger.Object, null));
    }

    [Fact]
    public async Task GetProfileDataAsync_WithValidSubjectId_ReturnsPersonClaims()
    {
        // Arrange
        var person = Create<Person>();

        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() 
                                    { new Claim(JwtClaimTypes.Subject, person.Id.ToString())
                                    }, Create<string>()));

        var context = Build<ProfileDataRequestContext>()
                      .FromFactory(() => Create<ProfileDataRequestContext>())
                      .With(x => x.Subject, claimsPrincipal)
                      .Create();

        _mockPersonService.Setup(x => x.GetPersonByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(person);

        // Act
        await _profileService.GetProfileDataAsync(context);

        // Assert
        context.IssuedClaims.Should().NotBeEmpty();
        _mockPersonService.Verify(x => x.GetPersonByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
    }

    [Fact]
    public async Task IsActiveAsync_WithValidSubjectId_ReturnsActive()
    {
        // Arrange
        var person = Create<Person>();

        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()
                                    { new Claim(JwtClaimTypes.Subject, person.Id.ToString())
                                    }, Create<string>()));

        var context = Build<IsActiveContext>()
                      .FromFactory(() => Create<IsActiveContext>())
                      .With(x => x.Subject, claimsPrincipal)
                      .Create();

        _mockPersonService.Setup(x => x.GetPersonByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(person);

        // Act
        await _profileService.IsActiveAsync(context);

        // Assert
        context.IsActive.Should().BeTrue();
        _mockPersonService.Verify(x => x.GetPersonByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
    }

    [Fact]
    public async Task IsActiveAsync_WithInvalidSubjectId_ReturnsInactive()
    {
        // Arrange
        Person? nullPerson = null;

        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()
                                    { new Claim(JwtClaimTypes.Subject, Create<string>())
                                    }, Create<string>()));

        var context = Build<IsActiveContext>()
                      .FromFactory(() => Create<IsActiveContext>())
                      .With(x => x.Subject, claimsPrincipal)
                      .Create();

        _mockPersonService.Setup(x => x.GetPersonByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(nullPerson);

        // Act
        await _profileService.IsActiveAsync(context);

        // Assert
        context.IsActive.Should().BeFalse();
        _mockPersonService.Verify(x => x.GetPersonByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
    }
}