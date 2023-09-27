using Microsoft.Extensions.Hosting;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace Example.Configurations.Logging;

[ExcludeFromCodeCoverage]
public static class ConfigureLogging
{
    /// <summary>
    /// Uses Serilog as logger and configures it.
    /// </summary>
    /// <param name="hostBuilder">The host builder.</param>
    /// <returns>The updated host builder.</returns>
    public static IHostBuilder UseSerilog(this IHostBuilder hostBuilder)
    {
        return hostBuilder.UseSerilog((hostContext, config) =>
        {
            const string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3} {Message:lj}{NewLine}{Exception}";
            config.WriteTo.Console(outputTemplate: outputTemplate);
        });
    }
}
