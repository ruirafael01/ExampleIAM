using Example.IdentityServer.Application.Database;
using Example.IdentityServer.Infrastructure;
using Example.IdentityServer.Models.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace Example.IdentityServer.Configurations;

/// <summary>
/// Database configurations.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class ConfigureDatabase
{
    /// <summary>
    /// Adds database to the web application builder.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <returns>The updated web application builder.</returns>
    public static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<IdentityServerContext>((ctx, options) =>
        {
            var databaseOptions = ctx.GetRequiredService<IOptionsSnapshot<DatabaseOptions>>();

            options.UseSqlServer(databaseOptions.Value.ConnectionString,
                                 sqlOptions =>
                                 {
                                     var assemblyName = typeof(IdentityServerContext).Assembly.GetName().Name;
                                     sqlOptions.MigrationsAssembly(assemblyName);
                                     sqlOptions.EnableRetryOnFailure(10, TimeSpan.FromSeconds(30), null);
                                 });
        }, ServiceLifetime.Scoped);

        return builder;
    }

    /// <summary>
    /// Seeds the database with correct data.
    /// </summary>
    /// <param name="app">The app app.</param>
    public static void SeedData(this WebApplication app)
    {
        var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

        if (scopedFactory is null)
            throw new InvalidOperationException($"Cannot seed database with null {nameof(IServiceScopeFactory)}");

        using var scope = scopedFactory.CreateScope();

        var service = scope.ServiceProvider.GetService<IDatabaseSeeder>();

        if (service is null)
            throw new InvalidOperationException($"Cannot seed database with null {nameof(IDatabaseSeeder)}");

        // since this is used in app initialiation
        // task needs run synchronously
        //service.SeedData().Wait();
    }
}