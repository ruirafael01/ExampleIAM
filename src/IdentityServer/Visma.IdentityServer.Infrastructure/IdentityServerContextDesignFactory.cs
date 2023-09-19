using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Diagnostics.CodeAnalysis;
using Visma.IdentityServer.Infrastructure.Services;

namespace Visma.IdentityServer.Infrastructure;

/// <summary>
/// For design time and local development support only.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class IdentityServerContextDesignFactory : IDesignTimeDbContextFactory<IdentityServerContext>
{
    /// <summary>
    /// Creates a new instance of a derived context.
    /// </summary>
    /// <param name="args">Arguments provided by the design-time service.</param>
    /// <returns>
    /// An instance of <typeparamref name="TContext" />.
    /// </returns>
    public IdentityServerContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<IdentityServerContext>()
        .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Visma.IdentityServer;Trusted_Connection=True;ConnectRetryCount=0");

        return new IdentityServerContext(optionsBuilder.Options, new DateTimeProvider());
    }
}