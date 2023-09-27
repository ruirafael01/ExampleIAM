using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;
using Example.IdentityServer.Domain.DomainOperations;

namespace Example.IdentityServer.Security;

/// <summary>
/// Implements the <see cref="ISecretValidator"/>
/// </summary>
internal sealed class CustomSecretValidator : ISecretValidator
{
    private readonly ICryptographer _cryptographer;

    public CustomSecretValidator(ICryptographer cryptographer)
    {
        _cryptographer = cryptographer ?? throw new ArgumentNullException(nameof(cryptographer));
    }

    /// <inheritdoc />
    public Task<SecretValidationResult> ValidateAsync(IEnumerable<Secret> secrets, ParsedSecret parsedSecret)
    {
        _ = secrets ?? throw new ArgumentNullException(nameof(secrets));
        _ = parsedSecret ?? throw new ArgumentNullException(nameof(parsedSecret));

        string? credential = parsedSecret.Credential as string;

        if (string.IsNullOrEmpty(credential))
            throw new ArgumentException(nameof(credential));

        foreach(var secret in secrets)
        {
           bool isValid =  _cryptographer.ValidateHash(secret.Value, credential);

            if (isValid)
                return SuccessSecretValidation();
        }

        return FailedSecretValidation();
    }

    private static Task<SecretValidationResult> SuccessSecretValidation()
        => Task.FromResult(new SecretValidationResult()
        {
            Success = true,
            IsError = false
        });

    private static Task<SecretValidationResult> FailedSecretValidation()
       => Task.FromResult(new SecretValidationResult()
       {
           Success = false,
           IsError = true,
           Error = "Client secret did not match."
       });
}
