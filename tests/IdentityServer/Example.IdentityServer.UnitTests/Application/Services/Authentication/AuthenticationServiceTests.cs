using Duende.IdentityServer.Configuration;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Example.Common.UnitTests.Fixtures;
using Example.IdentityServer.Domain.AggregatesModel.PersonAggregate;
using Example.IdentityServer.Domain.DomainServices;
using Example.IdentityServer.Models;
using AspNetCoreAuthenticationService = Microsoft.AspNetCore.Authentication.IAuthenticationService;
using AuthenticationOptions = Duende.IdentityServer.Configuration.AuthenticationOptions;
using AuthenticationService = Example.IdentityServer.Application.Services.Authentication.AuthenticationService;
using IAuthenticationService = Example.IdentityServer.Application.Services.Authentication.IAuthenticationService;


namespace Example.IdentityServer.UnitTests.Application.Services.Authentication;

/// <summary>
/// Unit tests for the <see cref="IAuthenticationService"/>.
/// </summary>
public sealed class AuthenticationServiceTests : TestFixture
{
    private readonly Mock<IIdentityServerInteractionService> _mockIdsService;
    private readonly Mock<IPersonService> _mockPersonService;
    private readonly Mock<AspNetCoreAuthenticationService> _mockASPCoreAuthService;
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly IAuthenticationService _authenticationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationServiceTests"/>.
    /// </summary>
    public AuthenticationServiceTests()
    {
        _mockIdsService = new();
        _mockPersonService = new();
        _mockServiceProvider = new();
        _mockASPCoreAuthService = new();

        _authenticationService = new AuthenticationService(Mock.Of<ILogger<AuthenticationService>>(),
                                                           _mockIdsService.Object,
                                                           _mockPersonService.Object);
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new AuthenticationService(null,
                                                                             _mockIdsService.Object,
                                                                             _mockPersonService.Object));
    }

    [Fact]
    public void Constructor_WithNullIdsService_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new AuthenticationService(Mock.Of<ILogger<AuthenticationService>>(),
                                                                             null,
                                                                             _mockPersonService.Object));
    }

    [Fact]
    public void Constructor_WithNullPersonService_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new AuthenticationService(Mock.Of<ILogger<AuthenticationService>>(),
                                                                             _mockIdsService.Object,
                                                                             null));
    }

    [Fact]
    public void AuthenticateAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _authenticationService.AuthenticateAsync(null, Mock.Of<HttpContext>(), It.IsAny<CancellationToken>()));
    }

    [Fact]
    public void AuthenticateAsync_WithNullHttpContext_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _authenticationService.AuthenticateAsync(Create<LoginRequest>(), null, It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task AuthenticateAsync_WithInvalidReturnUrl_ReturnsLoginFailedStatus()
    {
        // Arrange
        var loginRequest = Create<LoginRequest>();

        _mockIdsService
            .Setup(x => x.IsValidReturnUrl(loginRequest.ReturnUrl))
            .Returns(false);

        // Act
        var response = await _authenticationService.AuthenticateAsync(loginRequest, Mock.Of<HttpContext>(), It.IsAny<CancellationToken>());

        // Assert
        response.Status.Should().Be(Declares.LoginFailed);
        response.Error.Should().Be(Declares.NoContextFound);

        _mockIdsService
           .Verify(x => x.IsValidReturnUrl(loginRequest.ReturnUrl), Times.Exactly(1));
    }

    [Fact]
    public async Task AuthenticateAsync_WithNullAuthorizationContext_ReturnsLoginFailedStatus()
    {
        // Arrange
        var loginRequest = Create<LoginRequest>();

        AuthorizationRequest? nullAuthRequest = null;

        _mockIdsService
            .Setup(x => x.IsValidReturnUrl(loginRequest.ReturnUrl))
            .Returns(true);

        _mockIdsService
           .Setup(x => x.GetAuthorizationContextAsync(loginRequest.ReturnUrl))
           .ReturnsAsync(nullAuthRequest);

        // Act
        var response = await _authenticationService.AuthenticateAsync(loginRequest, Mock.Of<HttpContext>(), It.IsAny<CancellationToken>());

        // Assert
        response.Status.Should().Be(Declares.LoginFailed);
        response.Error.Should().Be(Declares.NoContextFound);

        _mockIdsService
           .Verify(x => x.IsValidReturnUrl(loginRequest.ReturnUrl), Times.Exactly(1));

        _mockIdsService
         .Verify(x => x.GetAuthorizationContextAsync(loginRequest.ReturnUrl), Times.Exactly(1));
    }

    [Fact]
    public async Task AuthenticateAsync_WithPersonNotFound_ReturnsLoginFailedStatus()
    {
        // Arrange
        var personEmail = Create<PersonEmail>();

        var loginRequest = Build<LoginRequest>()
                           .With(x => x.Email, personEmail.Value)
                           .Create();

        var authorizationRequest = Create<AuthorizationRequest>();

        Person? notFoundPerson = null;

        _mockIdsService
            .Setup(x => x.IsValidReturnUrl(loginRequest.ReturnUrl))
            .Returns(true);

        _mockIdsService
           .Setup(x => x.GetAuthorizationContextAsync(loginRequest.ReturnUrl))
           .ReturnsAsync(authorizationRequest);

        _mockPersonService
            .Setup(x => x.GetPersonByEmailAsync(personEmail, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notFoundPerson);

        // Act
        var response = await _authenticationService.AuthenticateAsync(loginRequest, Mock.Of<HttpContext>(), It.IsAny<CancellationToken>());

        // Assert
        response.Status.Should().Be(Declares.LoginFailed);
        response.Error.Should().Be(Declares.IncorrectUsernameOrPassword);

        _mockIdsService
           .Verify(x => x.IsValidReturnUrl(loginRequest.ReturnUrl), Times.Exactly(1));

        _mockIdsService
         .Verify(x => x.GetAuthorizationContextAsync(loginRequest.ReturnUrl), Times.Exactly(1));

        _mockPersonService
           .Verify(x => x.GetPersonByEmailAsync(personEmail, It.IsAny<CancellationToken>()), Times.Exactly(1));
    }

    [Fact]
    public async Task AuthenticateAsync_WithIncorrectPassword_ReturnsLoginFailedStatus()
    {
        // Arrange
        var person = Create<Person>();

        var loginRequest = Build<LoginRequest>()
                           .With(x => x.Email, person.Email.Value)
                           .Create();

        var authorizationRequest = Create<AuthorizationRequest>();

        _mockIdsService
            .Setup(x => x.IsValidReturnUrl(loginRequest.ReturnUrl))
            .Returns(true);

        _mockIdsService
           .Setup(x => x.GetAuthorizationContextAsync(loginRequest.ReturnUrl))
           .ReturnsAsync(authorizationRequest);

        _mockPersonService
            .Setup(x => x.GetPersonByEmailAsync(person.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(person);

        _mockPersonService
            .Setup(x => x.PasswordIsEqual(person, loginRequest.Password))
            .Returns(false);

        // Act
        var response = await _authenticationService.AuthenticateAsync(loginRequest, Mock.Of<HttpContext>(), It.IsAny<CancellationToken>());

        // Assert
        response.Status.Should().Be(Declares.LoginFailed);
        response.Error.Should().Be(Declares.IncorrectUsernameOrPassword);

        _mockIdsService
           .Verify(x => x.IsValidReturnUrl(loginRequest.ReturnUrl), Times.Exactly(1));

        _mockIdsService
         .Verify(x => x.GetAuthorizationContextAsync(loginRequest.ReturnUrl), Times.Exactly(1));

        _mockPersonService
           .Verify(x => x.GetPersonByEmailAsync(person.Email, It.IsAny<CancellationToken>()), Times.Exactly(1));

        _mockPersonService
           .Verify(x => x.PasswordIsEqual(person, loginRequest.Password), Times.Exactly(1));
    }

    [Fact]
    public async Task AuthenticateAsync_ValidRequest_ReturnsSuccessfulLogin()
    {
        // Arrange
        var person = Create<Person>();

        var loginRequest = Build<LoginRequest>()
                           .With(x => x.Email, person.Email.Value)
                           .Create();

        var authorizationRequest = Create<AuthorizationRequest>();

        _mockIdsService
            .Setup(x => x.IsValidReturnUrl(loginRequest.ReturnUrl))
            .Returns(true);

        _mockIdsService
           .Setup(x => x.GetAuthorizationContextAsync(loginRequest.ReturnUrl))
           .ReturnsAsync(authorizationRequest);

        _mockPersonService
            .Setup(x => x.GetPersonByEmailAsync(person.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(person);

        _mockPersonService
            .Setup(x => x.PasswordIsEqual(person, loginRequest.Password))
            .Returns(true);

        // The method 'httpContext.SignInAsync() is an extension method
        // that subsequently calls differece AspNetCore services. 
        // These have to mocked in order to correctly SignIn an user.
        _mockASPCoreAuthService
            .Setup(e => e.SignInAsync(It.IsAny<HttpContext>(),
                                      It.IsAny<string>(),
                                      It.IsAny<ClaimsPrincipal>(),
                                      It.IsAny<AuthenticationProperties>()))
            .Returns(Task.FromResult(null as object));

        _mockServiceProvider
            .Setup(e => e.GetService(typeof(AspNetCoreAuthenticationService)))
            .Returns(_mockASPCoreAuthService.Object);

        _mockServiceProvider
            .Setup(e => e.GetService(typeof(IdentityServerOptions)))
            .Returns(CreateMockedIdsOptions());

        var mockHttpContext = new Mock<HttpContext>();

        mockHttpContext
            .SetupGet(e => e.RequestServices)
            .Returns(_mockServiceProvider.Object);

        // Act
        var response = await _authenticationService.AuthenticateAsync(loginRequest, mockHttpContext.Object, It.IsAny<CancellationToken>());

        // Assert
        response.Status.Should().Be(Declares.LoginSuccess);

        _mockIdsService
           .Verify(x => x.IsValidReturnUrl(loginRequest.ReturnUrl), Times.Exactly(1));

        _mockIdsService
         .Verify(x => x.GetAuthorizationContextAsync(loginRequest.ReturnUrl), Times.Exactly(1));

        _mockPersonService
           .Verify(x => x.GetPersonByEmailAsync(person.Email, It.IsAny<CancellationToken>()), Times.Exactly(1));

        _mockPersonService
           .Verify(x => x.PasswordIsEqual(person, loginRequest.Password), Times.Exactly(1));

        _mockASPCoreAuthService
           .Verify(e => e.SignInAsync(It.IsAny<HttpContext>(),
                                     It.IsAny<string>(),
                                     It.IsAny<ClaimsPrincipal>(),
                                     It.IsAny<AuthenticationProperties>()), Times.Exactly(1));

        _mockServiceProvider
          .Verify(e => e.GetService(typeof(AspNetCoreAuthenticationService)), Times.Exactly(1));

        mockHttpContext
          .Verify(e => e.RequestServices, Times.Exactly(2));
    }

    private IdentityServerOptions CreateMockedIdsOptions()
    {
        var serverOptionsMock = Mock.Of<IdentityServerOptions>();

        var authOptionsMock = Mock.Of<AuthenticationOptions>();

        authOptionsMock.CookieAuthenticationScheme = "";

        serverOptionsMock.Authentication = authOptionsMock;

        return serverOptionsMock;
    }
}
