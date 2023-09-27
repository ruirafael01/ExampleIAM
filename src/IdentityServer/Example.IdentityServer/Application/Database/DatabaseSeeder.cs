using Example.IdentityServer.Domain.AggregatesModel.PersonAggregate;
using Example.IdentityServer.Domain.DomainServices;

namespace Example.IdentityServer.Application.Database;

/// <summary>
/// Implements the <see cref="IDatabaseSeeder"/>
/// </summary>
internal sealed class DatabaseSeeder : IDatabaseSeeder
{
    private readonly IPersonService _personService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseSeeder"/>
    /// </summary>
    public DatabaseSeeder(IPersonService personService)
    {
        _personService = personService ?? throw new ArgumentNullException(nameof(personService));
    }


    /// <inheritdoc />
    public async Task SeedData()
    {
        var email = "ronspwan11@gmail.com";

        Person? person = await _personService.GetPersonByEmailAsync(new PersonEmail("ronspwan11@gmail.com"), CancellationToken.None);

        if (person is null)
            await _personService.CreatePersonAsync("test123",
                                                   new PersonEmail(email),
                                                   PersonLanguage.EN,
                                                   PersonRole.Administrator,
                                                   new PersonName("Rui", "Rafael"),
                                                   CancellationToken.None);
    }
}