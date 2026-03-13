using Microsoft.Extensions.Logging;

namespace IronLedgerLib.Tests;

[TestClass]
public class LoggingTests
{
    // --- ComponentDataFactory ---

    [TestMethod]
    public void ComponentDataFactory_AcceptsNullLogger_DoesNotThrow()
    {
        var factory = new ComponentDataFactory(logger: null);

        Assert.IsNotNull(factory);
    }

    [TestMethod]
    public void ComponentDataFactory_GetProcessors_LogsDebugMessages()
    {
        var logger = new CapturingLogger<ComponentDataFactory>();
        var provider = new MockComponentDataProvider("CPU-001", "Intel", "Core i7");
        var factory = new ComponentDataFactory(processorProvider: provider, logger: logger);

        factory.GetProcessors();

        Assert.IsTrue(logger.HasEntries(LogLevel.Debug));
    }

    [TestMethod]
    public void ComponentDataFactory_GetSystem_LogsDebugMessages()
    {
        var logger = new CapturingLogger<ComponentDataFactory>();
        var provider = new MockComponentDataProvider("SYS-001", "Dell", "XPS");
        var factory = new ComponentDataFactory(systemProvider: provider, logger: logger);

        factory.GetSystem();

        Assert.IsTrue(logger.HasEntries(LogLevel.Debug));
    }

    [TestMethod]
    public void ComponentDataFactory_GetMemory_LogsDebugMessages()
    {
        var logger = new CapturingLogger<ComponentDataFactory>();
        var provider = new MockComponentDataProvider("MEM-001", "Corsair", "DDR4");
        var factory = new ComponentDataFactory(memoryProvider: provider, logger: logger);

        factory.GetMemory();

        Assert.IsTrue(logger.HasEntries(LogLevel.Debug));
    }

    [TestMethod]
    public void ComponentDataFactory_GetDisks_LogsDebugMessages()
    {
        var logger = new CapturingLogger<ComponentDataFactory>();
        var provider = new MockComponentDataProvider("DISK-001", "Samsung", "970 EVO");
        var factory = new ComponentDataFactory(diskProvider: provider, logger: logger);

        factory.GetDisks();

        Assert.IsTrue(logger.HasEntries(LogLevel.Debug));
    }

    [TestMethod]
    public void ComponentDataFactory_Create_LogsInformation_OnSuccess()
    {
        var logger = new CapturingLogger<ComponentDataFactory>();
        var provider = new MockComponentDataProvider("001", "Mfg", "Product");
        var factory = new ComponentDataFactory(provider, provider, provider, provider, logger);

        factory.Create();

        Assert.IsTrue(logger.HasEntries(LogLevel.Information));
    }

    [TestMethod]
    public void ComponentDataFactory_GetProcessors_LogsError_WhenProviderThrows()
    {
        var logger = new CapturingLogger<ComponentDataFactory>();
        var factory = new ComponentDataFactory(new ThrowingComponentDataProvider(), null, null, null, logger);

        Assert.ThrowsExactly<ComponentDataProviderException>(() => factory.GetProcessors());

        Assert.IsTrue(logger.HasEntries(LogLevel.Error));
    }

    [TestMethod]
    public void ComponentDataFactory_GetSystem_LogsError_WhenProviderThrows()
    {
        var logger = new CapturingLogger<ComponentDataFactory>();
        var factory = new ComponentDataFactory(null, new ThrowingComponentDataProvider(), null, null, logger);

        Assert.ThrowsExactly<ComponentDataProviderException>(() => factory.GetSystem());

        Assert.IsTrue(logger.HasEntries(LogLevel.Error));
    }

    [TestMethod]
    public void ComponentDataFactory_GetMemory_LogsError_WhenProviderThrows()
    {
        var logger = new CapturingLogger<ComponentDataFactory>();
        var factory = new ComponentDataFactory(null, null, new ThrowingComponentDataProvider(), null, logger);

        Assert.ThrowsExactly<ComponentDataProviderException>(() => factory.GetMemory());

        Assert.IsTrue(logger.HasEntries(LogLevel.Error));
    }

    [TestMethod]
    public void ComponentDataFactory_GetDisks_LogsError_WhenProviderThrows()
    {
        var logger = new CapturingLogger<ComponentDataFactory>();
        var factory = new ComponentDataFactory(null, null, null, new ThrowingComponentDataProvider(), logger);

        Assert.ThrowsExactly<ComponentDataProviderException>(() => factory.GetDisks());

        Assert.IsTrue(logger.HasEntries(LogLevel.Error));
    }

    [TestMethod]
    public void ComponentDataFactory_ExceptionStillPropagates_AfterLogging()
    {
        var logger = new CapturingLogger<ComponentDataFactory>();
        var factory = new ComponentDataFactory(new ThrowingComponentDataProvider(), null, null, null, logger);

        var ex = Assert.ThrowsExactly<ComponentDataProviderException>(() => factory.GetProcessors());

        Assert.AreEqual("TestProvider", ex.ProviderName);
        Assert.IsTrue(logger.HasEntries(LogLevel.Error));
    }

    [TestMethod]
    public void ComponentDataFactory_Create_LogsError_WhenProviderThrows()
    {
        var logger = new CapturingLogger<ComponentDataFactory>();
        var provider = new MockComponentDataProvider("001", "Mfg", "Product");
        var factory = new ComponentDataFactory(new ThrowingComponentDataProvider(), provider, provider, provider, logger);

        Assert.ThrowsExactly<ComponentDataProviderException>(() => factory.Create());

        Assert.IsTrue(logger.HasEntries(LogLevel.Error));
    }

    // --- AssetIdFactory ---

    [TestMethod]
    public void AssetIdFactory_AcceptsNullLogger_DoesNotThrow()
    {
        var factory = new AssetIdFactory(logger: null);

        Assert.IsNotNull(factory);
    }

    [TestMethod]
    public void AssetIdFactory_Create_LogsDebugMessages_OnSuccess()
    {
        var logger = new CapturingLogger<AssetIdFactory>();
        var provider = new MockMetadataProvider();
        var factory = new AssetIdFactory(provider, provider, provider, logger);

        factory.Create();

        Assert.IsTrue(logger.HasEntries(LogLevel.Debug));
    }

    [TestMethod]
    public void AssetIdFactory_Create_LogsError_WhenProviderThrows()
    {
        var logger = new CapturingLogger<AssetIdFactory>();
        var provider = new MockMetadataProvider();
        var factory = new AssetIdFactory(new ThrowingMetadataProvider(), provider, provider, logger);

        Assert.ThrowsExactly<ComponentDataProviderException>(() => factory.Create());

        Assert.IsTrue(logger.HasEntries(LogLevel.Error));
    }

    [TestMethod]
    public void AssetIdFactory_Create_ExceptionStillPropagates_AfterLogging()
    {
        var logger = new CapturingLogger<AssetIdFactory>();
        var provider = new MockMetadataProvider();
        var factory = new AssetIdFactory(new ThrowingMetadataProvider(), provider, provider, logger);

        var ex = Assert.ThrowsExactly<ComponentDataProviderException>(() => factory.Create());

        Assert.AreEqual("TestMetadataProvider", ex.ProviderName);
        Assert.IsTrue(logger.HasEntries(LogLevel.Error));
    }

    [TestMethod]
    public void AssetIdFactory_Create_LoggedErrorIncludesException()
    {
        var logger = new CapturingLogger<AssetIdFactory>();
        var provider = new MockMetadataProvider();
        var factory = new AssetIdFactory(new ThrowingMetadataProvider(), provider, provider, logger);

        Assert.ThrowsExactly<ComponentDataProviderException>(() => factory.Create());

        var errorEntry = logger.Entries.Single(e => e.Level == LogLevel.Error);
        Assert.IsNotNull(errorEntry.Exception);
        Assert.IsInstanceOfType<ComponentDataProviderException>(errorEntry.Exception);
    }

    // --- Helpers ---

    private sealed class CapturingLogger<T> : ILogger<T>
    {
        private readonly List<(LogLevel Level, string Message, Exception? Exception)> _entries = [];

        public IEnumerable<(LogLevel Level, string Message, Exception? Exception)> Entries => _entries;

        public bool HasEntries(LogLevel level) => _entries.Any(e => e.Level == level);

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            => _entries.Add((logLevel, formatter(state, exception), exception));
    }

    private static ComponentData MakeComponentData(string serialNumber, string manufacturer, string product)
        => new()
        {
            Metadata = new AssetMetadata { SerialNumber = serialNumber, Manufacturer = manufacturer, Product = product },
            Caption = "Mock",
            Properties = []
        };

    private sealed class MockComponentDataProvider : IComponentDataProvider
    {
        private readonly IReadOnlyList<ComponentData> _data;

        public MockComponentDataProvider(string serialNumber, string manufacturer, string product)
            => _data = [MakeComponentData(serialNumber, manufacturer, product)];

        public IReadOnlyList<ComponentData> GetData() => _data;
    }

    private sealed class ThrowingComponentDataProvider : IComponentDataProvider
    {
        public IReadOnlyList<ComponentData> GetData()
            => throw new ComponentDataProviderException("Simulated failure") { ProviderName = "TestProvider" };
    }

    private sealed class MockMetadataProvider : IAssetMetadataProvider
    {
        public AssetMetadata GetMetadata() => AssetMetadata.Empty;
    }

    private sealed class ThrowingMetadataProvider : IAssetMetadataProvider
    {
        public AssetMetadata GetMetadata()
            => throw new ComponentDataProviderException("Simulated metadata failure") { ProviderName = "TestMetadataProvider" };
    }
}
