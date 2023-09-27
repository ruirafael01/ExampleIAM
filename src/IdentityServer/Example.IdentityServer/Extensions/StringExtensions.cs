using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using System.Collections.Specialized;
using System.IdentityModel.Tokens.Jwt;

namespace Example.IdentityServer.Extensions;

/// <summary>
/// Extension methods for strings.
/// </summary>
internal static class StringExtensions
{
    /// <summary>
    /// Converts the token as a string to a JwtSecurityToken.
    /// </summary>
    /// <param name="token">The raw token.</param>
    /// <returns>The JwtSecurityToken.</returns>
    public static JwtSecurityToken AsJwtSecurityToken(this string token)
    {
        if(string.IsNullOrEmpty(token))
            throw new ArgumentException(nameof(token));

        var jwtHandler = new JwtSecurityTokenHandler();

        if (!jwtHandler.CanReadToken(token))
            throw new InvalidOperationException($"Cannot read '{token}' as a JWTSecurityToken");

        return jwtHandler.ReadJwtToken(token);
    }

    /// <summary>
    /// Converts a string into a name value collection.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>The string as a name value collection.</returns>
    public static NameValueCollection ReadQueryStringAsNameValueCollection(this string input)
    {
        if (input != null)
        {
            var idx = input.IndexOf('?');
            if (idx >= 0)
            {
                input = input.Substring(idx + 1);
            }
            var query = QueryHelpers.ParseNullableQuery(input);
            if (query != null)
            {
                return query.AsNameValueCollection();
            }
        }

        return new NameValueCollection();
    }

    /// <summary>
    /// Determines if a string is a local URL or not.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <returns>True if the string is a local URL and false if not.</returns>
    public static bool IsLocalUrl(this string url)
    {
        if (string.IsNullOrEmpty(url))
             return false;

        // Allows "/" or "/foo" but not "//" or "/\".
        if (url[0] == '/')
        {
            // url is exactly "/"
            if (url.Length == 1)
                return true;

            // url doesn't start with "//" or "/\"
            if (url[1] != '/' && url[1] != '\\')
                return true;

            return false;
        }

        // Allows "~/" or "~/foo" but not "~//" or "~/\".
        if (url[0] == '~' && url.Length > 1 && url[1] == '/')
        {
            // url is exactly "~/"
            if (url.Length == 2)
                return true;

            // url doesn't start with "~//" or "~/\"
            if (url[2] != '/' && url[2] != '\\')
                return true;

            return false;
        }

        return false;
    }

    private static NameValueCollection AsNameValueCollection(this IDictionary<string, StringValues> collection)
    {
        var nv = new NameValueCollection();

        foreach (var field in collection)
        {
            nv.Add(field.Key, field.Value.First());
        }

        return nv;
    }
}