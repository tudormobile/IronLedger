using System.Text.Json;
using Tudormobile.IronLedgerLib.Serialization;

namespace IronLedgerLib.Tests;

[TestClass]
public class IronLedgerJsonSerializerTests
{
    [TestMethod]
    public void IronLedgerJsonSerializer_DefaultConstructor_CreatesInstance()
    {
        // Arrange & Act
        var serializer = new IronLedgerJsonSerializer();

        // Assert
        Assert.IsNotNull(serializer);
    }

    [TestMethod]
    public void IronLedgerJsonSerializer_CustomOptions_UsesProvidedOptions()
    {
        // Arrange
        var options = new JsonSerializerOptions { WriteIndented = false };

        // Act
        var serializer = new IronLedgerJsonSerializer(options);

        // Assert
        Assert.IsNotNull(serializer);
    }

    [TestMethod]
    public void IronLedgerJsonSerializer_Constructor_ThrowsOnNullOptions()
    {
        // Arrange, Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => new IronLedgerJsonSerializer(null!));
    }

    [TestMethod]
    public void Serialize_AssetId_ReturnsJsonString()
    {
        // Arrange
        var serializer = new IronLedgerJsonSerializer();
        var assetId = new AssetId
        {
            SystemMetadata = new AssetMetadata
            {
                SerialNumber = "SYS-001",
                Manufacturer = "Dell",
                Product = "XPS 15"
            },
            BaseBoardMetadata = new AssetMetadata
            {
                SerialNumber = "BB-001",
                Manufacturer = "Intel",
                Product = "Motherboard"
            },
            BiosMetadata = new AssetMetadata
            {
                SerialNumber = "BIOS-001",
                Manufacturer = "Phoenix",
                Product = "BIOS v1.0"
            }
        };

        // Act
        var json = serializer.Serialize(assetId);

        // Assert
        Assert.IsNotNull(json);
        Assert.IsTrue(json.Contains("SYS-001"));
        Assert.IsTrue(json.Contains("Dell"));
        Assert.IsTrue(json.Contains("BB-001"));
        Assert.IsTrue(json.Contains("BIOS-001"));
    }

    [TestMethod]
    public void Deserialize_AssetId_ReturnsObject()
    {
        // Arrange
        var serializer = new IronLedgerJsonSerializer();
        var json = """
        {
          "system_metadata": {
            "serial_number": "SYS-001",
            "manufacturer": "Dell",
            "product": "XPS 15"
          },
          "base_board_metadata": {
            "serial_number": "BB-001",
            "manufacturer": "Intel",
            "product": "Motherboard"
          },
          "bios_metadata": {
            "serial_number": "BIOS-001",
            "manufacturer": "Phoenix",
            "product": "BIOS v1.0"
          }
        }
        """;

        // Act
        var assetId = serializer.Deserialize<AssetId>(json);

        // Assert
        Assert.IsNotNull(assetId);
        Assert.AreEqual("SYS-001", assetId.SystemMetadata.SerialNumber);
        Assert.AreEqual("Dell", assetId.SystemMetadata.Manufacturer);
        Assert.AreEqual("BB-001", assetId.BaseBoardMetadata.SerialNumber);
        Assert.AreEqual("BIOS-001", assetId.BiosMetadata.SerialNumber);
    }

    [TestMethod]
    public void Serialize_ComponentData_ReturnsJsonString()
    {
        // Arrange
        var serializer = new IronLedgerJsonSerializer();
        var componentData = new ComponentData
        {
            Metadata = new AssetMetadata
            {
                SerialNumber = "CPU-001",
                Manufacturer = "Intel",
                Product = "Core i7"
            },
            Caption = "Intel Core i7-9700K",
            Properties =
            [
                new ComponentProperty("Cores", "8"),
                new ComponentProperty("Speed", "3.6 GHz")
            ]
        };

        // Act
        var json = serializer.Serialize(componentData);

        // Assert
        Assert.IsNotNull(json);
        Assert.Contains("CPU-001", json);
        Assert.Contains("Intel", json);
        Assert.Contains("Intel Core i7-9700K", json);
        Assert.Contains("cores", json);
        Assert.Contains("8", json);
    }

    [TestMethod]
    public void Deserialize_ComponentData_ReturnsObject()
    {
        // Arrange
        var serializer = new IronLedgerJsonSerializer();
        var json = """
        {
          "metadata": {
            "serial_number": "CPU-001",
            "manufacturer": "Intel",
            "product": "Core i7"
          },
          "caption": "Intel Core i7-9700K",
          "properties": {
            "cores": "8",
            "speed": "3.6 GHz"
          }
        }
        """;

        // Act
        var componentData = serializer.Deserialize<ComponentData>(json);

        // Assert
        Assert.IsNotNull(componentData);
        Assert.AreEqual("CPU-001", componentData.Metadata.SerialNumber);
        Assert.AreEqual("Intel Core i7-9700K", componentData.Caption);
        Assert.AreEqual(2, componentData.Properties.Count);
        Assert.AreEqual("Cores", componentData.Properties[0].Name);
        Assert.AreEqual("8", componentData.Properties[0].Value);
    }

    [TestMethod]
    public void Serialize_SystemComponentData_ReturnsJsonString()
    {
        // Arrange
        var serializer = new IronLedgerJsonSerializer();
        var systemComponentData = new SystemComponentData
        {
            System = new ComponentData
            {
                Metadata = AssetMetadata.Empty,
                Caption = "DESKTOP-PC",
                Properties = []
            },
            Processors =
            [
                new ComponentData
                {
                    Metadata = AssetMetadata.Empty,
                    Caption = "Intel Core i7",
                    Properties = []
                }
            ],
            Memory = [],
            Disks = []
        };

        // Act
        var json = serializer.Serialize(systemComponentData);

        // Assert
        Assert.IsNotNull(json);
        Assert.IsTrue(json.Contains("DESKTOP-PC"));
        Assert.IsTrue(json.Contains("Intel Core i7"));
    }

    [TestMethod]
    public void Deserialize_SystemComponentData_ReturnsObject()
    {
        // Arrange
        var serializer = new IronLedgerJsonSerializer();
        var json = """
        {
          "system": {
            "metadata": {
              "serial_number": "",
              "manufacturer": "",
              "product": ""
            },
            "caption": "DESKTOP-PC",
            "properties": {}
          },
          "processors": [
            {
              "metadata": {
                "serial_number": "CPU-001",
                "manufacturer": "Intel",
                "product": "Core i7"
              },
              "caption": "Intel Core i7-9700K",
              "properties": {}
            }
          ],
          "memory": [],
          "disks": []
        }
        """;

        // Act
        var systemComponentData = serializer.Deserialize<SystemComponentData>(json);

        // Assert
        Assert.IsNotNull(systemComponentData);
        Assert.AreEqual("DESKTOP-PC", systemComponentData.System.Caption);
        Assert.AreEqual(1, systemComponentData.Processors.Count);
        Assert.AreEqual("Intel Core i7-9700K", systemComponentData.Processors[0].Caption);
    }

    [TestMethod]
    public void Serialize_ThrowsOnNullValue()
    {
        // Arrange
        var serializer = new IronLedgerJsonSerializer();

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => serializer.Serialize<AssetId>(null!));
    }

    [TestMethod]
    public void Deserialize_ThrowsOnNullData()
    {
        // Arrange
        var serializer = new IronLedgerJsonSerializer();

        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => serializer.Deserialize<AssetId>(null!));
    }

    [TestMethod]
    public void Deserialize_ThrowsOnEmptyData()
    {
        // Arrange
        var serializer = new IronLedgerJsonSerializer();

        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => serializer.Deserialize<AssetId>(""));
    }

    [TestMethod]
    public void Deserialize_ThrowsOnWhitespaceData()
    {
        // Arrange
        var serializer = new IronLedgerJsonSerializer();

        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => serializer.Deserialize<AssetId>("   "));
    }

    [TestMethod]
    public void RoundTrip_AssetId_PreservesData()
    {
        // Arrange
        var serializer = new IronLedgerJsonSerializer();
        var original = new AssetId
        {
            SystemMetadata = new AssetMetadata
            {
                SerialNumber = "SYS-123",
                Manufacturer = "Test Mfg",
                Product = "Test Product"
            },
            BaseBoardMetadata = AssetMetadata.Empty,
            BiosMetadata = AssetMetadata.Empty
        };

        // Act
        var json = serializer.Serialize(original);
        var deserialized = serializer.Deserialize<AssetId>(json);

        // Assert
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(original.SystemMetadata.SerialNumber, deserialized.SystemMetadata.SerialNumber);
        Assert.AreEqual(original.SystemMetadata.Manufacturer, deserialized.SystemMetadata.Manufacturer);
        Assert.AreEqual(original.SystemMetadata.Product, deserialized.SystemMetadata.Product);
    }

    [TestMethod]
    public void RoundTrip_ComponentData_PreservesData()
    {
        // Arrange
        var serializer = new IronLedgerJsonSerializer();
        var original = new ComponentData
        {
            Metadata = new AssetMetadata
            {
                SerialNumber = "TEST-001",
                Manufacturer = "TestCo",
                Product = "Widget"
            },
            Caption = "Test Caption",
            Properties =
            [
                new ComponentProperty("Prop1", "Val1"),
                new ComponentProperty("Prop2", "Val2")
            ]
        };

        // Act
        var json = serializer.Serialize(original);
        var deserialized = serializer.Deserialize<ComponentData>(json);

        // Assert
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(original.Caption, deserialized.Caption);
        Assert.AreEqual(original.Properties.Count, deserialized.Properties.Count);
        Assert.AreEqual(original.Properties[0].Name, deserialized.Properties[0].Name);
        Assert.AreEqual(original.Properties[0].Value, deserialized.Properties[0].Value);
    }

    [TestMethod]
    public void CreateDefaultOptions_ReturnsConfiguredOptions()
    {
        // Act
        var options = IronLedgerJsonSerializer.CreateDefaultOptions();

        // Assert
        Assert.IsNotNull(options);
        Assert.IsTrue(options.WriteIndented);
        Assert.AreEqual(JsonNamingPolicy.SnakeCaseLower, options.PropertyNamingPolicy);
    }
}
