using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using Duende.IdentityServer.Validation;
using Example.IdentityServer.Application.Database;
using Example.IdentityServer.Application.Stores;
using Example.IdentityServer.Domain.AggregatesModel.PersonAggregate;
using Example.IdentityServer.Domain.DomainOperations;
using Example.IdentityServer.Domain.DomainServices;
using Example.IdentityServer.Infrastructure.Operations;
using Example.IdentityServer.Infrastructure.Repositories;
using Example.IdentityServer.Infrastructure.Services;
using Example.IdentityServer.Security;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;
using AuthenticationService = Example.IdentityServer.Application.Services.Authentication.AuthenticationService;
using ExampleClientStore = Example.IdentityServer.Application.Stores.ExampleClientStore;
using IAuthenticationService = Example.IdentityServer.Application.Services.Authentication.IAuthenticationService;

namespace Example.IdentityServer.Configurations;

/// <summary>
/// Configure application required services.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class ConfigureApplicationServices
{
    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        // Application
        builder.Services.AddTransient<IDatabaseSeeder, DatabaseSeeder>();
        builder.Services.AddTransient<IProfileService, CustomProfileService>();
        builder.Services.AddTransient<ISecretValidator, CustomSecretValidator>();
        builder.Services.AddTransient<IExtensionGrantValidator, CustomDelegationGrantValidator>();
        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
        builder.Services.AddSingleton<IClientStore, CustomClientStore>();
        builder.Services.AddSingleton<IExampleClientStore, ExampleClientStore>();

        // Infrastructure
        builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        builder.Services.AddScoped<IPersonRepository, PersonRepository>();
        builder.Services.AddTransient<ICryptographer, Cryptographer>();
        builder.Services.AddTransient<IPasswordHasher<string>, PasswordHasher<string>>();

        // Domain
        builder.Services.AddScoped<IPersonService, PersonService>();

        return builder;
    }
}