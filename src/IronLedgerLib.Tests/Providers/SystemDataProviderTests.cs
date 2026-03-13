using Tudormobile.IronLedgerLib.Providers;

namespace IronLedgerLib.Tests.Providers;

[TestClass]
[TestCategory("Integration")]
public class SystemDataProviderTests
{
    [TestMethod]
    public void SystemDataProvider_ImplementsInterface()
    {
        // Arrange
        var dataProvider = new SystemDataProvider();

        // Assert
        Assert.IsInstanceOfType<IComponentDataProvider>(dataProvider);
    }

    [TestMethod]
    public void SystemDataProvider_GetData_ReturnsData()
    {
        // Arrange
        var dataProvider = new SystemDataProvider();

        // Act
        var data = dataProvider.GetData();

        // Assert
        Assert.IsNotEmpty(data);    // Must have some memory!
        Assert.IsNotEmpty(data[0].Properties);  // Must have some properties!
    }
}
