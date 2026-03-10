using Tudormobile.IronLedgerLib.Providers;

namespace IronLedgerLib.Tests.Providers;

[TestClass]
public class DiskDataProviderTests
{
    [TestMethod]
    public void DiskDataProvider_ImplementsInterface()
    {
        // Arrange
        var dataProvider = new DiskDataProvider();

        // Assert
        Assert.IsInstanceOfType<IComponentDataProvider>(dataProvider);
    }

    [TestMethod]
    public void DiskDataProvider_GetData_ReturnsData()
    {
        // Arrange
        var dataProvider = new DiskDataProvider();

        // Act
        var data = dataProvider.GetData();

        // Assert
        Assert.IsNotEmpty(data);    // Must have some memory!
        Assert.IsNotEmpty(data[0].Properties);  // Must have some properties!
    }
}
