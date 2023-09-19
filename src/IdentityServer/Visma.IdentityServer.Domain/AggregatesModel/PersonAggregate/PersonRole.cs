using Visma.IdentityServer.Domain.Exceptions;
using Visma.IdentityServer.Domain.SeedWork;

namespace Visma.IdentityServer.Domain.AggregatesModel.PersonAggregate;

/// <summary>
/// Person role enumeration.
/// </summary>
public sealed class PersonRole : Enumeration
{
    /// <summary>
    /// Standard.
    /// </summary>
    public static readonly PersonRole Standard = new(1, nameof(Standard).ToLowerInvariant());

    /// <summary>
    /// Accountant.
    /// </summary>
    public static readonly PersonRole Accountant = new(2, nameof(Accountant).ToLowerInvariant());

    /// <summary>
    /// Administrator.
    /// </summary>
    public static readonly PersonRole Administrator = new(3, nameof(Administrator).ToLowerInvariant());

    /// <summary>
    /// Empty constructor used by Entity Framework
    /// </summary>
    private PersonRole()
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PersonRole"/> enumeration.
    /// </summary>
    public PersonRole(int id, string name) : base(id, name)
    {

    }

    /// <summary>
    /// Get pre-defined person role types.
    /// </summary>
    /// <returns>A list of predefined person role types.</returns>
    public static IEnumerable<PersonRole> List() => new[] { Standard, Accountant, Administrator };

    /// <summary>
    /// Get person role type from name.
    /// </summary>
    /// <param name="name">The person role type name.</param>
    /// <returns>A person role type instance.</returns>
    public static PersonRole FromName(string name)
    {
        var personRole = List()
            .SingleOrDefault(e => string.Equals(e.Name, name, StringComparison.CurrentCultureIgnoreCase));

        if (personRole is null)
        {
            var message = $"Possible values for PersonRole: {string.Join(",", List().Select(e => e.Name))}";
            throw new PersonException(message);
        }

        return personRole;
    }

    /// <summary>
    /// Get person role type from id.
    /// </summary>
    /// <param name="id">The person role type id.</param>
    /// <returns>A person role type instance.</returns>
    public static PersonRole From(int id)
    {
        var personRole = List().SingleOrDefault(e => e.Id == id);

        if (personRole is null)
        {
            var message = $"Possible values for PersonRole: {string.Join(",", List().Select(e => e.Name))}";
            throw new PersonException(message);
        }

        return personRole;
    }
}
