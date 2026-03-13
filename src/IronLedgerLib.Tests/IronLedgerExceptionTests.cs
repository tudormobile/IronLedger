namespace IronLedgerLib.Tests;

[TestClass]
public class IronLedgerExceptionTests
{
    [TestMethod]
    public void IronLedgerException_DefaultConstructor_CreatesInstance()
    {
        // Act
        var exception = new IronLedgerException();

        // Assert
        Assert.IsNotNull(exception);
    }

    [TestMethod]
    public void IronLedgerException_MessageConstructor_SetsMessage()
    {
        // Arrange
        const string expectedMessage = "Test error message";

        // Act
        var exception = new IronLedgerException(expectedMessage);

        // Assert
        Assert.AreEqual(expectedMessage, exception.Message);
    }

    [TestMethod]
    public void IronLedgerException_MessageAndInnerExceptionConstructor_SetsProperties()
    {
        // Arrange
        const string expectedMessage = "Test error message";
        var innerException = new InvalidOperationException("Inner exception");

        // Act
        var exception = new IronLedgerException(expectedMessage, innerException);

        // Assert
        Assert.AreEqual(expectedMessage, exception.Message);
        Assert.AreEqual(innerException, exception.InnerException);
    }

    [TestMethod]
    public void IronLedgerException_InheritsFromException()
    {
        // Arrange & Act
        var exception = new IronLedgerException();

        // Assert
        Assert.IsInstanceOfType<Exception>(exception);
    }
}

[TestClass]
public class ComponentDataExceptionTests
{
    [TestMethod]
    public void ComponentDataException_DefaultConstructor_CreatesInstance()
    {
        // Act
        var exception = new ComponentDataException();

        // Assert
        Assert.IsNotNull(exception);
    }

    [TestMethod]
    public void ComponentDataException_MessageConstructor_SetsMessage()
    {
        // Arrange
        const string expectedMessage = "Component data error";

        // Act
        var exception = new ComponentDataException(expectedMessage);

        // Assert
        Assert.AreEqual(expectedMessage, exception.Message);
    }

    [TestMethod]
    public void ComponentDataException_MessageAndInnerExceptionConstructor_SetsProperties()
    {
        // Arrange
        const string expectedMessage = "Component data error";
        var innerException = new InvalidOperationException("Inner exception");

        // Act
        var exception = new ComponentDataException(expectedMessage, innerException);

        // Assert
        Assert.AreEqual(expectedMessage, exception.Message);
        Assert.AreEqual(innerException, exception.InnerException);
    }

    [TestMethod]
    public void ComponentDataException_InheritsFromIronLedgerException()
    {
        // Arrange & Act
        var exception = new ComponentDataException();

        // Assert
        Assert.IsInstanceOfType<IronLedgerException>(exception);
    }
}

[TestClass]
public class ComponentDataProviderExceptionTests
{
    [TestMethod]
    public void ComponentDataProviderException_DefaultConstructor_CreatesInstance()
    {
        // Act
        var exception = new ComponentDataProviderException();

        // Assert
        Assert.IsNotNull(exception);
        Assert.IsNull(exception.ProviderName);
        Assert.IsNull(exception.WmiClassName);
    }

    [TestMethod]
    public void ComponentDataProviderException_MessageConstructor_SetsMessage()
    {
        // Arrange
        const string expectedMessage = "Provider error";

        // Act
        var exception = new ComponentDataProviderException(expectedMessage);

        // Assert
        Assert.AreEqual(expectedMessage, exception.Message);
    }

    [TestMethod]
    public void ComponentDataProviderException_MessageAndInnerExceptionConstructor_SetsProperties()
    {
        // Arrange
        const string expectedMessage = "Provider error";
        var innerException = new InvalidOperationException("Inner exception");

        // Act
        var exception = new ComponentDataProviderException(expectedMessage, innerException);

        // Assert
        Assert.AreEqual(expectedMessage, exception.Message);
        Assert.AreEqual(innerException, exception.InnerException);
    }

    [TestMethod]
    public void ComponentDataProviderException_ProviderName_CanBeSet()
    {
        // Arrange
        const string expectedProviderName = "TestProvider";

        // Act
        var exception = new ComponentDataProviderException
        {
            ProviderName = expectedProviderName
        };

        // Assert
        Assert.AreEqual(expectedProviderName, exception.ProviderName);
    }

    [TestMethod]
    public void ComponentDataProviderException_WmiClassName_CanBeSet()
    {
        // Arrange
        const string expectedWmiClassName = "Win32_Processor";

        // Act
        var exception = new ComponentDataProviderException
        {
            WmiClassName = expectedWmiClassName
        };

        // Assert
        Assert.AreEqual(expectedWmiClassName, exception.WmiClassName);
    }

    [TestMethod]
    public void ComponentDataProviderException_AllProperties_CanBeSetTogether()
    {
        // Arrange
        const string expectedMessage = "Provider failed to retrieve data";
        const string expectedProviderName = "ProcessorDataProvider";
        const string expectedWmiClassName = "Win32_Processor";
        var innerException = new InvalidOperationException("WMI query failed");

        // Act
        var exception = new ComponentDataProviderException(expectedMessage, innerException)
        {
            ProviderName = expectedProviderName,
            WmiClassName = expectedWmiClassName
        };

        // Assert
        Assert.AreEqual(expectedMessage, exception.Message);
        Assert.AreEqual(innerException, exception.InnerException);
        Assert.AreEqual(expectedProviderName, exception.ProviderName);
        Assert.AreEqual(expectedWmiClassName, exception.WmiClassName);
    }

    [TestMethod]
    public void ComponentDataProviderException_InheritsFromComponentDataException()
    {
        // Arrange & Act
        var exception = new ComponentDataProviderException();

        // Assert
        Assert.IsInstanceOfType<ComponentDataException>(exception);
        Assert.IsInstanceOfType<IronLedgerException>(exception);
    }
}
