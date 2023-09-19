using IdentityModel;
using System.Security.Claims;
using Visma.IdentityServer.Domain.AggregatesModel.PersonAggregate;

namespace Visma.IdentityServer.Extensions;

/// <summary>
/// Extension methods for <see cref="Person"/>.
/// </summary>
internal static class PersonExtensions
{
    /// <summary>
    /// Converts the <see cref="Person"/> to it's corresponding list of claims.
    /// </summary>
    /// <param name="person">The person to be converted.</param>
    /// <returns>THe list of claims.</returns>
    public static IReadOnlyList<Claim> AsClaims(this Person person)
    {
        _ = person ?? throw new ArgumentNullException(nameof(person));

        return new List<Claim>()
        {
            new Claim(JwtClaimTypes.Subject, person.Id.ToString()),
            new Claim(JwtClaimTypes.Name, person.Name.ToString()),
            new Claim(JwtClaimTypes.Email, person.Email.Value),
            new Claim(JwtClaimTypes.Role, person.Role.ToString()),
            new Claim("canviewaccountingrecords", person.CanViewAccountingRecords.ToString())
        };
    }
}