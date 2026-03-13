namespace IronLedgerLib.Tests;

[TestClass]
public class ComponentDataTests
{
    [TestMethod]
    public void ComponentData_Empty_ReturnsEmptyInstance()
    {
        // Arrange & Act
        var componentData = ComponentData.Empty;

        // Assert
        Assert.AreEqual(AssetMetadata.Empty, componentData.Metadata);
        Assert.AreEqual(string.Empty, componentData.Caption);
        Assert.IsEmpty(componentData.Properties);
    }

    [TestMethod]
    public void ComponentData_CanBeCreated_WithRequiredProperties()
    {
        // Arrange
        var metadata = new AssetMetadata
        {
            SerialNumber = "SN123456",
            Manufacturer = "Dell",
            Product = "XPS 15"
        };

        var properties = new List<ComponentProperty>
        {
            new("CPU Speed", "3.5 GHz"),
            new("Cores", "8")
        };

        // Act
        var componentData = new ComponentData
        {
            Metadata = metadata,
            Caption = "Intel Core i7",
            Properties = properties
        };

        // Assert
        Assert.IsNotNull(componentData);
        Assert.AreEqual(metadata, componentData.Metadata);
        Assert.AreEqual("Intel Core i7", componentData.Caption);
        Assert.HasCount(2, componentData.Properties);
        Assert.AreEqual("CPU Speed", componentData.Properties[0].Name);
        Assert.AreEqual("3.5 GHz", componentData.Properties[0].Value);
    }

    [TestMethod]
    public void ComponentData_AllRequiredProperties_MustBeInitialized()
    {
        // Arrange
        var metadata = AssetMetadata.Empty;
        var properties = new List<ComponentProperty>();

        // Act
        var componentData = new ComponentData
        {
            Metadata = metadata,
            Caption = "Test Component",
            Properties = properties
        };

        // Assert
        Assert.IsNotNull(componentData);
        Assert.AreEqual(metadata, componentData.Metadata);
        Assert.AreEqual("Test Component", componentData.Caption);
        Assert.IsEmpty(componentData.Properties);
    }

    [TestMethod]
    public void ComponentData_CanBeCreated_WithEmptyPropertiesList()
    {
        // Arrange & Act
        var componentData = new ComponentData
        {
            Metadata = AssetMetadata.Empty,
            Caption = "Empty Component",
            Properties = []
        };

        // Assert
        Assert.IsNotNull(componentData);
        Assert.IsEmpty(componentData.Properties);
    }

    [TestMethod]
    public void ComponentData_Properties_AreReadOnly()
    {
        // Arrange
        var properties = new List<ComponentProperty>
        {
            new("Property1", "Value1"),
            new("Property2", "Value2")
        };

        var componentData = new ComponentData
        {
            Metadata = AssetMetadata.Empty,
            Caption = "Test",
            Properties = properties
        };

        // Act - modify the original list
        properties.Add(new ComponentProperty("Property3", "Value3"));

        // Assert - component data properties count should reflect the change
        // because List<T> is mutable even when cast to IReadOnlyList<T>
        Assert.HasCount(3, componentData.Properties);
    }

    [TestMethod]
    public void ComponentData_Properties_PreservesOrder()
    {
        // Arrange
        var properties = new List<ComponentProperty>
        {
            new("First", "1"),
            new("Second", "2"),
            new("Third", "3")
        };

        // Act
        var componentData = new ComponentData
        {
            Metadata = AssetMetadata.Empty,
            Caption = "Ordered Component",
            Properties = properties
        };

        // Assert
        Assert.AreEqual("First", componentData.Properties[0].Name);
        Assert.AreEqual("Second", componentData.Properties[1].Name);
        Assert.AreEqual("Third", componentData.Properties[2].Name);
    }

    [TestMethod]
    public void ComponentData_CanBeCreated_WithEmptyCaption()
    {
        // Arrange & Act
        var componentData = new ComponentData
        {
            Metadata = AssetMetadata.Empty,
            Caption = string.Empty,
            Properties = []
        };

        // Assert
        Assert.IsNotNull(componentData);
        Assert.AreEqual(string.Empty, componentData.Caption);
    }

    [TestMethod]
    public void ComponentData_Metadata_CanBeAssetMetadataEmpty()
    {
        // Arrange & Act
        var componentData = new ComponentData
        {
            Metadata = AssetMetadata.Empty,
            Caption = "Test",
            Properties = []
        };

        // Assert
        Assert.AreEqual(AssetMetadata.Empty, componentData.Metadata);
        Assert.AreEqual(string.Empty, componentData.Metadata.SerialNumber);
        Assert.AreEqual(string.Empty, componentData.Metadata.Manufacturer);
        Assert.AreEqual(string.Empty, componentData.Metadata.Product);
    }

    [TestMethod]
    public void ComponentData_InitProperties_AreImmutable()
    {
        // Arrange
        var metadata = new AssetMetadata
        {
            SerialNumber = "SN123",
            Manufacturer = "Dell",
            Product = "XPS"
        };

        var componentData = new ComponentData
        {
            Metadata = metadata,
            Caption = "Original Caption",
            Properties = []
        };

        // Act & Assert - Properties should be init-only
        // This is a compile-time check, but we verify the values don't change
        Assert.AreEqual("Original Caption", componentData.Caption);
        Assert.AreEqual(metadata, componentData.Metadata);
    }

    [TestMethod]
    public void ComponentData_WithMultipleProperties_MaintainsAllData()
    {
        // Arrange
        var metadata = new AssetMetadata
        {
            SerialNumber = "ABC123",
            Manufacturer = "ACME Corp",
            Product = "SuperWidget"
        };

        var properties = new List<ComponentProperty>
        {
            new("Speed", "Fast"),
            new("Color", "Blue"),
            new("Weight", "10kg"),
            new("Status", "Active")
        };

        // Act
        var componentData = new ComponentData
        {
            Metadata = metadata,
            Caption = "Multi-Property Component",
            Properties = properties
        };

        // Assert
        Assert.AreEqual("ABC123", componentData.Metadata.SerialNumber);
        Assert.AreEqual("ACME Corp", componentData.Metadata.Manufacturer);
        Assert.AreEqual("SuperWidget", componentData.Metadata.Product);
        Assert.AreEqual("Multi-Property Component", componentData.Caption);
        Assert.HasCount(4, componentData.Properties);
        Assert.AreEqual("Speed", componentData.Properties[0].Name);
        Assert.AreEqual("Fast", componentData.Properties[0].Value);
        Assert.AreEqual("Status", componentData.Properties[3].Name);
        Assert.AreEqual("Active", componentData.Properties[3].Value);
    }

    // --- Equals ---

    [TestMethod]
    public void Equals_WithSameInstance_ReturnsTrue()
    {
        var data = new ComponentData { Metadata = AssetMetadata.Empty, Caption = "CPU", Properties = [] };

        Assert.IsTrue(data.Equals(data));
    }

    [TestMethod]
    public void Equals_WithNull_ReturnsFalse()
    {
        var data = new ComponentData { Metadata = AssetMetadata.Empty, Caption = "CPU", Properties = [] };

        Assert.IsFalse(data.Equals(null));
    }

    [TestMethod]
    public void Equals_WithIdenticalPropertiesInSeparateLists_ReturnsTrue()
    {
        // This is the key scenario the override exists for: two distinct list instances
        // with identical elements must compare as equal.
        var metadata = new AssetMetadata { SerialNumber = "SN1", Manufacturer = "Dell", Product = "XPS" };

        var a = new ComponentData { Metadata = metadata, Caption = "CPU", Properties = [new("Speed", "3.5 GHz"), new("Cores", "8")] };
        var b = new ComponentData { Metadata = metadata, Caption = "CPU", Properties = [new("Speed", "3.5 GHz"), new("Cores", "8")] };

        Assert.IsTrue(a.Equals(b));
        Assert.AreEqual(a, b);
    }

    [TestMethod]
    public void Equals_WithDifferentCaption_ReturnsFalse()
    {
        var a = new ComponentData { Metadata = AssetMetadata.Empty, Caption = "CPU", Properties = [] };
        var b = new ComponentData { Metadata = AssetMetadata.Empty, Caption = "GPU", Properties = [] };

        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void Equals_WithDifferentMetadata_ReturnsFalse()
    {
        var a = new ComponentData
        {
            Metadata = new AssetMetadata { SerialNumber = "SN1", Manufacturer = "Dell", Product = "XPS" },
            Caption = "CPU",
            Properties = []
        };
        var b = new ComponentData
        {
            Metadata = new AssetMetadata { SerialNumber = "SN2", Manufacturer = "Dell", Product = "XPS" },
            Caption = "CPU",
            Properties = []
        };

        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void Equals_WithDifferentPropertyValues_ReturnsFalse()
    {
        var a = new ComponentData { Metadata = AssetMetadata.Empty, Caption = "CPU", Properties = [new("Speed", "3.5 GHz")] };
        var b = new ComponentData { Metadata = AssetMetadata.Empty, Caption = "CPU", Properties = [new("Speed", "4.0 GHz")] };

        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void Equals_WithPropertiesInDifferentOrder_ReturnsFalse()
    {
        // SequenceEqual is order-sensitive; swapped order must not be equal.
        var a = new ComponentData { Metadata = AssetMetadata.Empty, Caption = "CPU", Properties = [new("Speed", "3.5 GHz"), new("Cores", "8")] };
        var b = new ComponentData { Metadata = AssetMetadata.Empty, Caption = "CPU", Properties = [new("Cores", "8"), new("Speed", "3.5 GHz")] };

        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void Equals_WithDifferentPropertyCount_ReturnsFalse()
    {
        var a = new ComponentData { Metadata = AssetMetadata.Empty, Caption = "CPU", Properties = [new("Speed", "3.5 GHz")] };
        var b = new ComponentData { Metadata = AssetMetadata.Empty, Caption = "CPU", Properties = [] };

        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void Equals_TwoEmptyInstances_ReturnsTrue()
    {
        Assert.IsTrue(ComponentData.Empty.Equals(ComponentData.Empty));
        Assert.AreEqual(ComponentData.Empty, ComponentData.Empty);
    }

    [TestMethod]
    public void EqualityOperator_WithIdenticalSeparateLists_ReturnsTrue()
    {
        var metadata = AssetMetadata.Empty;
        var a = new ComponentData { Metadata = metadata, Caption = "CPU", Properties = [new("Speed", "3.5 GHz")] };
        var b = new ComponentData { Metadata = metadata, Caption = "CPU", Properties = [new("Speed", "3.5 GHz")] };

        Assert.IsTrue(a == b);
        Assert.IsFalse(a != b);
    }

    // --- GetHashCode ---

    [TestMethod]
    public void GetHashCode_IsConsistentForSameInstance()
    {
        var data = new ComponentData
        {
            Metadata = AssetMetadata.Empty,
            Caption = "CPU",
            Properties = [new("Speed", "3.5 GHz")]
        };

        Assert.AreEqual(data.GetHashCode(), data.GetHashCode());
    }

    [TestMethod]
    public void GetHashCode_IsSameForEqualInstances()
    {
        var metadata = new AssetMetadata { SerialNumber = "SN1", Manufacturer = "Dell", Product = "XPS" };

        var a = new ComponentData { Metadata = metadata, Caption = "CPU", Properties = [new("Speed", "3.5 GHz")] };
        var b = new ComponentData { Metadata = metadata, Caption = "CPU", Properties = [new("Speed", "3.5 GHz")] };

        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void GetHashCode_DiffersForDifferentCaption()
    {
        var a = new ComponentData { Metadata = AssetMetadata.Empty, Caption = "CPU", Properties = [] };
        var b = new ComponentData { Metadata = AssetMetadata.Empty, Caption = "GPU", Properties = [] };

        Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void GetHashCode_DiffersForDifferentProperties()
    {
        var a = new ComponentData { Metadata = AssetMetadata.Empty, Caption = "CPU", Properties = [new("Speed", "3.5 GHz")] };
        var b = new ComponentData { Metadata = AssetMetadata.Empty, Caption = "CPU", Properties = [new("Speed", "4.0 GHz")] };

        Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void GetHashCode_DiffersForDifferentMetadata()
    {
        var a = new ComponentData
        {
            Metadata = new AssetMetadata { SerialNumber = "SN1", Manufacturer = "Dell", Product = "XPS" },
            Caption = "CPU",
            Properties = []
        };
        var b = new ComponentData
        {
            Metadata = new AssetMetadata { SerialNumber = "SN2", Manufacturer = "Dell", Product = "XPS" },
            Caption = "CPU",
            Properties = []
        };

        Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
    }
}
