namespace Visma.IdentityServer.Domain.DomainOperations;

/// <summary>
/// The cryptographic domain operations.
/// </summary>
public interface ICryptographer
{
    /// <summary>
    /// Hashes the provided input.
    /// </summary>
    /// <param name="input">The input to be hashed.</param>
    /// <returns>The hashed version of the input.</returns>
    string Hash(string input);

    /// <summary>
    /// Validates if an hash is equal to provided plain text.
    /// </summary>
    /// <param name="hashedContent">The hashed content.</param>
    /// <param name="plainText">The plain text content for comparison.</param>
    /// <returns>True if it's a match and False if not.</returns>
    bool ValidateHash(string hashedContent, string plainText);
}
