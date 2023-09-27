using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;
using System.Collections.Specialized;
using Example.Common.UnitTests.Fixtures;
using Example.IdentityServer.Domain.AggregatesModel.PersonAggregate;
using Example.IdentityServer.Domain.DomainServices;
using Example.IdentityServer.Security;

namespace Example.IdentityServer.UnitTests.Security;

/// <summary>
/// Unit tests for the <see cref="CustomDelegationGrantValidator"/>.
/// </summary>
public sealed class CustomDelegationGrantValidatorTests : TestFixture
{
    private readonly Mock<IPersonService> _mockPersonService;
    private readonly Mock<ITokenValidator> _mockTokenValidator;
    private readonly IExtensionGrantValidator _grantValidator;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomDelegationGrantValidatorTests"/>
    /// </summary>
    public CustomDelegationGrantValidatorTests()
    {
        _mockPersonService = new();
        _mockTokenValidator = new();

        _grantValidator = new CustomDelegationGrantValidator(_mockTokenValidator.Object, _mockPersonService.Object);
    }

    [Fact]
    public void Constructor_WithNullTokenValidator_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CustomDelegationGrantValidator(null, _mockPersonService.Object));
    }

    [Fact]
    public void Constructor_WithNullPersonService_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CustomDelegationGrantValidator(_mockTokenValidator.Object, null));
    }

    [Fact]
    public void ValidateAsync_WithNullContext_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _grantValidator.ValidateAsync(null));
    }

    [Fact]
    public async Task ValidateAsync_WithNoTokenInRequest_ReturnsInvalidGrant()
    {
        // Arrange
        var rawRequest = new NameValueCollection()
        {
            { "scope",  Create<string>() }
        };

        var request = Build<ValidatedTokenRequest>()
                      .FromFactory(() => Create<ValidatedTokenRequest>())
                      .With(x => x.Raw, rawRequest)
                      .Create();

        var context = Build<ExtensionGrantValidationContext>()
                        .FromFactory(() => Create<ExtensionGrantValidationContext>())
                        .With(x => x.Request, request)
                        .Create();

        var expected = new GrantValidationResult(TokenRequestErrors.InvalidGrant);

        // Act
        await _grantValidator.ValidateAsync(context);

        // Assert
        context.Result.IsError.Should().BeTrue();
        context.Result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task ValidateAsync_WithNoScopeInRequest_ReturnsInvalidGrant()
    {
        // Arrange
        var rawRequest = new NameValueCollection()
        {
            { "token", Create<string>() }
        };

        var request = Build<ValidatedTokenRequest>()
                      .FromFactory(() => Create<ValidatedTokenRequest>())
                      .With(x => x.Raw, rawRequest)
                      .Create();

        var context = Build<ExtensionGrantValidationContext>()
                        .FromFactory(() => Create<ExtensionGrantValidationContext>())
                        .With(x => x.Request, request)
                        .Create();

        var expected = new GrantValidationResult(TokenRequestErrors.InvalidGrant);

        // Act
        await _grantValidator.ValidateAsync(context);

        // Assert
        context.Result.IsError.Should().BeTrue();
        context.Result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task ValidateAsync_WithInvalidToken_ReturnsInvalidGrant()
    {
        // Arrange
        var rawRequest = new NameValueCollection()
        {
            { "token", Create<string>() },
            { "scope", Create<string>() }
        };

        var request = Build<ValidatedTokenRequest>()
                      .FromFactory(() => Create<ValidatedTokenRequest>())
                      .With(x => x.Raw, rawRequest)
                      .Create();

        var context = Build<ExtensionGrantValidationContext>()
                        .FromFactory(() => Create<ExtensionGrantValidationContext>())
                        .With(x => x.Request, request)
                        .Create();

        var validationError = Build<TokenValidationResult>()
                              .FromFactory(() => Create<TokenValidationResult>())
                              .With(x => x.IsError, true)
                              .Create();

        _mockTokenValidator.Setup(x => x.ValidateAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(validationError);

        var expected = new GrantValidationResult(TokenRequestErrors.InvalidGrant);

        // Act
        await _grantValidator.ValidateAsync(context);

        // Assert
        context.Result.IsError.Should().BeTrue();
        context.Result.Should().BeEquivalentTo(expected);

        _mockTokenValidator.Verify(x => x.ValidateAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(1));
    }

    [Fact]
    public async Task ValidateAsync_WithValidRequest_ReturnsSuccessGrant()
    {
        // Arrange
        var person = Create<Person>();

        var rawRequest = new NameValueCollection()
        {
            { "token", CorrectlyFormedToken() },
            { "scope", "reporting" }
        };

        var request = Build<ValidatedTokenRequest>()
                      .FromFactory(() => Create<ValidatedTokenRequest>())
                      .With(x => x.Raw, rawRequest)
                      .Create();

        var context = Build<ExtensionGrantValidationContext>()
                        .FromFactory(() => Create<ExtensionGrantValidationContext>())
                        .With(x => x.Request, request)
                        .Create();

        var validationError = Build<TokenValidationResult>()
                              .FromFactory(() => Create<TokenValidationResult>())
                              .With(x => x.IsError, false)
                              .Create();

        _mockTokenValidator.Setup(x => x.ValidateAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(validationError);

        _mockPersonService.Setup(x => x.GetPersonByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(person);

        // Act
        await _grantValidator.ValidateAsync(context);

        // Assert
        context.Result.IsError.Should().BeFalse();

        _mockTokenValidator.Verify(x => x.ValidateAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(1));
        _mockPersonService.Verify(x => x.GetPersonByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
    }

    private string CorrectlyFormedToken()
        => "Bearer eyJhbGciOiJFUzI1NiIsImtpZCI6ImVmMjA4YTAxZWY0MzQwNmY4MzNiMjY3MDIzNzY2NTUwIiwidHlwIjoiYXQrand0In0.eyJpc3MiOiJodHRwczovL3Zpc" +
           "21hLWVjb25vbWljLmNvbTo3MDMyLyIsIm5iZiI6MTY4NzY5NTcyMiwiaWF0IjoxNjg3Njk1NzIyLCJleHAiOjE2ODc2OTc1MjIsInNjb3BlIjpbIm9wZW5pZCIsInByb2Z" +
          "pbGUiLCJlY29ub21pYyIsIm9mZmxpbmVfYWNjZXNzIl0sImFtciI6WyJwd2QiXSwiY2xpZW50X2lkIjoiRWNvbm9taWMiLCJzdWIiOiIxNjZiMzdiMC03YmY2LTQ5OTUtOTdlMS" +
          "0wOGRiNzE2N2QzMWQiLCJhdXRoX3RpbWUiOjE2ODc2OTIwMzEsImlkcCI6ImxvY2FsIiwibmFtZSI6IlJ1aSBSYWZhZWwiLCJlbWFpbCI6InJvbnNwd2FuMTFAZ21haWwuY29tIiw" +
          "icm9sZSI6ImFkbWluaXN0cmF0b3IiLCJjYW52aWV3YWNjb3VudGluZ3JlY29yZHMiOiJUcnVlIiwic2lkIjoiQkVERTc1OUQ0QkZGNzIwRTZCMTFENDQ0N0I1N0NDNUEiLCJqd" +
          "GkiOiI5QjEwMkQ1QUZCNjUxODRGNTgwMzQ1OEYxQUMwQkI1QiJ9.-hj9up9CNqapqVdtipaM0DfroPmCTZMGo6sezL9-FSltCQ7ZsQ2rNR8JTp9IJy6GYKYv9HP8EBKJm65MI6Vmlg";
}