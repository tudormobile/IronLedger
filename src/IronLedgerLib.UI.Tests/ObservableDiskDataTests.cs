namespace IronLedgerLib.UI.Tests;

[TestClass]
public class ObservableDiskDataTests
{
    [TestMethod]
    public void ObservableDiskData_Constructor_SetsDefaultProperties()
    {
        // Arrange
        var diskData = new ObservableDiskData();

        // Assert
        Assert.IsEmpty(diskData);
    }
}

