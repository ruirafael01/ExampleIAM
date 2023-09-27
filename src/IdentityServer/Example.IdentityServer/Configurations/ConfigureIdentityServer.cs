using Duende.IdentityServer;
using Duende.IdentityServer.Services;
using Example.Configurations.Helpers;
using Example.Configurations.Models;
using Example.IdentityServer.Models;
using Example.IdentityServer.Security;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;
using IdentityServerOptions = Example.IdentityServer.Models.Options.IdentityServerOptions;

namespace Example.IdentityServer.Configurations;

/// <summary>
/// Configure Identity Server related options.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class ConfigureIdentityServer
{
    /// <summary>
    /// Configure identity server.
    /// </summary>s
    /// <param name="appBuilder">The web application builder</param>
    /// <returns>The updated web application builder.</returns>
    public static WebApplicationBuilder ConfigureIdentityServerFunctionality(this WebApplicationBuilder appBuilder)
    {
        var configuration = appBuilder.Configuration;
        var environment = appBuilder.Environment;

        var endpoint = new EndpointConfiguration();

        var endpoints = configuration.GetSection("HttpServer:Endpoints")
                       .GetChildren()
                       .ToDictionary(section => section.Key, section =>
                       {

                           section.Bind(endpoint);
                           return endpoint;
                       });

        var certificate = CertificateHelper.LoadCertificate(endpoint, environment);

        var ecdsa = certificate.GetECDsaPrivateKey();

        if (ecdsa is null)
            throw new InvalidOperationException("Cannot configure identity server without Elliptic Curve certificate.");

        var securityKey = new ECDsaSecurityKey(ecdsa) { KeyId = "ef208a01ef43406f833b267023766550" };

        appBuilder.Services.AddTransient<IProfileService, CustomProfileService>();

        var identityServerOptions = appBuilder.Configuration.GetSection("IdentityServerOptions").Get<IdentityServerOptions>();

        if (identityServerOptions is null)
            throw new InvalidOperationException("Cannot register IdentityServer with IdentityServerOptions as null.");

        appBuilder.Services.AddIdentityServer(options =>
        {
            options.Events.RaiseSuccessEvents = true;
            options.Events.RaiseFailureEvents = true;
            options.Events.RaiseErrorEvents = true;

            options.UserInteraction.LoginUrl = identityServerOptions.LoginUrl;
            options.UserInteraction.ErrorUrl = identityServerOptions.ErrorUrl;
            options.UserInteraction.LogoutUrl = identityServerOptions.LogoutUrl;
            options.IssuerUri = identityServerOptions.IssuerUri;
        })
            .AddSigningCredential(securityKey, IdentityServerConstants.ECDsaSigningAlgorithm.ES256)
            .AddInMemoryIdentityResources(Config.GetIdentityResources())
            .AddInMemoryApiResources(Config.GetApis())
            .AddInMemoryApiScopes(Config.GetApiScopes())
            .AddClientStore<CustomClientStore>()
            .AddSecretValidator<CustomSecretValidator>()
            .AddExtensionGrantValidator<CustomDelegationGrantValidator>()
            .AddProfileService<CustomProfileService>();

        string? googleClientId = appBuilder.Configuration["Authentication:Google:ClientId"];
        string? googleSecret = appBuilder.Configuration["Authentication:Google:ClientSecret"];

        if (string.IsNullOrEmpty(googleClientId)
           || string.IsNullOrEmpty(googleSecret))
            throw new InvalidOperationException("Cannot use Google external provider with null/empty ClientID or null/emtpy ClientSecret");

        appBuilder.Services.AddAuthentication()
                .AddGoogle("Google", options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.ClientId = googleClientId;
                    options.ClientSecret = googleSecret;
                });

        appBuilder.Services.AddTransient<IReturnUrlParser, CustomReturnUrlParser>();

        return appBuilder;
    }
}