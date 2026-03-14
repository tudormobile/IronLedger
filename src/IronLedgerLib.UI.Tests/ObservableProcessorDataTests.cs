namespace IronLedgerLib.UI.Tests;

[TestClass]
public class ObservableProcessorDataTests
{
    [TestMethod]
    public void ObservableProcessorData_Constructor_SetsDefaultProperties()
    {
        // Arrange
        var processorData = new ObservableProcessorData();

        // Assert
        Assert.IsEmpty(processorData);
    }
}

