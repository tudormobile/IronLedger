using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Tudormobile.IronLedgerLib.Serialization;

namespace IronLedgerLib.Tests;

[TestClass]
public class IronLedgerServiceCollectionExtensionsTests
{
    [TestMethod]
    public void AddIronLedger_ReturnsServiceCollection()
    {
        var services = new ServiceCollection();

        var result = services.AddIronLedger();

        Assert.AreSame(services, result);
    }

    [TestMethod]
    public void AddIronLedger_RegistersDefaultSerializer()
    {
        var services = new ServiceCollection();
        services.AddIronLedger();

        var serializer = services.BuildServiceProvider().GetService<IIronLedgerSerializer>();

        Assert.IsNotNull(serializer);
        Assert.IsInstanceOfType<IronLedgerJsonSerializer>(serializer);
    }

    [TestMethod]
    public void AddIronLedger_RegistersAssetIdFactory()
    {
        var services = new ServiceCollection();
        services.AddIronLedger();

        var factory = services.BuildServiceProvider().GetService<IAssetIdFactory>();

        Assert.IsNotNull(factory);
    }

    [TestMethod]
    public void AddIronLedger_RegistersComponentDataFactory()
    {
        var services = new ServiceCollection();
        services.AddIronLedger();

        var factory = services.BuildServiceProvider().GetService<IComponentDataFactory>();

        Assert.IsNotNull(factory);
    }

    [TestMethod]
    public void AddIronLedger_SerializerIsRegisteredAsSingleton()
    {
        var provider = new ServiceCollection().AddIronLedger().BuildServiceProvider();

        var first = provider.GetRequiredService<IIronLedgerSerializer>();
        var second = provider.GetRequiredService<IIronLedgerSerializer>();

        Assert.AreSame(first, second);
    }

    [TestMethod]
    public void AddIronLedger_AssetIdFactoryIsRegisteredAsSingleton()
    {
        var provider = new ServiceCollection().AddIronLedger().BuildServiceProvider();

        var first = provider.GetRequiredService<IAssetIdFactory>();
        var second = provider.GetRequiredService<IAssetIdFactory>();

        Assert.AreSame(first, second);
    }

    [TestMethod]
    public void AddIronLedger_ComponentDataFactoryIsRegisteredAsSingleton()
    {
        var provider = new ServiceCollection().AddIronLedger().BuildServiceProvider();

        var first = provider.GetRequiredService<IComponentDataFactory>();
        var second = provider.GetRequiredService<IComponentDataFactory>();

        Assert.AreSame(first, second);
    }

    [TestMethod]
    public void AddIronLedger_WithCustomSerializer_RegistersCustomSerializer()
    {
        var customSerializer = new MockSerializer();
        var services = new ServiceCollection();
        services.AddIronLedger(options => options.Serializer = customSerializer);

        var serializer = services.BuildServiceProvider().GetRequiredService<IIronLedgerSerializer>();

        Assert.AreSame(customSerializer, serializer);
    }

    [TestMethod]
    public void AddIronLedger_WithCustomProcessorProvider_UsesProvidedProvider()
    {
        var mockProcessor = new MockComponentDataProvider("CPU-CUSTOM");
        var services = new ServiceCollection();
        services.AddIronLedger(options => options.ProcessorProvider = mockProcessor);

        var factory = services.BuildServiceProvider().GetRequiredService<IComponentDataFactory>();
        var processors = factory.GetProcessors();

        Assert.HasCount(1, processors);
        Assert.AreEqual("CPU-CUSTOM", processors[0].Metadata.SerialNumber);
    }

    [TestMethod]
    public void AddIronLedger_WithCustomMetadataProviders_UsesProvidedProviders()
    {
        var services = new ServiceCollection();
        services.AddIronLedger(options =>
        {
            options.SystemMetadataProvider = new MockMetadataProvider("SYS-CUSTOM");
            options.BaseboardMetadataProvider = new MockMetadataProvider("BB-CUSTOM");
            options.BiosMetadataProvider = new MockMetadataProvider("BIOS-CUSTOM");
        });

        var factory = services.BuildServiceProvider().GetRequiredService<IAssetIdFactory>();
        var assetId = factory.Create();

        Assert.AreEqual("SYS-CUSTOM", assetId.SystemMetadata.SerialNumber);
        Assert.AreEqual("BB-CUSTOM", assetId.BaseBoardMetadata.SerialNumber);
        Assert.AreEqual("BIOS-CUSTOM", assetId.BiosMetadata.SerialNumber);
    }

    [TestMethod]
    public void AddIronLedger_RegistersIAssetRepository()
    {
        var services = new ServiceCollection();
        services.AddIronLedger();

        var repository = services.BuildServiceProvider().GetService<IAssetRepository>();

        Assert.IsNotNull(repository);
        Assert.IsInstanceOfType<FileSystemAssetRepository>(repository);
    }

    [TestMethod]
    public void AddIronLedger_IAssetRepositoryIsRegisteredAsSingleton()
    {
        var provider = new ServiceCollection().AddIronLedger().BuildServiceProvider();

        var first = provider.GetRequiredService<IAssetRepository>();
        var second = provider.GetRequiredService<IAssetRepository>();

        Assert.AreSame(first, second);
    }

    [TestMethod]
    public void AddIronLedger_PreRegisteredRepository_IsNotOverwritten()
    {
        var custom = new MockRepository();
        var services = new ServiceCollection();
        services.AddSingleton<IAssetRepository>(custom);
        services.AddIronLedger();

        var resolved = services.BuildServiceProvider().GetRequiredService<IAssetRepository>();

        Assert.AreSame(custom, resolved);
    }

    [ExcludeFromCodeCoverage]
    private class MockRepository : IAssetRepository
    {
        public Task<IReadOnlyList<string>> GetAllIdentifiersAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<string>>([]);
        public Task<IReadOnlyList<AssetRecord>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<AssetRecord>>([]);
        public Task<AssetRecord?> GetAsync(string assetId, CancellationToken cancellationToken = default) => Task.FromResult<AssetRecord?>(null);
        public Task<bool> Exists(AssetRecord asset, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task SaveAsync(AssetRecord asset, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task DeleteAsync(string assetId, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<string> GetNotesAsync(string assetId, CancellationToken cancellationToken = default) => Task.FromResult(string.Empty);
        public Task SaveNotesAsync(string assetId, string markdown, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    [ExcludeFromCodeCoverage]
    private class MockSerializer : IIronLedgerSerializer
    {
        public string ContentType => "application/json";
        public string Serialize<T>(T value) => string.Empty;
        public T? Deserialize<T>(string data) => default;
    }

    [ExcludeFromCodeCoverage]
    private class MockComponentDataProvider : IComponentDataProvider
    {
        private readonly IReadOnlyList<ComponentData> _data;

        public MockComponentDataProvider(string serialNumber)
        {
            _data =
            [
                new ComponentData
                {
                    Metadata = new AssetMetadata
                    {
                        SerialNumber = serialNumber,
                        Manufacturer = string.Empty,
                        Product = string.Empty
                    },
                    Caption = string.Empty,
                    Properties = []
                }
            ];
        }

        public IReadOnlyList<ComponentData> GetData() => _data;
    }

    [ExcludeFromCodeCoverage]
    private class MockMetadataProvider : IAssetMetadataProvider
    {
        private readonly string _serialNumber;

        public MockMetadataProvider(string serialNumber) => _serialNumber = serialNumber;

        public AssetMetadata GetMetadata() => new()
        {
            SerialNumber = _serialNumber,
            Manufacturer = string.Empty,
            Product = string.Empty
        };
    }
}
