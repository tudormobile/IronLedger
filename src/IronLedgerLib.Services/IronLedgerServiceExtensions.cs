namespace Tudormobile.IronLedgerLib.Services;

/// <summary>
/// Provides extension methods for registering and configuring IronLedger client and service components with dependency
/// injection and for mapping IronLedger service endpoints to a web application.
/// </summary>
/// <remarks>These extension methods simplify the integration of IronLedger functionality into ASP.NET Core
/// applications by handling service registration and endpoint mapping. Use these methods during application startup to
/// ensure that IronLedger services and endpoints are properly configured.</remarks>
public static class IronLedgerServiceExtensions
{
    /// <summary>
    /// Adds the IronLedger HTTP client to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <remarks>
    /// Use this method in client-only applications (console, WPF, MAUI, Blazor) that communicate
    /// with a remote IronLedger service. For applications that host the service, use
    /// <see cref="AddIronLedgerService"/> instead.
    /// </remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configure">Action to configure <see cref="IronLedgerClientOptions"/>. Must set <see cref="IronLedgerClientOptions.ServerUrl"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddIronLedgerClient(
        this IServiceCollection services,
        Action<IronLedgerClientOptions> configure)
    {
        System.ArgumentNullException.ThrowIfNull(services);
        System.ArgumentNullException.ThrowIfNull(configure);

        var options = new IronLedgerClientOptions();
        configure(options);
        System.ArgumentNullException.ThrowIfNull(options.ServerUrl, nameof(options.ServerUrl));

        // Ensure the base address always has a trailing slash so that relative
        // endpoint paths (e.g. "api/v1/status") resolve correctly against any
        // path segment that may be present in the server URL.
        var baseAddress = new Uri(options.ServerUrl.ToString().TrimEnd('/') + "/");

        services.AddHttpClient(nameof(IIronLedgerClient),
            client => client.BaseAddress = baseAddress);

        services.AddSingleton<IIronLedgerClient>(sp =>
            IIronLedgerClient.Create(
                sp.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(IIronLedgerClient)),
                sp.GetService<ILogger<IronLedgerClient>>(),
                sp.GetService<IIronLedgerSerializer>()));

        return services;
    }

    /// <summary>
    /// Adds the IronLedger client and service to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configure">Optional action to configure the IronLedgerClient.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddIronLedgerService(
        this IServiceCollection services,
        Action<IronLedgerOptions>? configure = null)
    {
        System.ArgumentNullException.ThrowIfNull(services);

        services.AddHttpContextAccessor();
        services.AddIronLedger(configure);
        services.AddScoped<IIronLedgerService, IronLedgerService>();
        services.AddExceptionHandler<IronLedgerExceptionHandler>();
        services.AddProblemDetails();
        return services;
    }

    /// <summary>
    /// Adds the IronLedger exception handling middleware to the web application's request pipeline.
    /// </summary>
    /// <remarks>Call this method before other middleware that may throw exceptions, typically as one of the
    /// first entries in the pipeline. Requires <see cref="AddIronLedgerService"/> to have been called during
    /// service registration.</remarks>
    /// <param name="app">The web application instance to which the middleware is added.</param>
    /// <returns>The web application instance with the exception handler middleware added.</returns>
    public static WebApplication UseIronLedgerExceptionHandler(this WebApplication app)
    {
        System.ArgumentNullException.ThrowIfNull(app);

        app.UseExceptionHandler();
        return app;
    }

    /// <summary>
    /// Maps the Iron Ledger service endpoints to the specified web application using the provided URL prefix.
    /// </summary>
    /// <remarks>This method maps HTTP routes for Iron Ledger endpoints. Call
    /// <see cref="UseIronLedgerExceptionHandler"/> separately to add exception handling middleware to the
    /// pipeline.</remarks>
    /// <param name="app">The web application instance to which the service endpoints are added.</param>
    /// <param name="prefix">The URL prefix used to namespace the service endpoints. Must be a valid URL segment if it is provided.</param>
    /// <returns>The web application instance with the service endpoints mapped.</returns>
    public static WebApplication UseIronLedgerService(this WebApplication app, string prefix = "")
    {
        System.ArgumentNullException.ThrowIfNull(app);

        prefix = prefix.TrimEnd('/');

        // Administrative Endpoints
        app.MapGet($"{prefix}/api/v1/status", async Task<IResult> (
            IIronLedgerService ironLedgerService, CancellationToken cancellationToken)
            => await ironLedgerService.GetStatusAsync(cancellationToken));

        // Asset Injest and Retrieval
        app.MapPost($"{prefix}/api/v1/assets/injest", async Task<IResult> (
            HttpRequest request, IIronLedgerService ironLedgerService, CancellationToken cancellationToken)
            => await ironLedgerService.InjestAssetAsync(request.Body, cancellationToken));

        app.MapGet($"{prefix}/api/v1/assets/{{assetId?}}", async Task<IResult> (
            IIronLedgerService ironLedgerService, string? assetId, CancellationToken cancellationToken)
            => await ironLedgerService.GetAssetAsync(assetId, cancellationToken));

        app.Logger.LogInformation("{ServiceName} is running", nameof(IronLedgerService));
        return app;
    }
}
