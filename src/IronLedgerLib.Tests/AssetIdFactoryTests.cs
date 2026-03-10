namespace IronLedgerLib.Tests;

[TestClass]
public class AssetIdFactoryTests
{
    [TestMethod]
    public void AssetIdFactory_Create_ReturnsAssetIdWithAllMetadata()
    {
        // Arrange
        var systemProvider = new MockMetadataProvider("SYS-001", "System Mfg", "System Model");
        var baseboardProvider = new MockMetadataProvider("BB-001", "Baseboard Mfg", "Baseboard Model");
        var biosProvider = new MockMetadataProvider("BIOS-001", "BIOS Mfg", "BIOS Model");

        var factory = new AssetIdFactory(systemProvider, baseboardProvider, biosProvider);

        // Act
        var assetId = factory.Create();

        // Assert
        Assert.IsNotNull(assetId);
        Assert.AreEqual("SYS-001", assetId.SystemMetadata.SerialNumber);
        Assert.AreEqual("System Mfg", assetId.SystemMetadata.Manufacturer);
        Assert.AreEqual("System Model", assetId.SystemMetadata.Product);

        Assert.AreEqual("BB-001", assetId.BaseBoardMetadata.SerialNumber);
        Assert.AreEqual("Baseboard Mfg", assetId.BaseBoardMetadata.Manufacturer);
        Assert.AreEqual("Baseboard Model", assetId.BaseBoardMetadata.Product);

        Assert.AreEqual("BIOS-001", assetId.BiosMetadata.SerialNumber);
        Assert.AreEqual("BIOS Mfg", assetId.BiosMetadata.Manufacturer);
        Assert.AreEqual("BIOS Model", assetId.BiosMetadata.Product);
    }

    [TestMethod]
    public void AssetIdFactory_DefaultConstructor_CreatesFactoryWithDefaultProviders()
    {
        // Arrange & Act
        var factory = new AssetIdFactory();

        // Assert
        Assert.IsNotNull(factory);
    }

    [TestMethod]
    public void AssetIdFactory_Constructor_UsesDefaultSystemProvider_WhenNull()
    {
        // Arrange
        var baseboardProvider = new MockMetadataProvider("BB-001", "BB Mfg", "BB Model");
        var biosProvider = new MockMetadataProvider("BIOS-001", "BIOS Mfg", "BIOS Model");

        var factory = new AssetIdFactory(null, baseboardProvider, biosProvider);

        // Act
        var assetId = factory.Create();

        // Assert
        Assert.IsNotNull(assetId.SystemMetadata);
        Assert.AreEqual("BB-001", assetId.BaseBoardMetadata.SerialNumber);
        Assert.AreEqual("BIOS-001", assetId.BiosMetadata.SerialNumber);
    }

    [TestMethod]
    public void AssetIdFactory_Constructor_UsesDefaultBaseboardProvider_WhenNull()
    {
        // Arrange
        var systemProvider = new MockMetadataProvider("SYS-001", "SYS Mfg", "SYS Model");
        var biosProvider = new MockMetadataProvider("BIOS-001", "BIOS Mfg", "BIOS Model");

        var factory = new AssetIdFactory(systemProvider, null, biosProvider);

        // Act
        var assetId = factory.Create();

        // Assert
        Assert.AreEqual("SYS-001", assetId.SystemMetadata.SerialNumber);
        Assert.IsNotNull(assetId.BaseBoardMetadata);
        Assert.AreEqual("BIOS-001", assetId.BiosMetadata.SerialNumber);
    }

    [TestMethod]
    public void AssetIdFactory_Constructor_UsesDefaultBiosProvider_WhenNull()
    {
        // Arrange
        var systemProvider = new MockMetadataProvider("SYS-001", "SYS Mfg", "SYS Model");
        var baseboardProvider = new MockMetadataProvider("BB-001", "BB Mfg", "BB Model");

        var factory = new AssetIdFactory(systemProvider, baseboardProvider, null);

        // Act
        var assetId = factory.Create();

        // Assert
        Assert.AreEqual("SYS-001", assetId.SystemMetadata.SerialNumber);
        Assert.AreEqual("BB-001", assetId.BaseBoardMetadata.SerialNumber);
        Assert.IsNotNull(assetId.BiosMetadata);
    }

    [TestMethod]
    public void AssetIdFactory_Constructor_UsesAllDefaults_WhenAllNull()
    {
        // Arrange & Act
        var factory = new AssetIdFactory(null, null, null);
        var assetId = factory.Create();

        // Assert
        Assert.IsNotNull(assetId.SystemMetadata);
        Assert.IsNotNull(assetId.BaseBoardMetadata);
        Assert.IsNotNull(assetId.BiosMetadata);
    }

    private class MockMetadataProvider : IAssetMetadataProvider
    {
        private readonly string _serialNumber;
        private readonly string _manufacturer;
        private readonly string _product;

        public MockMetadataProvider(
            string serialNumber = "MOCK-SN",
            string manufacturer = "Mock Manufacturer",
            string product = "Mock Product")
        {
            _serialNumber = serialNumber;
            _manufacturer = manufacturer;
            _product = product;
        }

        public AssetMetadata GetMetadata()
        {
            return new AssetMetadata
            {
                SerialNumber = _serialNumber,
                Manufacturer = _manufacturer,
                Product = _product
            };
        }
    }
}
