using Visma.IdentityServer.Domain.Exceptions;
using Visma.IdentityServer.Domain.SeedWork;

namespace Visma.IdentityServer.Domain.AggregatesModel.PersonAggregate;

/// <summary>
/// Person language enumeration.
/// </summary>
public sealed class PersonLanguage : Enumeration
{
    /// <summary>
    /// Default person language.
    /// </summary>
    public static readonly PersonLanguage Default = new(1, nameof(EN).ToLowerInvariant());

    /// <summary>
    /// English language.
    /// </summary>
    public static readonly PersonLanguage EN = new(1, nameof(EN).ToLowerInvariant());

    /// <summary>
    /// Danish language.
    /// </summary>
    public static readonly PersonLanguage DK = new(2, nameof(DK).ToLowerInvariant());

    /// <summary>
    /// Empty constructor used by Entity Framework
    /// </summary>
    private PersonLanguage()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PersonLanguage"/> enumeration.
    /// </summary>
    public PersonLanguage(int id, string name) : base(id, name)
    {
    }

    /// <summary>
    /// Get pre-defined person languages.
    /// </summary>
    /// <returns>A list of predefined person languages.</returns>
    public static IEnumerable<PersonLanguage> List() => new[] { EN, DK };

    /// <summary>
    /// Get person language from name.
    /// </summary>
    /// <param name="name">The person language name.</param>
    /// <returns>A person language instance.</returns>
    public static PersonLanguage FromName(string name)
    {
        var personLanguage = List()
            .SingleOrDefault(e => string.Equals(e.Name, name, StringComparison.CurrentCultureIgnoreCase));

        if (personLanguage is null)
        {
            var message = $"Possible values for Language: {string.Join(",", List().Select(e => e.Name))}";
            throw new PersonException(message);
        }

        return personLanguage;
    }

    /// <summary>
    /// Get person language from id.
    /// </summary>
    /// <param name="id">The person language id.</param>
    /// <returns>A person language instance.</returns>
    public static PersonLanguage From(int id)
    {
        var personLanguage = List().SingleOrDefault(e => e.Id == id);

        if (personLanguage is null)
        {
            var message = $"Possible values for Language: {string.Join(",", List().Select(e => e.Name))}";
            throw new PersonException(message);
        }

        return personLanguage;
    }
}