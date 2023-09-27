using Example.IdentityServer.Domain.Exceptions;
using Example.IdentityServer.Domain.SeedWork;

namespace Example.IdentityServer.Domain.AggregatesModel.PersonAggregate;

/// <summary>
/// Person name value object.
/// </summary>
public sealed class PersonName : ValueObject
{
    private const uint _firstNameMaxLength = 40;
    private const uint _lastNameMaxLength = 80;

    /// <summary>
    /// Empty constructor used by Entity Framework
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private PersonName()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PersonName"/> class.
    /// </summary>
    public PersonName(string firstName, string lastName)
    {
        FirstName = ValidateFirstName(firstName);
        LastName = ValidateLastName(lastName);
    }

    /// <summary>
    /// The person's first name.
    /// </summary>
    public string FirstName { get; private set; }

    /// <summary>
    /// The person's last name.
    /// </summary>
    public string LastName { get; private set; }

    /// <inheritdoc />
    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return FirstName;
        yield return LastName;
    }

    /// <inheritdoc />
    public override string ToString()
        => $"{FirstName} {LastName}";

    private static string ValidateFirstName(string firstName)
    {
        if(string.IsNullOrWhiteSpace(firstName))
            throw new PersonException("First name cannot be null or empty.");

        if (firstName.Length > _firstNameMaxLength)
            throw new PersonException($"The maximum length for first name is {_firstNameMaxLength} characters.");

        return firstName;
    }
    
    private static string ValidateLastName(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
            throw new PersonException("Last name cannot be null or empty.");

        if (lastName.Length > _lastNameMaxLength)
            throw new PersonException($"The maximum length for last name is {_lastNameMaxLength} characters.");

        return lastName;
    }
}
