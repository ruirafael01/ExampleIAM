using Visma.IdentityServer.Domain.Exceptions;
using Visma.IdentityServer.Domain.SeedWork;

namespace Visma.IdentityServer.Domain.AggregatesModel.PersonAggregate;

/// <summary>
/// Person entity.
/// </summary>
public sealed class Person : Entity, IAggregateRoot
{
    /// <summary>
    /// Empty constructor used by Entity Framework
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Person()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Person"/> class.
    /// </summary>
    public Person(PersonPassword personPassword,
                  PersonName personName,
                  PersonRole personRole,
                  PersonLanguage personLanguage,
                  PersonEmail personEmail)
    {
        Password = personPassword ?? throw new PersonException($"{nameof(personPassword)} cannot be null.");
        Role = personRole ?? throw new PersonException($"{nameof(personRole)} cannot be null.");
        Language = personLanguage ?? throw new PersonException($"{nameof(personLanguage)} cannot be null.");
        Name = personName ?? throw new PersonException($"{nameof(personName)} cannot be null.");
        Email = personEmail ?? throw new PersonException($"{nameof(personEmail)} cannot be null.");
        CanViewAccountingRecords = EnforceAccountingViewingRecords();
    }

    /// <summary>
    /// The name.
    /// </summary>
    public PersonName Name { get; set; }

    /// <summary>
    /// The password.
    /// </summary>
    public PersonPassword Password { get; set; }

    /// <summary>
    /// The role.
    /// </summary>
    public PersonRole Role { get; private set; }

    /// <summary>
    /// The person language.
    /// </summary>
    public PersonLanguage Language { get; private set; }

    /// <summary>
    /// The person e-mail.
    /// </summary>
    public PersonEmail Email { get; private set; }

    /// <summary>
    /// The identifier if the current person can view accounting records.
    /// </summary>
    public bool CanViewAccountingRecords { get; private set; }

    /// <summary>
    /// Changes the person personName.
    /// </summary>
    /// <param personName="personName">The person personName.</param>
    public void ChangeName(PersonName personName)
    {
        Name = personName ?? throw new PersonException($"{nameof(personName)} cannot be null.");
    }

    /// <summary>
    /// Changes the person e-mail.
    /// </summary>
    /// <param personName="name">The person e-mail.</param>
    public void ChangeEmail(PersonEmail personEmail)
    {
        Email = personEmail ?? throw new PersonException($"{nameof(personEmail)} cannot be null.");
    }

    /// <summary>
    /// Changes the person language.
    /// </summary>
    /// <param personName="personLanguage">The person language.</param>
    public void ChangeLanguage(PersonLanguage personLanguage)
    {
        Language = personLanguage ?? throw new PersonException($"{nameof(personLanguage)} cannot be null.");
    }

    /// <summary>
    /// Changes the person language.
    /// </summary>
    /// <param personName="personLanguage">The person language.</param>
    public void ChangeRole(PersonRole personRole)
    {
        Role = personRole ?? throw new PersonException($"{nameof(personRole)} cannot be null.");

        CanViewAccountingRecords = EnforceAccountingViewingRecords();
    }

    private bool EnforceAccountingViewingRecords()
        => Role.Equals(PersonRole.Standard) ? false : true;
}