using Microsoft.EntityFrameworkCore;
using System.Net;
using Example.IdentityServer.Domain.AggregatesModel.PersonAggregate;
using Example.IdentityServer.Domain.Models;
using Example.IdentityServer.Domain.SeedWork;
using Example.IdentityServer.Infrastructure.EntityConfigurations;
using Example.IdentityServer.Infrastructure.Extensions;
using Example.IdentityServer.Infrastructure.Services;

namespace Example.IdentityServer.Infrastructure;

/// <summary>
/// The implementation of the Identity Server database context.
/// </summary>
public sealed class IdentityServerContext : DbContext, IUnitOfWork
{
    #region Fields

    public const string DefaultSchema = "ids";

    private readonly IDateTimeProvider _dateTimeProvider;

    #endregion

    #region Properties

    public DbSet<Person> Persons { get; set; }

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityServerContext"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="dateTimeProvider">The date/time provider.</param>
    public IdentityServerContext(DbContextOptions<IdentityServerContext> options,
                      IDateTimeProvider dateTimeProvider)
        : base(options)
    {
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
    }

    /// <inheritdoc />
    public async Task<DomainResponse> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            UpdateDateModified();

            await SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            return DomainResponse.Failed((int)HttpStatusCode.Conflict, exception);
        }
        catch (Exception exception)
        {
            return DomainResponse.Failed((int)HttpStatusCode.InternalServerError, exception);
        }

        return DomainResponse.Success;
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PersonConfiguration());
        modelBuilder.ApplyConfiguration(new PersonLanguageConfiguration());
        modelBuilder.ApplyConfiguration(new PersonRoleConfiguration());

        // add default data
        modelBuilder.SeedDefaultData();
    }

    private void UpdateDateModified()
    {
        var changeSet = ChangeTracker.Entries<Entity>();
        if (changeSet == null)
            return;

        var dt = _dateTimeProvider.DateTimeOffsetUtcNow;

        foreach (var entry in changeSet.Where(c => c.State == EntityState.Modified))
        {
            entry.Entity.ModificationDate = dt;
        }
    }
}