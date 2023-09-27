using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Example.Common.UnitTests.Fixtures;
using Example.IdentityServer.Application.Services.Authentication;
using Example.IdentityServer.Controllers;
using Example.IdentityServer.Models;

namespace Example.IdentityServer.UnitTests.Controllers;

/// <summary>
/// Unit tests for the <see cref="AuthenticateController"/>.
/// </summary>
public sealed class AuthenticateControllerTests : TestFixture
{
    private readonly Mock<IAuthenticationService> _mockAuthService;
    private readonly AuthenticateController _controller;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticateControllerTests"/>.
    /// </summary>
    public AuthenticateControllerTests()
    {
        _mockAuthService = new();

        _controller = new AuthenticateController(_mockAuthService.Object);
    }

    [Fact]
    public void Constructor_NullAuthenticationService_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new AuthenticateController(null));
    }

    [Fact]
    public async Task Login_WithValidRequest_ReturnsOkResult()
    {
        // Arrange
        var loginRequest = Create<LoginRequest>();

        var expectedResult = Build<LoginResponse>()
                            .With(x => x.Error, string.Empty)
                            .With(x => x.ErrorCode, string.Empty)
                            .With(x => x.ErrorDescription, string.Empty)
                            .Create();

        _mockAuthService
            .Setup(x => x.AuthenticateAsync(loginRequest, It.IsAny<HttpContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.Login(loginRequest, It.IsAny<CancellationToken>());

        // Assert
        result.Should().BeOfType<OkObjectResult>()
              .Which.Value.Should().BeEquivalentTo(expectedResult);

        _mockAuthService
            .Verify(x => x.AuthenticateAsync(loginRequest, It.IsAny<HttpContext>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
    }

    [Fact]
    public async Task Login_WithInvalidRequest_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = Create<LoginRequest>();

        var expectedResult = Build<LoginResponse>()
                            .With(x => x.Error, "Authentication Error")
                            .With(x => x.ErrorCode, "401")
                            .Create();

        _mockAuthService
            .Setup(x => x.AuthenticateAsync(loginRequest, It.IsAny<HttpContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.Login(loginRequest, It.IsAny<CancellationToken>());

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();

        _mockAuthService
            .Verify(x => x.AuthenticateAsync(loginRequest, It.IsAny<HttpContext>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
    }
}
