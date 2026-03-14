namespace IronLedgerLib.UI.Tests;

[TestClass]
public class ObservableAssetTests
{
    [TestMethod]
    public void ObservableAsset_Constructor_SetsDefaultProperties()
    {
        // Arrange
        var provider = new EmptyMetadataProvider();
        var id = new AssetIdFactory(provider, provider, provider).Create();
        var asset = new ObservableAsset(id);

        // Assert
        Assert.AreEqual(AssetMetadata.Empty, asset.AssetId.SystemMetadata);
        Assert.AreEqual(AssetMetadata.Empty, asset.AssetId.BaseBoardMetadata);
        Assert.AreEqual(AssetMetadata.Empty, asset.AssetId.BiosMetadata);

        Assert.AreEqual(ComponentData.Empty, asset.System.Data);
        Assert.IsEmpty(asset.Disks);
        Assert.IsEmpty(asset.Memory);
        Assert.IsEmpty(asset.Processors);
    }

    private class EmptyMetadataProvider : IAssetMetadataProvider
    {
        public AssetMetadata GetMetadata() => AssetMetadata.Empty;
    }
}

