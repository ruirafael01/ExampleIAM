using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;
using Example.IdentityServer.Domain.AggregatesModel.PersonAggregate;

namespace Example.IdentityServer.Infrastructure.EntityConfigurations;

/// <summary>
/// The implementation of entity framework configuration for <see cref="PersonLanguage"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class PersonLanguageConfiguration : IEntityTypeConfiguration<PersonLanguage>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<PersonLanguage> builder)
    {
        builder.ToTable("personLanguages", IdentityServerContext.DefaultSchema);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(e => e.Name)
            .HasMaxLength(200)
            .IsRequired();
    }
}