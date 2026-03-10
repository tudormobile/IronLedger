using Tudormobile.IronLedgerLib.Providers;

namespace IronLedgerLib.Tests.Providers;

[TestClass]
public class MemoryDataProviderTests
{
    [TestMethod]
    public void MemoryDataProvider_ImplementsInterface()
    {
        // Arrange
        var dataProvider = new MemoryDataProvider();

        // Assert
        Assert.IsInstanceOfType<IComponentDataProvider>(dataProvider);
    }

    [TestMethod]
    public void MemoryDataProvider_GetData_ReturnsData()
    {
        // Arrange
        var dataProvider = new MemoryDataProvider();

        // Act
        var data = dataProvider.GetData();

        // Assert
        Assert.IsNotEmpty(data);    // Must have some memory!
        Assert.IsNotEmpty(data[0].Properties);  // Must have some properties!
    }
}
