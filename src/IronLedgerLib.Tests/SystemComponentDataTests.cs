namespace IronLedgerLib.Tests;

[TestClass]
public class SystemComponentDataTests
{
    [TestMethod]
    public void SystemComponentData_CanBeCreated_WithRequiredProperties()
    {
        // Arrange
        var processors = new List<ComponentData>
        {
            CreateMockComponentData("CPU-001", "Intel", "Core i7", "Intel Core i7-9700K")
        };

        var system = CreateMockComponentData("SYS-001", "Dell", "XPS 15", "Dell XPS 15 9500");

        var memory = new List<ComponentData>
        {
            CreateMockComponentData("MEM-001", "Corsair", "DDR4-3200", "Corsair Vengeance")
        };

        var disks = new List<ComponentData>
        {
            CreateMockComponentData("DISK-001", "Samsung", "970 EVO", "Samsung SSD 970 EVO")
        };

        // Act
        var systemComponentData = new SystemComponentData
        {
            Processors = processors,
            System = system,
            Memory = memory,
            Disks = disks
        };

        // Assert
        Assert.IsNotNull(systemComponentData);
        Assert.AreEqual(1, systemComponentData.Processors.Count);
        Assert.IsNotNull(systemComponentData.System);
        Assert.AreEqual(1, systemComponentData.Memory.Count);
        Assert.AreEqual(1, systemComponentData.Disks.Count);
    }

    [TestMethod]
    public void SystemComponentData_AllRequiredProperties_MustBeInitialized()
    {
        // Arrange
        var processors = new List<ComponentData>();
        var system = ComponentData.Empty;
        var memory = new List<ComponentData>();
        var disks = new List<ComponentData>();

        // Act
        var systemComponentData = new SystemComponentData
        {
            Processors = processors,
            System = system,
            Memory = memory,
            Disks = disks
        };

        // Assert
        Assert.IsNotNull(systemComponentData);
        Assert.AreEqual(0, systemComponentData.Processors.Count);
        Assert.AreEqual(ComponentData.Empty, systemComponentData.System);
        Assert.AreEqual(0, systemComponentData.Memory.Count);
        Assert.AreEqual(0, systemComponentData.Disks.Count);
    }

    [TestMethod]
    public void SystemComponentData_Processors_CanContainMultipleItems()
    {
        // Arrange
        var processors = new List<ComponentData>
        {
            CreateMockComponentData("CPU-001", "Intel", "Core i7"),
            CreateMockComponentData("CPU-002", "Intel", "Core i5"),
            CreateMockComponentData("CPU-003", "AMD", "Ryzen 7")
        };

        // Act
        var systemComponentData = new SystemComponentData
        {
            Processors = processors,
            System = ComponentData.Empty,
            Memory = Array.Empty<ComponentData>(),
            Disks = Array.Empty<ComponentData>()
        };

        // Assert
        Assert.AreEqual(3, systemComponentData.Processors.Count);
        Assert.AreEqual("CPU-001", systemComponentData.Processors[0].Metadata.SerialNumber);
        Assert.AreEqual("CPU-002", systemComponentData.Processors[1].Metadata.SerialNumber);
        Assert.AreEqual("CPU-003", systemComponentData.Processors[2].Metadata.SerialNumber);
    }

    [TestMethod]
    public void SystemComponentData_Memory_CanContainMultipleItems()
    {
        // Arrange
        var memory = new List<ComponentData>
        {
            CreateMockComponentData("MEM-001", "Corsair", "DDR4-3200"),
            CreateMockComponentData("MEM-002", "Corsair", "DDR4-3200"),
            CreateMockComponentData("MEM-003", "Kingston", "DDR4-2666"),
            CreateMockComponentData("MEM-004", "Kingston", "DDR4-2666")
        };

        // Act
        var systemComponentData = new SystemComponentData
        {
            Processors = Array.Empty<ComponentData>(),
            System = ComponentData.Empty,
            Memory = memory,
            Disks = Array.Empty<ComponentData>()
        };

        // Assert
        Assert.AreEqual(4, systemComponentData.Memory.Count);
        Assert.AreEqual("MEM-001", systemComponentData.Memory[0].Metadata.SerialNumber);
        Assert.AreEqual("MEM-004", systemComponentData.Memory[3].Metadata.SerialNumber);
    }

    [TestMethod]
    public void SystemComponentData_Disks_CanContainMultipleItems()
    {
        // Arrange
        var disks = new List<ComponentData>
        {
            CreateMockComponentData("DISK-001", "Samsung", "970 EVO"),
            CreateMockComponentData("DISK-002", "WD", "Blue SN550")
        };

        // Act
        var systemComponentData = new SystemComponentData
        {
            Processors = Array.Empty<ComponentData>(),
            System = ComponentData.Empty,
            Memory = Array.Empty<ComponentData>(),
            Disks = disks
        };

        // Assert
        Assert.AreEqual(2, systemComponentData.Disks.Count);
        Assert.AreEqual("DISK-001", systemComponentData.Disks[0].Metadata.SerialNumber);
        Assert.AreEqual("DISK-002", systemComponentData.Disks[1].Metadata.SerialNumber);
    }

    [TestMethod]
    public void SystemComponentData_CanBeCreated_WithEmptySystem()
    {
        // Arrange & Act
        var systemComponentData = new SystemComponentData
        {
            Processors = Array.Empty<ComponentData>(),
            System = ComponentData.Empty,
            Memory = Array.Empty<ComponentData>(),
            Disks = Array.Empty<ComponentData>()
        };

        // Assert
        Assert.IsNotNull(systemComponentData);
        Assert.AreEqual(0, systemComponentData.Processors.Count);
        Assert.AreEqual(ComponentData.Empty, systemComponentData.System);
        Assert.AreEqual(0, systemComponentData.Memory.Count);
        Assert.AreEqual(0, systemComponentData.Disks.Count);
    }

    [TestMethod]
    public void SystemComponentData_IsImmutable_PropertiesAreInitOnly()
    {
        // Arrange
        var processors = new List<ComponentData>
        {
            CreateMockComponentData("CPU-001", "Intel", "Core i7")
        };

        var system = CreateMockComponentData("SYS-001", "Dell", "XPS 15");

        var memory = new List<ComponentData>
        {
            CreateMockComponentData("MEM-001", "Corsair", "DDR4")
        };

        var disks = new List<ComponentData>
        {
            CreateMockComponentData("DISK-001", "Samsung", "970 EVO")
        };

        // Act
        var systemComponentData = new SystemComponentData
        {
            Processors = processors,
            System = system,
            Memory = memory,
            Disks = disks
        };

        // Assert - Properties are init-only (compile-time check)
        Assert.AreEqual(processors, systemComponentData.Processors);
        Assert.AreEqual(system, systemComponentData.System);
        Assert.AreEqual(memory, systemComponentData.Memory);
        Assert.AreEqual(disks, systemComponentData.Disks);
    }

    [TestMethod]
    public void SystemComponentData_RecordEquality_WorksCorrectly()
    {
        // Arrange
        var processors = new List<ComponentData>
        {
            CreateMockComponentData("CPU-001", "Intel", "Core i7")
        };

        var data1 = new SystemComponentData
        {
            Processors = processors,
            System = ComponentData.Empty,
            Memory = Array.Empty<ComponentData>(),
            Disks = Array.Empty<ComponentData>()
        };

        var data2 = new SystemComponentData
        {
            Processors = processors,
            System = ComponentData.Empty,
            Memory = Array.Empty<ComponentData>(),
            Disks = Array.Empty<ComponentData>()
        };

        // Act & Assert - Records with same reference values should be equal
        Assert.AreEqual(data1, data2);
    }

    [TestMethod]
    public void SystemComponentData_WithCompleteSystemInventory_ContainsAllData()
    {
        // Arrange
        var processors = new List<ComponentData>
        {
            CreateMockComponentData("CPU-001", "Intel", "Core i7-9700K", "Intel Core i7-9700K @ 3.60GHz")
        };

        var system = CreateMockComponentData("SYS-001", "Dell Inc.", "XPS 15 9500", "DESKTOP-ABC123");

        var memory = new List<ComponentData>
        {
            CreateMockComponentData("MEM-001", "Corsair", "CMK32GX4M2D3200C16", "Physical Memory"),
            CreateMockComponentData("MEM-002", "Corsair", "CMK32GX4M2D3200C16", "Physical Memory")
        };

        var disks = new List<ComponentData>
        {
            CreateMockComponentData("DISK-001", "Samsung", "SSD 970 EVO 1TB", "Samsung SSD 970 EVO 1TB"),
            CreateMockComponentData("DISK-002", "Seagate", "ST2000DM008", "Seagate BarraCuda 2TB")
        };

        // Act
        var systemComponentData = new SystemComponentData
        {
            Processors = processors,
            System = system,
            Memory = memory,
            Disks = disks
        };

        // Assert
        Assert.AreEqual(1, systemComponentData.Processors.Count);
        Assert.IsNotNull(systemComponentData.System);
        Assert.AreEqual(2, systemComponentData.Memory.Count);
        Assert.AreEqual(2, systemComponentData.Disks.Count);

        // Verify processor details
        Assert.AreEqual("Intel", systemComponentData.Processors[0].Metadata.Manufacturer);
        Assert.AreEqual("Intel Core i7-9700K @ 3.60GHz", systemComponentData.Processors[0].Caption);

        // Verify system details
        Assert.AreEqual("Dell Inc.", systemComponentData.System.Metadata.Manufacturer);
        Assert.AreEqual("DESKTOP-ABC123", systemComponentData.System.Caption);

        // Verify memory details
        Assert.AreEqual("Corsair", systemComponentData.Memory[0].Metadata.Manufacturer);
        Assert.AreEqual("Corsair", systemComponentData.Memory[1].Metadata.Manufacturer);

        // Verify disk details
        Assert.AreEqual("Samsung", systemComponentData.Disks[0].Metadata.Manufacturer);
        Assert.AreEqual("Seagate", systemComponentData.Disks[1].Metadata.Manufacturer);
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
            Properties = new List<ComponentProperty>
            {
                new("Property1", "Value1"),
                new("Property2", "Value2")
            }
        };
    }
}
