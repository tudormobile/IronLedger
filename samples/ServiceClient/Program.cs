using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Tudormobile.IronLedgerLib.Services;

namespace ServiceClient;

internal class Program
{
    static async Task Main(string[] args)
    {

        var environment = Debugger.IsAttached ? "Development" : "Production";

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false)
            .Build();

        var serviceUrl = config["IronLedgerServiceUrl"] ?? "http://www.example.com";

        // Create the logger
        using var loggerFactory = LoggerFactory.Create(b => b
            .AddConsole()
            .SetMinimumLevel(LogLevel.Information)
            );
        var logger = loggerFactory.CreateLogger<Program>();
        var clientLogger = loggerFactory.CreateLogger<IIronLedgerClient>();

        logger.LogInformation("Connecting to: {ServiceUrl}", serviceUrl);

        // Create the client
        using var httpClient = new HttpClient() { BaseAddress = new Uri(serviceUrl) };
        var client = IIronLedgerClient.Create(httpClient, clientLogger);

        // Request remote service status
        logger.LogInformation("Requesting Service Status");

        var status = await client.GetStatusAsync();

        logger.LogInformation($"GetStatusAsync() Success: {status.IsSuccess}");
        if (status.IsSuccess) logger.LogInformation("GetStatusAsync() Data: {Data}", status.Data);
        else logger.LogError("GetStatusAsync() Error Message: {ErrorMessage}", status.ErrorMessage);
    }
}
