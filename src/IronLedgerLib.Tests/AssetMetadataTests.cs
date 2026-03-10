namespace IronLedgerLib.Tests;

[TestClass]
public class AssetMetadataTests
{
    [TestMethod]
    public void AssetMetadata_Empty_ReturnsEmptyMetadata()
    {
        // Arrange and Act
        var metadata = AssetMetadata.Empty;

        // Assert
        Assert.IsEmpty(metadata.SerialNumber);
        Assert.IsEmpty(metadata.Manufacturer);
        Assert.IsEmpty(metadata.Product);
    }

    [TestMethod]
    public void AssetMetadata_CanBeCreated_WithRequiredProperties()
    {
        // Arrange & Act
        var metadata = new AssetMetadata
        {
            SerialNumber = "SN123456",
            Manufacturer = "Dell",
            Product = "XPS 15"
        };

        // Assert
        Assert.IsNotNull(metadata);
        Assert.AreEqual("SN123456", metadata.SerialNumber);
        Assert.AreEqual("Dell", metadata.Manufacturer);
        Assert.AreEqual("XPS 15", metadata.Product);
    }

    [TestMethod]
    public void AssetMetadata_SupportsRecordEquality_WhenAllPropertiesMatch()
    {
        // Arrange
        var metadata1 = new AssetMetadata
        {
            SerialNumber = "SN123456",
            Manufacturer = "Dell",
            Product = "XPS 15"
        };

        var metadata2 = new AssetMetadata
        {
            SerialNumber = "SN123456",
            Manufacturer = "Dell",
            Product = "XPS 15"
        };

        // Act & Assert
        Assert.AreEqual(metadata1, metadata2);
        Assert.IsTrue(metadata1 == metadata2);
        Assert.IsFalse(metadata1 != metadata2);
    }

    [TestMethod]
    public void AssetMetadata_RecordInequality_WhenSerialNumberDiffers()
    {
        // Arrange
        var metadata1 = new AssetMetadata
        {
            SerialNumber = "SN123456",
            Manufacturer = "Dell",
            Product = "XPS 15"
        };

        var metadata2 = new AssetMetadata
        {
            SerialNumber = "SN999999",
            Manufacturer = "Dell",
            Product = "XPS 15"
        };

        // Act & Assert
        Assert.AreNotEqual(metadata1, metadata2);
        Assert.IsFalse(metadata1 == metadata2);
        Assert.IsTrue(metadata1 != metadata2);
    }

    [TestMethod]
    public void AssetMetadata_RecordInequality_WhenManufacturerDiffers()
    {
        // Arrange
        var metadata1 = new AssetMetadata
        {
            SerialNumber = "SN123456",
            Manufacturer = "Dell",
            Product = "XPS 15"
        };

        var metadata2 = new AssetMetadata
        {
            SerialNumber = "SN123456",
            Manufacturer = "HP",
            Product = "XPS 15"
        };

        // Act & Assert
        Assert.AreNotEqual(metadata1, metadata2);
    }

    [TestMethod]
    public void AssetMetadata_RecordInequality_WhenProductDiffers()
    {
        // Arrange
        var metadata1 = new AssetMetadata
        {
            SerialNumber = "SN123456",
            Manufacturer = "Dell",
            Product = "XPS 15"
        };

        var metadata2 = new AssetMetadata
        {
            SerialNumber = "SN123456",
            Manufacturer = "Dell",
            Product = "XPS 13"
        };

        // Act & Assert
        Assert.AreNotEqual(metadata1, metadata2);
    }

    [TestMethod]
    public void AssetMetadata_GetHashCode_IsConsistent()
    {
        // Arrange
        var metadata = new AssetMetadata
        {
            SerialNumber = "SN123456",
            Manufacturer = "Dell",
            Product = "XPS 15"
        };

        // Act
        var hashCode1 = metadata.GetHashCode();
        var hashCode2 = metadata.GetHashCode();

        // Assert
        Assert.AreEqual(hashCode1, hashCode2);
    }

    [TestMethod]
    public void AssetMetadata_GetHashCode_IsSameForEqualRecords()
    {
        // Arrange
        var metadata1 = new AssetMetadata
        {
            SerialNumber = "SN123456",
            Manufacturer = "Dell",
            Product = "XPS 15"
        };

        var metadata2 = new AssetMetadata
        {
            SerialNumber = "SN123456",
            Manufacturer = "Dell",
            Product = "XPS 15"
        };

        // Act & Assert
        Assert.AreEqual(metadata1.GetHashCode(), metadata2.GetHashCode());
    }

    [TestMethod]
    public void AssetMetadata_ToString_ReturnsFormattedString()
    {
        // Arrange
        var metadata = new AssetMetadata
        {
            SerialNumber = "SN123456",
            Manufacturer = "Dell",
            Product = "XPS 15"
        };

        // Act
        var result = metadata.ToString();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Contains("SN123456"));
        Assert.IsTrue(result.Contains("Dell"));
        Assert.IsTrue(result.Contains("XPS 15"));
    }

    [TestMethod]
    public void AssetMetadata_WithClause_CreatesNewInstance()
    {
        // Arrange
        var original = new AssetMetadata
        {
            SerialNumber = "SN123456",
            Manufacturer = "Dell",
            Product = "XPS 15"
        };

        // Act
        var modified = original with { SerialNumber = "SN999999" };

        // Assert
        Assert.AreNotEqual(original, modified);
        Assert.AreEqual("SN123456", original.SerialNumber);
        Assert.AreEqual("SN999999", modified.SerialNumber);
        Assert.AreEqual(original.Manufacturer, modified.Manufacturer);
        Assert.AreEqual(original.Product, modified.Product);
    }

    [TestMethod]
    public void AssetMetadata_WithClause_CanModifyManufacturer()
    {
        // Arrange
        var original = new AssetMetadata
        {
            SerialNumber = "SN123456",
            Manufacturer = "Dell",
            Product = "XPS 15"
        };

        // Act
        var modified = original with { Manufacturer = "HP" };

        // Assert
        Assert.AreNotEqual(original, modified);
        Assert.AreEqual("Dell", original.Manufacturer);
        Assert.AreEqual("HP", modified.Manufacturer);
    }

    [TestMethod]
    public void AssetMetadata_WithClause_CanModifyProduct()
    {
        // Arrange
        var original = new AssetMetadata
        {
            SerialNumber = "SN123456",
            Manufacturer = "Dell",
            Product = "XPS 15"
        };

        // Act
        var modified = original with { Product = "Latitude 7490" };

        // Assert
        Assert.AreNotEqual(original, modified);
        Assert.AreEqual("XPS 15", original.Product);
        Assert.AreEqual("Latitude 7490", modified.Product);
    }

    [TestMethod]
    public void AssetMetadata_WithClause_CanModifyMultipleProperties()
    {
        // Arrange
        var original = new AssetMetadata
        {
            SerialNumber = "SN123456",
            Manufacturer = "Dell",
            Product = "XPS 15"
        };

        // Act
        var modified = original with
        {
            SerialNumber = "SN999999",
            Manufacturer = "HP"
        };

        // Assert
        Assert.AreNotEqual(original, modified);
        Assert.AreEqual("SN123456", original.SerialNumber);
        Assert.AreEqual("SN999999", modified.SerialNumber);
        Assert.AreEqual("Dell", original.Manufacturer);
        Assert.AreEqual("HP", modified.Manufacturer);
        Assert.AreEqual(original.Product, modified.Product);
    }

    [TestMethod]
    public void AssetMetadata_CanHandleEmptyStrings()
    {
        // Arrange & Act
        var metadata = new AssetMetadata
        {
            SerialNumber = "",
            Manufacturer = "",
            Product = ""
        };

        // Assert
        Assert.IsNotNull(metadata);
        Assert.AreEqual("", metadata.SerialNumber);
        Assert.AreEqual("", metadata.Manufacturer);
        Assert.AreEqual("", metadata.Product);
    }

    [TestMethod]
    public void AssetMetadata_CanHandleWhitespaceStrings()
    {
        // Arrange & Act
        var metadata = new AssetMetadata
        {
            SerialNumber = "   ",
            Manufacturer = "\t",
            Product = "\n"
        };

        // Assert
        Assert.IsNotNull(metadata);
        Assert.AreEqual("   ", metadata.SerialNumber);
        Assert.AreEqual("\t", metadata.Manufacturer);
        Assert.AreEqual("\n", metadata.Product);
    }

    [TestMethod]
    public void AssetMetadata_CanHandleSpecialCharacters()
    {
        // Arrange & Act
        var metadata = new AssetMetadata
        {
            SerialNumber = "SN-123/456#789",
            Manufacturer = "Dell & Associates (USA)",
            Product = "XPS 15\" Model"
        };

        // Assert
        Assert.IsNotNull(metadata);
        Assert.AreEqual("SN-123/456#789", metadata.SerialNumber);
        Assert.AreEqual("Dell & Associates (USA)", metadata.Manufacturer);
        Assert.AreEqual("XPS 15\" Model", metadata.Product);
    }

    [TestMethod]
    public void AssetMetadata_CanHandleUnicodeCharacters()
    {
        // Arrange & Act
        var metadata = new AssetMetadata
        {
            SerialNumber = "序列号123",
            Manufacturer = "联想",
            Product = "ThinkPad™ X1 Carbon®"
        };

        // Assert
        Assert.IsNotNull(metadata);
        Assert.AreEqual("序列号123", metadata.SerialNumber);
        Assert.AreEqual("联想", metadata.Manufacturer);
        Assert.AreEqual("ThinkPad™ X1 Carbon®", metadata.Product);
    }

    [TestMethod]
    public void AssetMetadata_CanHandleLongStrings()
    {
        // Arrange
        var longSerialNumber = new string('A', 1000);
        var longManufacturer = new string('B', 1000);
        var longProduct = new string('C', 1000);

        // Act
        var metadata = new AssetMetadata
        {
            SerialNumber = longSerialNumber,
            Manufacturer = longManufacturer,
            Product = longProduct
        };

        // Assert
        Assert.IsNotNull(metadata);
        Assert.AreEqual(1000, metadata.SerialNumber.Length);
        Assert.AreEqual(1000, metadata.Manufacturer.Length);
        Assert.AreEqual(1000, metadata.Product.Length);
    }

    [TestMethod]
    public void AssetMetadata_PropertiesAreImmutable()
    {
        // Arrange
        var metadata = new AssetMetadata
        {
            SerialNumber = "SN123456",
            Manufacturer = "Dell",
            Product = "XPS 15"
        };

        // Act - Attempting to modify should require 'with' clause
        // This is a compile-time test, verifying that properties have 'init' accessor
        var modified = metadata with { SerialNumber = "NewSN" };

        // Assert
        Assert.AreEqual("SN123456", metadata.SerialNumber);
        Assert.AreEqual("NewSN", modified.SerialNumber);
    }

    [TestMethod]
    public void AssetMetadata_CanBeUsedInCollections()
    {
        // Arrange
        var metadata1 = new AssetMetadata
        {
            SerialNumber = "SN123456",
            Manufacturer = "Dell",
            Product = "XPS 15"
        };

        var metadata2 = new AssetMetadata
        {
            SerialNumber = "SN123456",
            Manufacturer = "Dell",
            Product = "XPS 15"
        };

        var metadata3 = new AssetMetadata
        {
            SerialNumber = "SN999999",
            Manufacturer = "HP",
            Product = "Elite"
        };

        // Act
        var set = new HashSet<AssetMetadata> { metadata1, metadata2, metadata3 };
        var dict = new Dictionary<AssetMetadata, string>
        {
            { metadata1, "First" },
            { metadata3, "Second" }
        };

        // Assert
        Assert.AreEqual(2, set.Count); // metadata1 and metadata2 are equal
        Assert.IsTrue(set.Contains(metadata1));
        Assert.IsTrue(set.Contains(metadata2));
        Assert.IsTrue(set.Contains(metadata3));
        Assert.AreEqual("First", dict[metadata2]); // Can lookup by equal key
    }

    [TestMethod]
    public void AssetMetadata_ReferenceEquality_DifferentInstances()
    {
        // Arrange
        var metadata1 = new AssetMetadata
        {
            SerialNumber = "SN123456",
            Manufacturer = "Dell",
            Product = "XPS 15"
        };

        var metadata2 = new AssetMetadata
        {
            SerialNumber = "SN123456",
            Manufacturer = "Dell",
            Product = "XPS 15"
        };

        // Act & Assert
        Assert.IsFalse(ReferenceEquals(metadata1, metadata2)); // Different instances
        Assert.AreEqual(metadata1, metadata2); // But value-equal
    }

    [TestMethod]
    public void AssetMetadata_ReferenceEquality_SameInstance()
    {
        // Arrange
        var metadata = new AssetMetadata
        {
            SerialNumber = "SN123456",
            Manufacturer = "Dell",
            Product = "XPS 15"
        };

        // Act
        var sameReference = metadata;

        // Assert
        Assert.IsTrue(ReferenceEquals(metadata, sameReference));
        Assert.AreEqual(metadata, sameReference);
    }
}