using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IronLedgerLib.Services.Tests;

[TestClass]
public class IronLedgerServiceExtensionsTests
{
    // --- AddIronLedgerClient ---

    [TestMethod]
    public void AddIronLedgerClient_ReturnsServiceCollection()
    {
        var services = new ServiceCollection();

        var result = services.AddIronLedgerClient(o => o.ServerUrl = new Uri("https://myserver:5037"));

        Assert.AreSame(services, result);
    }

    [TestMethod]
    public void AddIronLedgerClient_NullServices_Throws()
        => Assert.ThrowsExactly<ArgumentNullException>(() =>
            ((IServiceCollection)null!).AddIronLedgerClient(o => o.ServerUrl = new Uri("https://myserver")));

    [TestMethod]
    public void AddIronLedgerClient_NullConfigure_Throws()
        => Assert.ThrowsExactly<ArgumentNullException>(() =>
            new ServiceCollection().AddIronLedgerClient(null!));

    [TestMethod]
    public void AddIronLedgerClient_NullServerUrl_Throws()
        => Assert.ThrowsExactly<ArgumentNullException>(() =>
            new ServiceCollection().AddIronLedgerClient(o => { /* ServerUrl left null */ }));

    [TestMethod]
    public void AddIronLedgerClient_RegistersIIronLedgerClient()
    {
        var services = new ServiceCollection();
        services.AddIronLedgerClient(o => o.ServerUrl = new Uri("https://myserver:5037"));

        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IIronLedgerClient));

        Assert.IsNotNull(descriptor);
    }

    [TestMethod]
    public void AddIronLedgerClient_IIronLedgerClientIsSingleton()
    {
        var services = new ServiceCollection();
        services.AddIronLedgerClient(o => o.ServerUrl = new Uri("https://myserver:5037"));

        var descriptor = services.Single(d => d.ServiceType == typeof(IIronLedgerClient));

        Assert.AreEqual(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [TestMethod]
    public void AddIronLedgerClient_ConfiguresHttpClientBaseAddress()
    {
        var services = new ServiceCollection();
        var expectedBase = new Uri("https://myserver:5037/");
        services.AddIronLedgerClient(o => o.ServerUrl = expectedBase);

        var factory = services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>();
        var httpClient = factory.CreateClient(nameof(IIronLedgerClient));

        Assert.AreEqual(expectedBase, httpClient.BaseAddress);
    }

    [TestMethod]
    public void AddIronLedgerClient_EnsuresTrailingSlashOnBaseAddress()
    {
        var services = new ServiceCollection();
        services.AddIronLedgerClient(o => o.ServerUrl = new Uri("https://myserver:5037"));

        var factory = services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>();
        var httpClient = factory.CreateClient(nameof(IIronLedgerClient));

        Assert.IsTrue(httpClient.BaseAddress!.ToString().EndsWith('/'));
    }

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

    [TestMethod]
    public void UseIronLedgerService_RegistersGetComponentsEndpoint()
    {
        var app = BuildApp();
        app.UseIronLedgerExceptionHandler();
        app.UseIronLedgerService();

        Assert.IsTrue(HasRoute(app, "GET", "api/v1/assets/{assetId}/components"));
    }

    [TestMethod]
    public void UseIronLedgerService_RegistersPatchComponentsEndpoint()
    {
        var app = BuildApp();
        app.UseIronLedgerExceptionHandler();
        app.UseIronLedgerService();

        Assert.IsTrue(HasRoute(app, "PATCH", "api/v1/assets/{assetId}/components"));
    }

    [TestMethod]
    public void UseIronLedgerService_RegistersGetNotesEndpoint()
    {
        var app = BuildApp();
        app.UseIronLedgerExceptionHandler();
        app.UseIronLedgerService();

        Assert.IsTrue(HasRoute(app, "GET", "api/v1/assets/{assetId}/notes"));
    }

    [TestMethod]
    public void UseIronLedgerService_RegistersPatchNotesEndpoint()
    {
        var app = BuildApp();
        app.UseIronLedgerExceptionHandler();
        app.UseIronLedgerService();

        Assert.IsTrue(HasRoute(app, "PATCH", "api/v1/assets/{assetId}/notes"));
    }

    private static WebApplication BuildApp()
    {
        var builder = WebApplication.CreateSlimBuilder();
        builder.Logging.ClearProviders();
        builder.Services.AddIronLedgerService();
        return builder.Build();
    }

    private static bool HasRoute(WebApplication app, string httpMethod, string routePattern)
    {
        var normalizedPattern = routePattern.TrimStart('/');
        return ((IEndpointRouteBuilder)app).DataSources
            .SelectMany(ds => ds.Endpoints)
            .OfType<RouteEndpoint>()
            .Any(e =>
                string.Equals(e.RoutePattern.RawText!.TrimStart('/'), normalizedPattern, StringComparison.OrdinalIgnoreCase) &&
                (e.Metadata.GetMetadata<IHttpMethodMetadata>()?.HttpMethods.Contains(httpMethod, StringComparer.OrdinalIgnoreCase) == true));
    }
}
