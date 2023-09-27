using Example.IdentityServer.Domain.Exceptions;
using Example.IdentityServer.Domain.SeedWork;

namespace Example.IdentityServer.Domain.AggregatesModel.PersonAggregate;

/// <summary>
/// Person password value object.
/// </summary>
public sealed class PersonPassword : ValueObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PersonPassword"/>
    /// </summary>
    public PersonPassword(string value)
    {
        if (string.IsNullOrEmpty(value))
            throw new PersonException($"Password cannot be null or empty.");

        Value = value;
    }

    /// <summary>
    /// The actual value of the password.
    /// </summary>
    public string Value { get; set; }

    /// <inheritdoc />
    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
