using Microsoft.AspNetCore.Identity;
using Example.IdentityServer.Domain.DomainOperations;

namespace Example.IdentityServer.Infrastructure.Operations;

/// <summary>
/// Initializes a new instance of the <see cref="ICryptographer"/>
/// </summary>
public sealed class Cryptographer : ICryptographer
{
    private readonly IPasswordHasher<string> _hasher;

    /// <summary>
    /// Initializes a new instance of the <see cref="Cryptographer"/>
    /// </summary>
    public Cryptographer(IPasswordHasher<string> hasher)
    {
        _hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
    }

    /// <inheritdoc />
    public string Hash(string input)
    {
        if (string.IsNullOrEmpty(input))
            throw new ArgumentException(nameof(input));

        return _hasher.HashPassword(string.Empty, input);
    }

    /// <inheritdoc />
    public bool ValidateHash(string hashedContent, string plainText)
    {
        if (string.IsNullOrEmpty(hashedContent))
            throw new ArgumentException(nameof(hashedContent));

        if (string.IsNullOrEmpty(plainText))
            throw new ArgumentException(nameof(plainText));

        return _hasher
               .VerifyHashedPassword(string.Empty, hashedContent, plainText)
               .Equals(PasswordVerificationResult.Failed) ? false : true;
    }
}
