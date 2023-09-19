using System.Net.Mail;
using Visma.IdentityServer.Domain.Exceptions;
using Visma.IdentityServer.Domain.SeedWork;

namespace Visma.IdentityServer.Domain.AggregatesModel.PersonAggregate;

/// <summary>
/// Person email value object.
/// </summary>
public sealed class PersonEmail : ValueObject
{
    /// <summary>
    /// Empty constructor used by Entity Framework
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private PersonEmail()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PersonEmail"/>
    /// </summary>
    public PersonEmail(string email)
    {
        Value = ValidateEmail(email);
    }

    /// <summary>
    /// The concrete value of the e-mail(e.g myemail@hotmail.com)
    /// </summary>
    public string Value { get; }


    /// <inheritdoc />
    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    private static string ValidateEmail(string email)
    {
        if (MailAddress.TryCreate(email, out _))
            return email;

        throw new PersonException("Email does not match acceptable email formation.");
    }
}
