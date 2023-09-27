using Example.IdentityServer.Domain.AggregatesModel.PersonAggregate;
using Example.IdentityServer.Domain.DomainOperations;
using Example.IdentityServer.Domain.Exceptions;
using Example.IdentityServer.Domain.Models;

namespace Example.IdentityServer.Domain.DomainServices;

/// <summary>
/// Implements the <see cref="IPersonService"/>
/// </summary>
public sealed class PersonService : IPersonService
{
    private readonly IPersonRepository _personRepository;
    private readonly ICryptographer _cryptographer;

    /// <summary>
    /// Initializes a new instance of the <see cref="PersonService"/>
    /// </summary>
    public PersonService(IPersonRepository personRepository, ICryptographer cryptographer)
    {
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
        _cryptographer = cryptographer ?? throw new ArgumentNullException(nameof(cryptographer));
    }


    /// <inheritdoc />
    public async Task CreatePersonAsync(string password,
                                  PersonEmail email,
                                  PersonLanguage language,
                                  PersonRole role,
                                  PersonName name,
                                  CancellationToken cancellationToken)
    {
        var personPassword = new PersonPassword(_cryptographer.Hash(password));

        var person = new Person(personPassword,
                                name,
                                role,
                                language,
                                email);

        await _personRepository.AddAsync(person, cancellationToken);

        DomainResponse response = await _personRepository
                                            .UnitOfWork
                                            .SaveEntitiesAsync(cancellationToken);

        if (response.Successful is false)
        {
            Exception exception = response.Error?.Exception is null 
                                        ? new PersonException("Error when trying to persist new person to domain.")
                                        : response.Error.Exception;
            throw exception;
        }
    }

    /// <inheritdoc />
    public Task<Person?> GetPersonByEmailAsync(PersonEmail email, CancellationToken cancellationToken)
        => _personRepository.GetByEmailAsync(email.Value, cancellationToken);

    /// <inheritdoc />
    public Task<Person?> GetPersonByIdAsync(Guid id, CancellationToken cancellationToken)
        => _personRepository.GetAsync(id, cancellationToken);

    public bool PasswordIsEqual(Person person, string inputPassword)
        => _cryptographer.ValidateHash(person.Password.Value, inputPassword);
}
