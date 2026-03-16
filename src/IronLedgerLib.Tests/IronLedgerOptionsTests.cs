using System.Diagnostics.CodeAnalysis;

namespace IronLedgerLib.Tests;

[TestClass]
public class IronLedgerOptionsTests
{
    [TestMethod]
    public void IronLedgerOptions_DefaultConstructor_AllPropertiesAreNull()
    {
        var options = new IronLedgerOptions();

        Assert.IsNull(options.Serializer);
        Assert.IsNull(options.ProcessorProvider);
        Assert.IsNull(options.SystemProvider);
        Assert.IsNull(options.MemoryProvider);
        Assert.IsNull(options.DiskProvider);
        Assert.IsNull(options.SystemMetadataProvider);
        Assert.IsNull(options.BaseboardMetadataProvider);
        Assert.IsNull(options.BiosMetadataProvider);
    }

    [TestMethod]
    public void IronLedgerOptions_Serializer_CanBeSetAndRetrieved()
    {
        var serializer = new MockSerializer();
        var options = new IronLedgerOptions { Serializer = serializer };

        Assert.AreSame(serializer, options.Serializer);
    }

    [TestMethod]
    public void IronLedgerOptions_ProcessorProvider_CanBeSetAndRetrieved()
    {
        var provider = new MockComponentDataProvider();
        var options = new IronLedgerOptions { ProcessorProvider = provider };

        Assert.AreSame(provider, options.ProcessorProvider);
    }

    [TestMethod]
    public void IronLedgerOptions_SystemProvider_CanBeSetAndRetrieved()
    {
        var provider = new MockComponentDataProvider();
        var options = new IronLedgerOptions { SystemProvider = provider };

        Assert.AreSame(provider, options.SystemProvider);
    }

    [TestMethod]
    public void IronLedgerOptions_MemoryProvider_CanBeSetAndRetrieved()
    {
        var provider = new MockComponentDataProvider();
        var options = new IronLedgerOptions { MemoryProvider = provider };

        Assert.AreSame(provider, options.MemoryProvider);
    }

    [TestMethod]
    public void IronLedgerOptions_DiskProvider_CanBeSetAndRetrieved()
    {
        var provider = new MockComponentDataProvider();
        var options = new IronLedgerOptions { DiskProvider = provider };

        Assert.AreSame(provider, options.DiskProvider);
    }

    [TestMethod]
    public void IronLedgerOptions_SystemMetadataProvider_CanBeSetAndRetrieved()
    {
        var provider = new MockMetadataProvider();
        var options = new IronLedgerOptions { SystemMetadataProvider = provider };

        Assert.AreSame(provider, options.SystemMetadataProvider);
    }

    [TestMethod]
    public void IronLedgerOptions_BaseboardMetadataProvider_CanBeSetAndRetrieved()
    {
        var provider = new MockMetadataProvider();
        var options = new IronLedgerOptions { BaseboardMetadataProvider = provider };

        Assert.AreSame(provider, options.BaseboardMetadataProvider);
    }

    [TestMethod]
    public void IronLedgerOptions_BiosMetadataProvider_CanBeSetAndRetrieved()
    {
        var provider = new MockMetadataProvider();
        var options = new IronLedgerOptions { BiosMetadataProvider = provider };

        Assert.AreSame(provider, options.BiosMetadataProvider);
    }

    [TestMethod]
    public void IronLedgerOptions_SettingOneProperty_DoesNotAffectOthers()
    {
        var provider = new MockComponentDataProvider();
        var options = new IronLedgerOptions { ProcessorProvider = provider };

        Assert.IsNull(options.Serializer);
        Assert.IsNull(options.SystemProvider);
        Assert.IsNull(options.MemoryProvider);
        Assert.IsNull(options.DiskProvider);
        Assert.IsNull(options.SystemMetadataProvider);
        Assert.IsNull(options.BaseboardMetadataProvider);
        Assert.IsNull(options.BiosMetadataProvider);
    }

    [TestMethod]
    public void IronLedgerOptions_AllPropertiesCanBeSetTogether()
    {
        var serializer = new MockSerializer();
        var processorProvider = new MockComponentDataProvider();
        var systemProvider = new MockComponentDataProvider();
        var memoryProvider = new MockComponentDataProvider();
        var diskProvider = new MockComponentDataProvider();
        var systemMetadataProvider = new MockMetadataProvider();
        var baseboardMetadataProvider = new MockMetadataProvider();
        var biosMetadataProvider = new MockMetadataProvider();
        var dataPath = @"some\path";

        var options = new IronLedgerOptions
        {
            Serializer = serializer,
            ProcessorProvider = processorProvider,
            SystemProvider = systemProvider,
            MemoryProvider = memoryProvider,
            DiskProvider = diskProvider,
            SystemMetadataProvider = systemMetadataProvider,
            BaseboardMetadataProvider = baseboardMetadataProvider,
            BiosMetadataProvider = biosMetadataProvider,
            DataPath = dataPath

        };

        Assert.AreSame(serializer, options.Serializer);
        Assert.AreSame(processorProvider, options.ProcessorProvider);
        Assert.AreSame(systemProvider, options.SystemProvider);
        Assert.AreSame(memoryProvider, options.MemoryProvider);
        Assert.AreSame(diskProvider, options.DiskProvider);
        Assert.AreSame(systemMetadataProvider, options.SystemMetadataProvider);
        Assert.AreSame(baseboardMetadataProvider, options.BaseboardMetadataProvider);
        Assert.AreSame(biosMetadataProvider, options.BiosMetadataProvider);
        Assert.AreSame(dataPath, options.DataPath);
    }
    [ExcludeFromCodeCoverage]
    private class MockSerializer : IIronLedgerSerializer
    {
        public string Serialize<T>(T value) => string.Empty;
        public T? Deserialize<T>(string data) => default;
    }

    [ExcludeFromCodeCoverage]
    private class MockComponentDataProvider : IComponentDataProvider
    {
        public IReadOnlyList<ComponentData> GetData() => [];
    }

    [ExcludeFromCodeCoverage]
    private class MockMetadataProvider : IAssetMetadataProvider
    {
        public AssetMetadata GetMetadata() => AssetMetadata.Empty;
    }
}
