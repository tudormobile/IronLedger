using Tudormobile.IronLedgerLib.Providers;

namespace IronLedgerLib.Tests.Providers;

[TestClass]
[TestCategory("Integration")]
public class ProcessorDataProviderTests
{
    [TestMethod]
    public void ProcessorDataProvider_ImplementsInterface()
    {
        // Arrange
        var dataProvider = new ProcessorDataProvider();

        // Assert
        Assert.IsInstanceOfType<IComponentDataProvider>(dataProvider);
    }

    [TestMethod]
    public void ProcessorDataProvider_GetData_ReturnsData()
    {
        // Arrange
        var dataProvider = new ProcessorDataProvider();

        // Act
        var data = dataProvider.GetData();

        // Assert
        Assert.IsNotEmpty(data);    // Must have some memory!
        Assert.IsNotEmpty(data[0].Properties);  // Must have some properties!
    }
}
