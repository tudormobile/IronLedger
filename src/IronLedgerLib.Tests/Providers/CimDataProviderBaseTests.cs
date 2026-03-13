using System.Diagnostics.CodeAnalysis;
using Tudormobile.IronLedgerLib.Providers;

namespace IronLedgerLib.Tests.Providers;

[TestClass]
public class CimDataProviderBaseTests
{
    [TestMethod]
    public void CimDataProviderBase_WhenProviderThrows_ThrowsException()
    {
        // Arrange
        var provider = new TestCimDataProvider();

        // Act & Assert
        var exception = Assert.ThrowsExactly<ComponentDataProviderException>(() => provider.GetData());
        Assert.IsNotNull(exception.InnerException);
        Assert.IsInstanceOfType<NotImplementedException>(exception.InnerException);
        Assert.Contains(nameof(TestCimDataProvider), exception.Message);
    }

    [ExcludeFromCodeCoverage]
    internal class TestCimDataProvider : CimDataProviderBase
    {
        protected override string WmiClassName => nameof(TestCimDataProvider);

        protected override string CaptionProperty => throw new NotImplementedException();

        protected override string[] ComponentPropertyNames => throw new NotImplementedException();
    }
}
