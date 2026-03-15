namespace IronLedgerLib.UI.Tests;

[TestClass]
public class ObservableMemoryDataTests
{
    [TestMethod]
    public void ObservableMemoryData_Constructor_SetsDefaultProperties()
    {
        // Arrange
        var memoryData = new ObservableMemoryData();

        // Assert
        Assert.IsEmpty(memoryData);
    }
}

