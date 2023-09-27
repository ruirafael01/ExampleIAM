using Example.Configurations.Helpers;
using Example.Configurations.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Example.Configurations.Hosting;

/// <summary>
/// Configure hosting.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ConfigureHost
{
    /// <summary>
    /// Configure Kestrel process.
    /// </summary>
    /// <param name="appBuilder">The web application builder</param>
    /// <returns>The updated web application builder.</returns>
    public static WebApplicationBuilder ConfigureKestrel(this WebApplicationBuilder appBuilder)
    {
        // configure HTTPS
        var configuration = appBuilder.Configuration;
        var environment = appBuilder.Environment;

        appBuilder.WebHost.ConfigureKestrel(options =>
        {
            var endpoints = configuration.GetSection("HttpServer:Endpoints")
                .GetChildren()
                .ToDictionary(section => section.Key, section =>
                {
                    var endpoint = new EndpointConfiguration();
                    section.Bind(endpoint);
                    return endpoint;
                });

            foreach (var endpoint in endpoints)
            {
                var config = endpoint.Value;
                var port = config.Port ?? (config.Scheme == "https" ? 443 : 80);

                var ipAddresses = new List<IPAddress>();
                if (config.Host == "localhost")
                {
                    ipAddresses.Add(IPAddress.IPv6Loopback);
                    ipAddresses.Add(IPAddress.Loopback);
                }
                else if (IPAddress.TryParse(config.Host, out var address))
                {
                    ipAddresses.Add(address);
                }
                else
                {
                    ipAddresses.Add(IPAddress.IPv6Any);
                }

                foreach (var address in ipAddresses)
                {
                    options.Listen(address, port,
                        listenOptions =>
                        {
                            if (config.Scheme != "https")
                                return;

                            var certificate = CertificateHelper.LoadCertificate(config, environment);
                            listenOptions.UseHttps(certificate);
                        });
                }
            }
        });

        return appBuilder;
    }
}