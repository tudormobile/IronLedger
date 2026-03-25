namespace IronLedgerLib.Integration.Tests;

[TestClass]
public class DefaultTests
{
    [TestMethod]
    public void DefaultTest()
    {
        // Each test project requires at least one test to 'succeed'. this default
        // test allows this project to 'succeed' even if we filter out all integration
        // tests.
#pragma warning disable MSTEST0032 // Assertion condition is always true
        Assert.IsTrue(true);
#pragma warning restore MSTEST0032 // Assertion condition is always true
    }
}
