using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using Example.IdentityServer.Domain.AggregatesModel.PersonAggregate;

namespace Example.IdentityServer.Infrastructure.Extensions;

/// <summary>
/// Extension methods related to <see cref="ModelBuilder"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ModelBuilderExtensions
{
    /// <summary>
    /// Seed default data.
    /// </summary>
    /// <param name="modelBuilder">The builder.</param>
    /// <returns>An instance of a <see cref="ModelBuilder"/>.</returns>
    public static ModelBuilder SeedDefaultData(this ModelBuilder modelBuilder)
    {
        AddPersonLanguage(modelBuilder);
        AddPersonRole(modelBuilder);

        return modelBuilder;
    }

    private static void AddPersonRole(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<PersonRole>();

        builder.HasData(PersonRole.List());
    }

    private static void AddPersonLanguage(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<PersonLanguage>();

        builder.HasData(PersonLanguage.List());
    }
}