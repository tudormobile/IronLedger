using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IronLedgerLib.Services.Tests;

[TestClass]
public class IronLedgerServiceExtensionsTests
{
    // --- AddIronLedgerService ---

    [TestMethod]
    public void AddIronLedgerService_ReturnsServiceCollection()
    {
        var services = new ServiceCollection();

        var result = services.AddIronLedgerService();

        Assert.AreSame(services, result);
    }

    [TestMethod]
    public void AddIronLedgerService_RegistersIIronLedgerService()
    {
        var services = new ServiceCollection();
        services.AddIronLedgerService();

        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IIronLedgerService));

        Assert.IsNotNull(descriptor);
    }

    [TestMethod]
    public void AddIronLedgerService_IIronLedgerServiceIsScoped()
    {
        var services = new ServiceCollection();
        services.AddIronLedgerService();

        var descriptor = services.Single(d => d.ServiceType == typeof(IIronLedgerService));

        Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    // --- UseIronLedgerExceptionHandler ---

    [TestMethod]
    public void UseIronLedgerExceptionHandler_ReturnsApp()
    {
        var app = BuildApp();

        var result = app.UseIronLedgerExceptionHandler();

        Assert.AreSame(app, result);
    }

    // --- UseIronLedgerService ---

    [TestMethod]
    public void UseIronLedgerService_ReturnsApp()
    {
        var app = BuildApp();
        app.UseIronLedgerExceptionHandler();

        var result = app.UseIronLedgerService();

        Assert.AreSame(app, result);
    }

    [TestMethod]
    public void UseIronLedgerService_WithPrefix_ReturnsApp()
    {
        var app = BuildApp();
        app.UseIronLedgerExceptionHandler();

        var result = app.UseIronLedgerService(prefix: "/v1");

        Assert.AreSame(app, result);
    }

    private static WebApplication BuildApp()
    {
        var builder = WebApplication.CreateSlimBuilder();
        builder.Logging.ClearProviders();
        builder.Services.AddIronLedgerService();
        return builder.Build();
    }
}
