namespace IronLedgerLib.Services.Tests;

[TestClass]
public class IronLedgerClientOptionsTests
{
    [TestMethod]
    public void IronLedgerClientOptions_DefaultServerUrl_IsNull()
        => Assert.IsNull(new IronLedgerClientOptions().ServerUrl);

    [TestMethod]
    public void IronLedgerClientOptions_ServerUrl_CanBeSetAndRetrieved()
    {
        var url = new Uri("https://myserver:5037");
        var options = new IronLedgerClientOptions { ServerUrl = url };

        Assert.AreEqual(url, options.ServerUrl);
    }
}
