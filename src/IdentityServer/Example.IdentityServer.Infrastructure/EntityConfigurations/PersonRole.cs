using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;
using Example.IdentityServer.Domain.AggregatesModel.PersonAggregate;

namespace Example.IdentityServer.Infrastructure.EntityConfigurations;

/// <summary>
/// The implementation of entity framework configuration for <see cref="PersonRole"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class PersonRoleConfiguration : IEntityTypeConfiguration<PersonRole>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<PersonRole> builder)
    {
        builder.ToTable("personRoles", IdentityServerContext.DefaultSchema);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(e => e.Name)
            .HasMaxLength(200)
            .IsRequired();
    }
}