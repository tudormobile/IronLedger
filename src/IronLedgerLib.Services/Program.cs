namespace Tudormobile.IronLedgerLib.Services;

internal class Program
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
        app.UseIronLedgerService(prefix: string.Empty);


        app.Run();
    }
}
