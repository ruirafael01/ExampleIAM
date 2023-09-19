using Microsoft.EntityFrameworkCore;
using Visma.IdentityServer.Domain.AggregatesModel.PersonAggregate;
using Visma.IdentityServer.Domain.SeedWork;

namespace Visma.IdentityServer.Infrastructure.Repositories;

/// <summary>
/// Implements the <see cref="IPersonRepository"/>
/// </summary>
public sealed class PersonRepository : IPersonRepository
{
    private readonly IdentityServerContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="PersonRepository"/> class.
    /// </summary>
    /// <param name="context">The database context</param>
    public PersonRepository(IdentityServerContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public IQueryable<Person> AllPersons => _context.Persons;

    /// <inheritdoc />
    public IUnitOfWork UnitOfWork => _context;

    /// <inheritdoc />
    public async Task<Person> AddAsync(Person person, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(person);

        await SetLanguageAsync(person, cancellationToken);
        await SetRoleAsync(person, cancellationToken);

        var entry = _context.Persons.Add(person);
        return entry.Entity;
    }

    /// <inheritdoc />
    public async Task AddManyAsync(IEnumerable<Person> persons, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(persons);

        foreach (var person in persons)
        {
            await AddAsync(person, cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, byte[] rowVersion, CancellationToken cancellationToken)
    {
        var person = await _context.Persons.FindAsync(new object[] { id }, cancellationToken);
        if (person is not null)
        {
            _context.Entry(person).OriginalValues[nameof(Person.RowVersion)] = rowVersion;
            _context.Persons.Remove(person);
        }
    }

    /// <inheritdoc />
    public Task<Person?> GetAsync(Guid id, CancellationToken cancellationToken) 
        => AllPersons
            .AsNoTracking()
            .Include(p => p.Name)
            .Include(p => p.Language)
            .Include(p => p.Email)
            .Include(p => p.Password)
            .Include(p => p.Role)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    /// <inheritdoc />
    public Task<Person?> GetByEmailAsync(string email, CancellationToken cancellationToken)
         => AllPersons
            .AsNoTracking()
            .Include(p => p.Name)
            .Include(p => p.Language)
            .Include(p => p.Email)
            .Include(p => p.Password)
            .Include(p => p.Role)
            .FirstOrDefaultAsync(p => p.Email.Value.Equals(email), cancellationToken);

    /// <inheritdoc />
    public async Task UpdateAsync(Person person, byte[] rowVersion, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(person);

        var dbPerson = await _context.Persons
            .Include(p => p.Name)
            .Include(p => p.Language)
            .Include(p => p.Email)
            .FirstOrDefaultAsync(p => p.Id == person.Id, cancellationToken);

        if (dbPerson is not null)
        {
            dbPerson.ChangeRole(person.Role);
            dbPerson.ChangeLanguage(person.Language);
            dbPerson.ChangeEmail(person.Email);
            dbPerson.ChangeName(person.Name);

            _context.Entry(dbPerson).State = EntityState.Modified;
            _context.Entry(dbPerson).OriginalValues[nameof(Person.RowVersion)] = rowVersion;
        }
    }

    #region Privates

    private async Task SetLanguageAsync(Person targetEntity, CancellationToken cancellationToken)
    {
        if (targetEntity.Language is not null && _context.Entry(targetEntity.Language).State.Equals(EntityState.Detached))
        {
            var language = await _context.Set<PersonLanguage>().FindAsync(new object[] { targetEntity.Language.Id }, cancellationToken);

            if (language is not null)
                targetEntity.ChangeLanguage(language);
        }
    }

    private async Task SetRoleAsync(Person targetEntity, CancellationToken cancellationToken)
    {
        if (targetEntity.Role is not null && _context.Entry(targetEntity.Role).State.Equals(EntityState.Detached))
        {
            var role = await _context.Set<PersonRole>().FindAsync(new object[] { targetEntity.Role.Id }, cancellationToken);

            if (role is not null)
                targetEntity.ChangeRole(role);
        }
    }

    #endregion
}