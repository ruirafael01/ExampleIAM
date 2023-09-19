using Microsoft.Extensions.Hosting;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using Visma.Configurations.Models;

namespace Visma.Configurations.Helpers;

/// <summary>
/// Class containing helper methods related to certificate.
/// </summary>
public static class CertificateHelper
{
    private const string NonHexRegex = @"[^\da-fA-F]";

    /// <summary>
    /// Load a certificate compatible with the specified configuration.
    /// </summary>
    /// <param name="config">The endpoint configuration.</param>
    /// <param name="environment">The hosting environment.</param>
    /// <returns>A certificate compatible with the specified configuration.</returns>
    public static X509Certificate2 LoadCertificate(EndpointConfiguration config, IHostEnvironment environment)
    {
        if (environment == null)
            throw new ArgumentNullException(nameof(environment));

        // load from certificate store, if specified
        if (!string.IsNullOrEmpty(config.StoreName) &&
            !string.IsNullOrEmpty(config.StoreLocation) &&
            !string.IsNullOrEmpty(config.Thumbprint))
        {
            // clean up thumbprint by removing non-hex characters
            var thumbprint = Regex.Replace(config.Thumbprint, NonHexRegex, string.Empty).ToUpper(CultureInfo.CurrentCulture);

            // find the certificate via its thumbprint
            using var store = new X509Store(ParseEnum<StoreName>(config.StoreName),
                ParseEnum<StoreLocation>(config.StoreLocation));
            store.Open(OpenFlags.ReadOnly);

            // find certificate
            var certificates = store.Certificates.Where(x => x.Thumbprint.Equals(thumbprint)).ToList();

            //var certificates = store.Certificates.Find(X509FindType.FindByThumbprint,
            //    thumbprint,
            //    !environment.IsDevelopment());

            if (certificates.Count == 0)
            {
                throw new InvalidOperationException($"Certificate not found for {config.Host}.");
            }

            return certificates[0];
        }

        // not loading from certificate store, maybe load from file?
        if (config.FilePath != null && config.Password != null)
        {
            if (!File.Exists(config.FilePath))
                throw new FileNotFoundException("Certificate not found", config.FilePath);

            return new X509Certificate2(config.FilePath, config.Password);
        }

        throw new InvalidOperationException("No valid certificate configuration found");
    }

    private static T ParseEnum<T>(string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }
}