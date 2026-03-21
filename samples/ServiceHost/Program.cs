using Tudormobile.IronLedgerLib.Services;

namespace ServiceHost;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddIronLedgerService();

        // Build the application.
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseIronLedgerExceptionHandler();
        app.UseHttpsRedirection();

        // Map in the service endpoints.
        var prefix = builder.Configuration["EndpointPrefix"] ?? string.Empty;
        app.UseIronLedgerService(prefix: prefix);

        // Run the application
        app.Run();
    }
}
