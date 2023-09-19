using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;
using Visma.IdentityServer.Domain.AggregatesModel.PersonAggregate;

namespace Visma.IdentityServer.Infrastructure.EntityConfigurations;

/// <summary>
/// The implementation of entity framework configuration for <see cref="Person"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("persons", IdentityServerContext.DefaultSchema);

        // primary key
        builder.HasKey(e => e.Id);

        // row version
        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        // person --> personName (one to one)
        builder.OwnsOne(e => e.Name, eb =>
        {
            eb.ToTable("personNames", IdentityServerContext.DefaultSchema);

            // foreign key back to parent person
            eb.WithOwner().HasForeignKey("PersonId");

            // first name
            eb.Property(e => e.FirstName)
                .HasMaxLength(40)
                .IsRequired();

            // last name
            eb.Property(e => e.LastName)
                .HasMaxLength(80)
                .IsRequired();
        });

        // person --> personPassword (one to one)
        builder.OwnsOne(e => e.Password, eb =>
        {
            eb.ToTable("personPasswords", IdentityServerContext.DefaultSchema);

            // foreign key back to parent person
            eb.WithOwner().HasForeignKey("PersonId");

            // first name
            eb.Property(e => e.Value)
                .HasMaxLength(500)
                .IsRequired();
        });

        // person --> personEmail (one to one)
        builder.OwnsOne(e => e.Email, eb =>
        {
            eb.ToTable("personEmails", IdentityServerContext.DefaultSchema);

            // foreign key back to parent person
            eb.WithOwner().HasForeignKey("PersonId");

            // first name
            eb.Property(e => e.Value)
                .HasMaxLength(320)
                .IsRequired();
        });

        // person --> language (one to one)
        builder.HasOne(e => e.Language)
            .WithMany()
            .HasForeignKey("PersonLanguageId")
        .IsRequired();

        // person --> role (one to one)
        builder.HasOne(e => e.Role)
            .WithMany()
            .HasForeignKey("PersonRoleId")
        .IsRequired();

        // default creation date value
        builder.Property(e => e.CreationDate)
            .HasDefaultValueSql("SYSUTCDATETIME()");
    }
}