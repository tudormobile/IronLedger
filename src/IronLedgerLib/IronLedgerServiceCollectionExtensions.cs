using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Tudormobile.IronLedgerLib;
using Tudormobile.IronLedgerLib.Serialization;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for registering IronLedger services with <see cref="IServiceCollection"/>.
/// </summary>
public static class IronLedgerServiceCollectionExtensions
{
    /// <summary>
    /// Registers IronLedger services with the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configure">An optional delegate to configure <see cref="IronLedgerOptions"/>.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for chaining.</returns>
    public static IServiceCollection AddIronLedger(
        this IServiceCollection services,
        Action<IronLedgerOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));

        var options = new IronLedgerOptions();
        configure?.Invoke(options);

        services.AddSingleton<IIronLedgerSerializer>(
            options.Serializer ?? new IronLedgerJsonSerializer());

        services.AddSingleton<IAssetIdFactory>(sp => new AssetIdFactory(
            options.SystemMetadataProvider,
            options.BaseboardMetadataProvider,
            options.BiosMetadataProvider,
            sp.GetService<ILogger<AssetIdFactory>>()));

        services.AddSingleton<IComponentDataFactory>(sp => new ComponentDataFactory(
            options.ProcessorProvider,
            options.SystemProvider,
            options.MemoryProvider,
            options.DiskProvider,
            sp.GetService<ILogger<ComponentDataFactory>>()));

        var dataPath = options.DataPath ?? Path.Combine(Directory.GetCurrentDirectory(), "data");
        services.TryAddSingleton<IAssetRepository>(sp =>
            new FileSystemAssetRepository(dataPath, sp.GetRequiredService<IIronLedgerSerializer>()));

        return services;
    }
}
