using Microsoft.Management.Infrastructure;
using Tudormobile.IronLedgerLib.Providers;

namespace IronLedgerLib.Tests.Providers;

[TestClass]
public class CimMetadataProviderBaseTests
{
    // TestProvider overrides QueryRawMetadata() to return controlled values without
    // making any real WMI call, isolating the null-to-empty coercion logic in GetMetadata().
    [TestMethod]
    public void GetMetadata_ConvertsNullSerialNumberToEmpty()
    {
        // Arrange
        var provider = new TestProvider(serialNumber: null, manufacturer: "Mfg", product: "Product");

        // Act
        var metadata = provider.GetMetadata();

        // Assert
        Assert.AreEqual(string.Empty, metadata.SerialNumber);
        Assert.AreEqual("Mfg", metadata.Manufacturer);
        Assert.AreEqual("Product", metadata.Product);
    }

    [TestMethod]
    public void GetMetadata_ConvertsNullManufacturerToEmpty()
    {
        // Arrange
        var provider = new TestProvider(serialNumber: "SN123", manufacturer: null, product: "Product");

        // Act
        var metadata = provider.GetMetadata();

        // Assert
        Assert.AreEqual("SN123", metadata.SerialNumber);
        Assert.AreEqual(string.Empty, metadata.Manufacturer);
        Assert.AreEqual("Product", metadata.Product);
    }

    [TestMethod]
    public void GetMetadata_ConvertsNullProductToEmpty()
    {
        // Arrange
        var provider = new TestProvider(serialNumber: "SN123", manufacturer: "Mfg", product: null);

        // Act
        var metadata = provider.GetMetadata();

        // Assert
        Assert.AreEqual("SN123", metadata.SerialNumber);
        Assert.AreEqual("Mfg", metadata.Manufacturer);
        Assert.AreEqual(string.Empty, metadata.Product);
    }

    [TestMethod]
    public void GetMetadata_ConvertsAllNullsToEmpty()
    {
        // Arrange
        var provider = new TestProvider(serialNumber: null, manufacturer: null, product: null);

        // Act
        var metadata = provider.GetMetadata();

        // Assert
        Assert.AreEqual(string.Empty, metadata.SerialNumber);
        Assert.AreEqual(string.Empty, metadata.Manufacturer);
        Assert.AreEqual(string.Empty, metadata.Product);
    }

    private class TestProvider : CimMetadataProviderBase
    {
        private readonly string? _serialNumber;
        private readonly string? _manufacturer;
        private readonly string? _product;

        public TestProvider(string? serialNumber, string? manufacturer, string? product)
        {
            _serialNumber = serialNumber;
            _manufacturer = manufacturer;
            _product = product;
        }

        protected override string WmiClassName => string.Empty;
        protected override string Properties => string.Empty;

        protected override (string? SerialNumber, string? Manufacturer, string? Product)? QueryRawMetadata()
            => (_serialNumber, _manufacturer, _product);

        protected override (string? SerialNumber, string? Manufacturer, string? Product) ExtractMetadata(CimInstance instance)
            => throw new NotImplementedException();
    }

}
