namespace IronLedgerLib.Tests;

[TestClass]
public class ComponentPropertyTests
{
    [TestMethod]
    public void ComponentProperty_CanBeCreated_WithNameAndValue()
    {
        // Arrange & Act
        var property = new ComponentProperty("CPU Speed", "3.5 GHz");

        // Assert
        Assert.IsNotNull(property);
        Assert.AreEqual("CPU Speed", property.Name);
        Assert.AreEqual("3.5 GHz", property.Value);
    }

    [TestMethod]
    public void ComponentProperty_SupportsRecordEquality_WhenBothPropertiesMatch()
    {
        // Arrange
        var property1 = new ComponentProperty("CPU Speed", "3.5 GHz");
        var property2 = new ComponentProperty("CPU Speed", "3.5 GHz");

        // Assert
        Assert.AreEqual(property1, property2);
        Assert.IsTrue(property1 == property2);
        Assert.IsFalse(property1 != property2);
    }

    [TestMethod]
    public void ComponentProperty_SupportsRecordInequality_WhenNameDiffers()
    {
        // Arrange
        var property1 = new ComponentProperty("CPU Speed", "3.5 GHz");
        var property2 = new ComponentProperty("Processor Speed", "3.5 GHz");

        // Assert
        Assert.AreNotEqual(property1, property2);
        Assert.IsFalse(property1 == property2);
        Assert.IsTrue(property1 != property2);
    }

    [TestMethod]
    public void ComponentProperty_SupportsRecordInequality_WhenValueDiffers()
    {
        // Arrange
        var property1 = new ComponentProperty("CPU Speed", "3.5 GHz");
        var property2 = new ComponentProperty("CPU Speed", "4.0 GHz");

        // Assert
        Assert.AreNotEqual(property1, property2);
        Assert.IsFalse(property1 == property2);
        Assert.IsTrue(property1 != property2);
    }

    [TestMethod]
    public void ComponentProperty_CanBeCreated_WithEmptyStrings()
    {
        // Arrange & Act
        var property = new ComponentProperty(string.Empty, string.Empty);

        // Assert
        Assert.IsNotNull(property);
        Assert.AreEqual(string.Empty, property.Name);
        Assert.AreEqual(string.Empty, property.Value);
    }

    [TestMethod]
    public void ComponentProperty_HasSameHashCode_ForEqualInstances()
    {
        // Arrange
        var property1 = new ComponentProperty("CPU Speed", "3.5 GHz");
        var property2 = new ComponentProperty("CPU Speed", "3.5 GHz");

        // Act & Assert
        Assert.AreEqual(property1.GetHashCode(), property2.GetHashCode());
    }

    [TestMethod]
    public void ComponentProperty_SupportsDeconstruction()
    {
        // Arrange
        var property = new ComponentProperty("CPU Speed", "3.5 GHz");

        // Act
        var (name, value) = property;

        // Assert
        Assert.AreEqual("CPU Speed", name);
        Assert.AreEqual("3.5 GHz", value);
    }

    [TestMethod]
    public void ComponentProperty_ToString_ReturnsExpectedFormat()
    {
        // Arrange
        var property = new ComponentProperty("CPU Speed", "3.5 GHz");

        // Act
        var result = property.ToString();

        // Assert
        Assert.IsTrue(result.Contains("CPU Speed"));
        Assert.IsTrue(result.Contains("3.5 GHz"));
    }

    [TestMethod]
    public void ComponentProperty_WithExpression_CreatesNewInstance()
    {
        // Arrange
        var original = new ComponentProperty("CPU Speed", "3.5 GHz");

        // Act
        var modified = original with { Value = "4.0 GHz", Name = "CPU Speed2" };

        // Assert
        Assert.AreNotEqual(original, modified);
        Assert.AreEqual("CPU Speed2", modified.Name);
        Assert.AreEqual("4.0 GHz", modified.Value);
        Assert.AreEqual("3.5 GHz", original.Value);
    }
}
