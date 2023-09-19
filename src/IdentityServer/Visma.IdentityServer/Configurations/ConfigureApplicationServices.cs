using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using Duende.IdentityServer.Validation;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;
using Visma.IdentityServer.Application.Database;
using Visma.IdentityServer.Application.Stores;
using Visma.IdentityServer.Domain.AggregatesModel.PersonAggregate;
using Visma.IdentityServer.Domain.DomainOperations;
using Visma.IdentityServer.Domain.DomainServices;
using Visma.IdentityServer.Infrastructure.Operations;
using Visma.IdentityServer.Infrastructure.Repositories;
using Visma.IdentityServer.Infrastructure.Services;
using Visma.IdentityServer.Security;
using AuthenticationService = Visma.IdentityServer.Application.Services.Authentication.AuthenticationService;
using IAuthenticationService = Visma.IdentityServer.Application.Services.Authentication.IAuthenticationService;
using VismaClientStore = Visma.IdentityServer.Application.Stores.VismaClientStore;

namespace Visma.IdentityServer.Configurations;

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
        builder.Services.AddSingleton<IVismaClientStore, VismaClientStore>();

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