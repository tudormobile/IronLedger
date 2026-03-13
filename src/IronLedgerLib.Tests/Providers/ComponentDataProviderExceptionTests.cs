namespace IronLedgerLib.Tests.Providers;

[TestClass]
public class ComponentDataProviderExceptionTests
{
    [TestMethod]
    public void ComponentDataFactory_Create_PropagatesProviderException()
    {
        // Arrange
        var throwingProvider = new ThrowingComponentDataProvider();
        var factory = new ComponentDataFactory(throwingProvider, null, null, null);

        // Act & Assert
        var exception = Assert.ThrowsExactly<ComponentDataProviderException>(() => factory.Create());

        Assert.AreEqual("TestProvider", exception.ProviderName);
        Assert.IsTrue(exception.Message.Contains("Simulated provider failure"));
    }

    [TestMethod]
    public void ComponentDataFactory_GetProcessors_PropagatesProviderException()
    {
        // Arrange
        var throwingProvider = new ThrowingComponentDataProvider();
        var factory = new ComponentDataFactory(throwingProvider, null, null, null);

        // Act & Assert
        Assert.ThrowsExactly<ComponentDataProviderException>(() => factory.GetProcessors());
    }

    [TestMethod]
    public void ComponentDataFactory_GetSystem_PropagatesProviderException()
    {
        // Arrange
        var throwingProvider = new ThrowingComponentDataProvider();
        var factory = new ComponentDataFactory(null, throwingProvider, null, null);

        // Act & Assert
        Assert.ThrowsExactly<ComponentDataProviderException>(() => factory.GetSystem());
    }

    [TestMethod]
    public void ComponentDataFactory_GetMemory_PropagatesProviderException()
    {
        // Arrange
        var throwingProvider = new ThrowingComponentDataProvider();
        var factory = new ComponentDataFactory(null, null, throwingProvider, null);

        // Act & Assert
        Assert.ThrowsExactly<ComponentDataProviderException>(() => factory.GetMemory());
    }

    [TestMethod]
    public void ComponentDataFactory_GetDisks_PropagatesProviderException()
    {
        // Arrange
        var throwingProvider = new ThrowingComponentDataProvider();
        var factory = new ComponentDataFactory(null, null, null, throwingProvider);

        // Act & Assert
        Assert.ThrowsExactly<ComponentDataProviderException>(() => factory.GetDisks());
    }

    [TestMethod]
    public void AssetIdFactory_Create_PropagatesMetadataProviderException()
    {
        // Arrange
        var throwingProvider = new ThrowingAssetMetadataProvider();
        var factory = new AssetIdFactory(throwingProvider, null, null);

        // Act & Assert
        var exception = Assert.ThrowsExactly<ComponentDataProviderException>(() => factory.Create());

        Assert.AreEqual("TestMetadataProvider", exception.ProviderName);
        Assert.IsTrue(exception.Message.Contains("Simulated metadata provider failure"));
    }

    [TestMethod]
    public void ComponentDataProviderException_PreservesInnerException()
    {
        // Arrange
        var innerException = new InvalidOperationException("Inner error");
        var throwingProvider = new ThrowingComponentDataProviderWithInner(innerException);
        var factory = new ComponentDataFactory(throwingProvider, null, null, null);

        // Act & Assert
        var exception = Assert.ThrowsExactly<ComponentDataProviderException>(() => factory.GetProcessors());

        Assert.AreSame(innerException, exception.InnerException);
    }

    [TestMethod]
    public void ComponentDataProviderException_ContainsProviderName()
    {
        // Arrange
        var throwingProvider = new ThrowingComponentDataProvider();
        var factory = new ComponentDataFactory(throwingProvider, null, null, null);

        // Act & Assert
        var exception = Assert.ThrowsExactly<ComponentDataProviderException>(() => factory.GetProcessors());

        Assert.IsNotNull(exception.ProviderName);
        Assert.AreEqual("TestProvider", exception.ProviderName);
    }

    [TestMethod]
    public void ComponentDataProviderException_WithWmiClassName_PopulatesProperty()
    {
        // Arrange
        var throwingProvider = new ThrowingComponentDataProviderWithWmi();
        var factory = new ComponentDataFactory(throwingProvider, null, null, null);

        // Act & Assert
        var exception = Assert.ThrowsExactly<ComponentDataProviderException>(() => factory.GetProcessors());

        Assert.AreEqual("Win32_TestClass", exception.WmiClassName);
    }

    [TestMethod]
    public void AssetIdFactory_Create_WithBaseboardProviderException_Throws()
    {
        // Arrange
        var throwingProvider = new ThrowingAssetMetadataProvider();
        var factory = new AssetIdFactory(null, throwingProvider, null);

        // Act & Assert
        Assert.ThrowsExactly<ComponentDataProviderException>(() => factory.Create());
    }

    [TestMethod]
    public void AssetIdFactory_Create_WithBiosProviderException_Throws()
    {
        // Arrange
        var throwingProvider = new ThrowingAssetMetadataProvider();
        var factory = new AssetIdFactory(null, null, throwingProvider);

        // Act & Assert
        Assert.ThrowsExactly<ComponentDataProviderException>(() => factory.Create());
    }

    // Test helper classes

    private class ThrowingComponentDataProvider : IComponentDataProvider
    {
        public IReadOnlyList<ComponentData> GetData()
        {
            throw new ComponentDataProviderException("Simulated provider failure")
            {
                ProviderName = "TestProvider"
            };
        }
    }

    private class ThrowingComponentDataProviderWithInner : IComponentDataProvider
    {
        private readonly Exception _innerException;

        public ThrowingComponentDataProviderWithInner(Exception innerException)
        {
            _innerException = innerException;
        }

        public IReadOnlyList<ComponentData> GetData()
        {
            throw new ComponentDataProviderException("Simulated provider failure", _innerException)
            {
                ProviderName = "TestProvider"
            };
        }
    }

    private class ThrowingComponentDataProviderWithWmi : IComponentDataProvider
    {
        public IReadOnlyList<ComponentData> GetData()
        {
            throw new ComponentDataProviderException("Simulated WMI provider failure")
            {
                ProviderName = "TestWmiProvider",
                WmiClassName = "Win32_TestClass"
            };
        }
    }

    private class ThrowingAssetMetadataProvider : IAssetMetadataProvider
    {
        public AssetMetadata GetMetadata()
        {
            throw new ComponentDataProviderException("Simulated metadata provider failure")
            {
                ProviderName = "TestMetadataProvider"
            };
        }
    }
}
