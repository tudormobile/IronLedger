namespace IronLedgerLib.Tests;

[TestClass]
public class ComponentDataFactoryTests
{
    [TestMethod]
    public void ComponentDataFactory_Create_ReturnsSystemComponentDataWithAllComponents()
    {
        // Arrange
        var processorProvider = new MockComponentDataProvider("CPU-001", "Intel", "Core i7");
        var systemProvider = new MockComponentDataProvider("SYS-001", "Dell", "XPS 15");
        var memoryProvider = new MockComponentDataProvider("MEM-001", "Corsair", "DDR4-3200");
        var diskProvider = new MockComponentDataProvider("DISK-001", "Samsung", "970 EVO");

        var factory = new ComponentDataFactory(
            processorProvider,
            systemProvider,
            memoryProvider,
            diskProvider);

        // Act
        var result = factory.Create();

        // Assert
        Assert.IsNotNull(result);
        Assert.HasCount(1, result.Processors);
        Assert.AreNotEqual(ComponentData.Empty, result.System);
        Assert.HasCount(1, result.Memory);
        Assert.HasCount(1, result.Disks);

        Assert.AreEqual("CPU-001", result.Processors[0].Metadata.SerialNumber);
        Assert.AreEqual("SYS-001", result.System.Metadata.SerialNumber);
        Assert.AreEqual("MEM-001", result.Memory[0].Metadata.SerialNumber);
        Assert.AreEqual("DISK-001", result.Disks[0].Metadata.SerialNumber);
    }

    [TestMethod]
    public void ComponentDataFactory_GetProcessors_ReturnsProcessorData()
    {
        // Arrange
        var processorProvider = new MockComponentDataProvider("CPU-001", "Intel", "Core i7", "Intel Core i7-9700K");
        var factory = new ComponentDataFactory(processorProvider, null, null, null);

        // Act
        var processors = factory.GetProcessors();

        // Assert
        Assert.IsNotNull(processors);
        Assert.HasCount(1, processors);
        Assert.AreEqual("CPU-001", processors[0].Metadata.SerialNumber);
        Assert.AreEqual("Intel", processors[0].Metadata.Manufacturer);
        Assert.AreEqual("Core i7", processors[0].Metadata.Product);
        Assert.AreEqual("Intel Core i7-9700K", processors[0].Caption);
    }

    [TestMethod]
    public void ComponentDataFactory_GetSystem_ReturnsSystemData()
    {
        // Arrange
        var systemProvider = new MockComponentDataProvider("SYS-001", "Dell", "XPS", "Dell XPS 15");
        var factory = new ComponentDataFactory(null, systemProvider, null, null);

        // Act
        var system = factory.GetSystem();

        // Assert
        Assert.IsNotNull(system);
        Assert.AreEqual("SYS-001", system.Metadata.SerialNumber);
        Assert.AreEqual("Dell", system.Metadata.Manufacturer);
        Assert.AreEqual("XPS", system.Metadata.Product);
        Assert.AreEqual("Dell XPS 15", system.Caption);
    }

    [TestMethod]
    public void ComponentDataFactory_GetSystem_WithNoResult_ReturnsEmptyData()
    {
        // Arrange
        var systemProvider = new MockComponentDataProvider();
        var factory = new ComponentDataFactory(null, systemProvider, null, null);

        // Act
        var system = factory.GetSystem();

        // Assert
        Assert.AreEqual(ComponentData.Empty, system);
    }

    [TestMethod]
    public void ComponentDataFactory_GetMemory_ReturnsMemoryData()
    {
        // Arrange
        var memoryProvider = new MockComponentDataProvider("MEM-001", "Corsair", "DDR4", "Corsair Vengeance");
        var factory = new ComponentDataFactory(null, null, memoryProvider, null);

        // Act
        var memory = factory.GetMemory();

        // Assert
        Assert.IsNotNull(memory);
        Assert.HasCount(1, memory);
        Assert.AreEqual("MEM-001", memory[0].Metadata.SerialNumber);
        Assert.AreEqual("Corsair", memory[0].Metadata.Manufacturer);
        Assert.AreEqual("DDR4", memory[0].Metadata.Product);
        Assert.AreEqual("Corsair Vengeance", memory[0].Caption);
    }

    [TestMethod]
    public void ComponentDataFactory_GetDisks_ReturnsDiskData()
    {
        // Arrange
        var diskProvider = new MockComponentDataProvider("DISK-001", "Samsung", "970 EVO", "Samsung SSD 970 EVO");
        var factory = new ComponentDataFactory(null, null, null, diskProvider);

        // Act
        var disks = factory.GetDisks();

        // Assert
        Assert.IsNotNull(disks);
        Assert.HasCount(1, disks);
        Assert.AreEqual("DISK-001", disks[0].Metadata.SerialNumber);
        Assert.AreEqual("Samsung", disks[0].Metadata.Manufacturer);
        Assert.AreEqual("970 EVO", disks[0].Metadata.Product);
        Assert.AreEqual("Samsung SSD 970 EVO", disks[0].Caption);
    }

    [TestMethod]
    public void ComponentDataFactory_DefaultConstructor_CreatesFactoryWithDefaultProviders()
    {
        // Arrange & Act
        var factory = new ComponentDataFactory();

        // Assert
        Assert.IsNotNull(factory);
    }

    [TestMethod]
    public void ComponentDataFactory_Constructor_UsesAllDefaults_WhenAllNull()
    {
        // Arrange & Act
        var factory = new ComponentDataFactory(null, null, null, null);
        var result = factory.Create();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Processors);
        Assert.IsNotNull(result.System);
        Assert.IsNotNull(result.Memory);
        Assert.IsNotNull(result.Disks);
    }

    [TestMethod]
    public void ComponentDataFactory_Create_WithMultipleProcessors_ReturnsAllProcessors()
    {
        // Arrange
        var processorProvider = new MockComponentDataProvider(
            [
                CreateMockComponentData("CPU-001", "Intel", "Core i7", "Intel Core i7-9700K"),
                CreateMockComponentData("CPU-002", "Intel", "Core i5", "Intel Core i5-8400")
            ]);

        var factory = new ComponentDataFactory(processorProvider, null, null, null);

        // Act
        var result = factory.Create();

        // Assert
        Assert.HasCount(2, result.Processors);
        Assert.AreEqual("CPU-001", result.Processors[0].Metadata.SerialNumber);
        Assert.AreEqual("CPU-002", result.Processors[1].Metadata.SerialNumber);
    }

    [TestMethod]
    public void ComponentDataFactory_Create_WithEmptyProviders_ReturnsEmptyCollections()
    {
        // Arrange
        var emptyProvider = new MockComponentDataProvider([]);
        var factory = new ComponentDataFactory(emptyProvider, emptyProvider, emptyProvider, emptyProvider);

        // Act
        var result = factory.Create();

        // Assert
        Assert.AreEqual(ComponentData.Empty, result.System);
        Assert.IsEmpty(result.Processors);
        Assert.IsEmpty(result.Memory);
        Assert.IsEmpty(result.Disks);
    }

    [TestMethod]
    public void ComponentDataFactory_IndividualGetMethods_DoNotAffectEachOther()
    {
        // Arrange
        var processorProvider = new MockComponentDataProvider("CPU-001", "Intel", "Core i7");
        var diskProvider = new MockComponentDataProvider("DISK-001", "Samsung", "970 EVO");
        var factory = new ComponentDataFactory(processorProvider, null, null, diskProvider);

        // Act
        var processors = factory.GetProcessors();
        var disks = factory.GetDisks();

        // Assert
        Assert.HasCount(1, processors);
        Assert.HasCount(1, disks);
        Assert.AreEqual("CPU-001", processors[0].Metadata.SerialNumber);
        Assert.AreEqual("DISK-001", disks[0].Metadata.SerialNumber);
    }

    [TestMethod]
    public void ComponentDataFactory_GetProcessors_WithMultipleItems_ReturnsAllItems()
    {
        // Arrange
        var processorProvider = new MockComponentDataProvider(
            [
                CreateMockComponentData("CPU-001", "Intel", "Core i7"),
                CreateMockComponentData("CPU-002", "Intel", "Core i5"),
                CreateMockComponentData("CPU-003", "AMD", "Ryzen 7")
            ]);

        var factory = new ComponentDataFactory(processorProvider, null, null, null);

        // Act
        var processors = factory.GetProcessors();

        // Assert
        Assert.HasCount(3, processors);
        Assert.AreEqual("CPU-001", processors[0].Metadata.SerialNumber);
        Assert.AreEqual("CPU-002", processors[1].Metadata.SerialNumber);
        Assert.AreEqual("CPU-003", processors[2].Metadata.SerialNumber);
    }

    private static ComponentData CreateMockComponentData(
        string serialNumber,
        string manufacturer,
        string product,
        string caption = "Mock Component")
    {
        return new ComponentData
        {
            Metadata = new AssetMetadata
            {
                SerialNumber = serialNumber,
                Manufacturer = manufacturer,
                Product = product
            },
            Caption = caption,
            Properties =
            [
                new("Property1", "Value1"),
                new("Property2", "Value2")
            ]
        };
    }

    private class MockComponentDataProvider : IComponentDataProvider
    {
        private readonly IReadOnlyList<ComponentData> _data;

        public MockComponentDataProvider(
            string serialNumber = "MOCK-SN",
            string manufacturer = "Mock Manufacturer",
            string product = "Mock Product",
            string caption = "Mock Component")
        {
            _data =
            [
                CreateMockComponentData(serialNumber, manufacturer, product, caption)
            ];
        }
        public MockComponentDataProvider() { _data = []; }

        public MockComponentDataProvider(IReadOnlyList<ComponentData> data)
        {
            _data = data;
        }

        public IReadOnlyList<ComponentData> GetData() => _data;
    }
}
