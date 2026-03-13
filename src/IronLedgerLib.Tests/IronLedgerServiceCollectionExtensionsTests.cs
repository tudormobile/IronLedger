using Microsoft.Extensions.DependencyInjection;
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

    private class MockSerializer : IIronLedgerSerializer
    {
        public string Serialize<T>(T value) => string.Empty;
        public T? Deserialize<T>(string data) => default;
    }

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

    private class MockMetadataProvider : IAssetMetadataProvider
    {
        private readonly string _serialNumber;

        public MockMetadataProvider(string serialNumber) => _serialNumber = serialNumber;

        public AssetMetadata GetMetadata() => new AssetMetadata
        {
            SerialNumber = _serialNumber,
            Manufacturer = string.Empty,
            Product = string.Empty
        };
    }
}
