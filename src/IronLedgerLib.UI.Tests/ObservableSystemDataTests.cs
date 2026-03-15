namespace IronLedgerLib.UI.Tests;

[TestClass]
public class ObservableSystemDataTests
{
    [TestMethod]
    public void ObservableSystemData_Constructor_SetsDefaultProperties()
    {
        // Arrange
        var systemData = new ObservableSystemData();

        // Assert
        Assert.AreEqual(ComponentData.Empty, systemData.Data);
        Assert.AreEqual(string.Empty, systemData.DisplayName);
        Assert.AreEqual(string.Empty, systemData.Description);
        Assert.AreEqual(string.Empty, systemData.Notes);
    }
}
