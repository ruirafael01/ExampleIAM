using AspNetCoreRateLimit;
using System.IO.Pipelines;
using System.Text;
using System.Text.Json;
using Visma.IdentityServer.Models;

namespace Visma.IdentityServer.Security;

/// <summary>
/// Implements the <see cref="IClientResolveContributor"/>.
/// This custom resolver should only be applied on authentication endpoints
/// that use <see cref="LoginRequest"/> as the request body.
/// </summary>
internal sealed class EmailCustomResolver : IClientResolveContributor
{
    private const string _authenticationPath = "/api/authenticate";
    private readonly ILogger<EmailCustomResolver> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailCustomResolver"/> class.
    /// </summary>
    /// <param name="logger">Logger</param>
    public EmailCustomResolver(ILogger<EmailCustomResolver> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<string> ResolveClientAsync(HttpContext httpContext)
    {
        var email = await GetEmail(httpContext);

        if (!string.IsNullOrEmpty(email))
            return email;

        _logger.LogWarning("Failed to obtain email from JWT Token in rate limiting middleware.");

        return string.Empty;
    }

    private async Task<string> GetEmail(HttpContext context)
    {
        try
        {
            if (context.Request.Path.Value?.Equals(_authenticationPath) is false
                || context.Request.Method != "POST")
                return string.Empty;

            string requestBody;

            var writer = PipeWriter.Create(context.Request.Body);

            using var reader = new StreamReader(writer.AsStream(), Encoding.UTF8);

            requestBody = await reader.ReadToEndAsync();

            var loginRequest = JsonSerializer.Deserialize<Dictionary<string, object>>(requestBody);

            if (loginRequest is null)
                return string.Empty;

            string? email = loginRequest["email"]?.ToString();

            if (string.IsNullOrEmpty(email))
                return string.Empty;

            return email;
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "An error occurred when trying to get the email from login request.");

            return string.Empty;
        }
    }
}