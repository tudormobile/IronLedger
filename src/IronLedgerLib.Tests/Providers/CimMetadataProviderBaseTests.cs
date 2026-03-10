using Microsoft.Management.Infrastructure;
using Tudormobile.IronLedgerLib.Providers;

namespace IronLedgerLib.Tests.Providers;

[TestClass]
public class CimMetadataProviderBaseTests
{
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

    [TestMethod]
    public void GetPropertyValue_ReturnsValueForExistingProperty()
    {
        // Arrange
        var provider = new PropertyTestProvider();

        // Act
        provider.CapturePropertyValue("Caption");

        // Assert
        Assert.IsNotNull(provider.GetCapturedValue());
    }

    [TestMethod]
    public void GetPropertyValue_ReturnsNullForNonExistentProperty()
    {
        // Arrange
        var provider = new PropertyTestProvider();

        // Act
        provider.CapturePropertyValue("NonExistentProperty");

        // Assert
        Assert.IsNull(provider.GetCapturedValue());
    }

    /// <summary>
    /// Test provider that returns controlled tuple values using a real WMI class.
    /// Uses Win32_OperatingSystem which always has exactly one instance.
    /// </summary>
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

        protected override string WmiClassName => "Win32_OperatingSystem";
        protected override string Properties => "Caption";

        protected override (string? SerialNumber, string? Manufacturer, string? Product) ExtractMetadata(CimInstance instance)
        {
            return (_serialNumber, _manufacturer, _product);
        }
    }

    /// <summary>
    /// Test provider that exposes GetPropertyValue for direct testing.
    /// Uses Win32_OperatingSystem which always has exactly one instance.
    /// </summary>
    private class PropertyTestProvider : CimMetadataProviderBase
    {
        private string? _capturedValue;

        public string? GetCapturedValue() => _capturedValue;

        public void CapturePropertyValue(string propertyName)
        {
            using var session = Microsoft.Management.Infrastructure.CimSession.Create(null);
            var instances = session.QueryInstances(@"root\cimv2", "WQL", $"SELECT {Properties} FROM {WmiClassName}");
            foreach (var instance in instances)
            {
                _capturedValue = GetPropertyValue(instance, propertyName);
                return;
            }
        }

        protected override string WmiClassName => "Win32_OperatingSystem";
        protected override string Properties => "Caption";

        protected override (string? SerialNumber, string? Manufacturer, string? Product) ExtractMetadata(CimInstance instance)
            => (null, null, null);
    }

}
