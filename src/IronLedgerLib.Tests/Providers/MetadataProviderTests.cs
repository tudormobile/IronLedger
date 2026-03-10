using Tudormobile.IronLedgerLib.Providers;

namespace IronLedgerLib.Tests.Providers;

[TestClass]
public class MetadataProviderTests
{
    [TestMethod]
    public void SystemMetadataProvider_GetMetadata_ReturnsNonNullMetadata()
    {
        // Arrange
        var provider = new SystemMetadataProvider();

        // Act
        var metadata = provider.GetMetadata();

        // Assert
        Assert.IsNotNull(metadata);
        Assert.IsNotNull(metadata.SerialNumber);
        Assert.IsNotNull(metadata.Manufacturer);
        Assert.IsNotNull(metadata.Product);
    }

    [TestMethod]
    public void BaseboardMetadataProvider_GetMetadata_ReturnsNonNullMetadata()
    {
        // Arrange
        var provider = new BaseboardMetadataProvider();

        // Act
        var metadata = provider.GetMetadata();

        // Assert
        Assert.IsNotNull(metadata);
        Assert.IsNotNull(metadata.SerialNumber);
        Assert.IsNotNull(metadata.Manufacturer);
        Assert.IsNotNull(metadata.Product);
    }

    [TestMethod]
    public void BiosMetadataProvider_GetMetadata_ReturnsNonNullMetadata()
    {
        // Arrange
        var provider = new BiosMetadataProvider();

        // Act
        var metadata = provider.GetMetadata();

        // Assert
        Assert.IsNotNull(metadata);
        Assert.IsNotNull(metadata.SerialNumber);
        Assert.IsNotNull(metadata.Manufacturer);
        Assert.IsNotNull(metadata.Product);
    }
}
