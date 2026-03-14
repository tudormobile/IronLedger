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
    /// Adds the IronLedger client and service to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configure">Optional action to configure the IronLedgerClient.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddIronLedgerService(
        this IServiceCollection services,
        Action<IronLedgerOptions>? configure = null)
    {
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
        prefix = prefix.TrimEnd('/');

        app.MapGet($"{prefix}/api/v1", async Task<IResult> (
            HttpContext context, IIronLedgerService ironLedgerService, CancellationToken cancellationToken)
            => await ironLedgerService.GetStatusAsync(context, cancellationToken));

        app.Logger.LogInformation("{ServiceName} is running", nameof(IronLedgerService));
        return app;
    }
}
