namespace IronLedgerLib.Tests;

[TestClass]
public class SerializationExtensionsTests
{
    [TestMethod]
    public void AssetId_Serialize_ReturnsJsonString()
    {
        // Arrange
        var assetId = new AssetId
        {
            SystemMetadata = new AssetMetadata
            {
                SerialNumber = "SYS-001",
                Manufacturer = "Dell",
                Product = "XPS 15"
            },
            BaseBoardMetadata = AssetMetadata.Empty,
            BiosMetadata = AssetMetadata.Empty
        };

        // Act
        var json = assetId.Serialize();

        // Assert
        Assert.IsNotNull(json);
        Assert.IsTrue(json.Contains("SYS-001"));
        Assert.IsTrue(json.Contains("Dell"));
    }

    [TestMethod]
    public void AssetId_Serialize_WithCustomSerializer_UsesCustomSerializer()
    {
        // Arrange
        var assetId = new AssetId
        {
            SystemMetadata = AssetMetadata.Empty,
            BaseBoardMetadata = AssetMetadata.Empty,
            BiosMetadata = AssetMetadata.Empty
        };
        var mockSerializer = new MockSerializer();

        // Act
        var result = assetId.Serialize(mockSerializer);

        // Assert
        Assert.AreEqual("MOCK", result);
        Assert.IsTrue(mockSerializer.SerializeCalled);
    }

    [TestMethod]
    public void ComponentData_Serialize_ReturnsJsonString()
    {
        // Arrange
        var componentData = new ComponentData
        {
            Metadata = new AssetMetadata
            {
                SerialNumber = "CPU-001",
                Manufacturer = "Intel",
                Product = "Core i7"
            },
            Caption = "Intel Core i7-9700K",
            Properties = []
        };

        // Act
        var json = componentData.Serialize();

        // Assert
        Assert.IsNotNull(json);
        Assert.IsTrue(json.Contains("CPU-001"));
        Assert.IsTrue(json.Contains("Intel Core i7-9700K"));
    }

    [TestMethod]
    public void SystemComponentData_Serialize_ReturnsJsonString()
    {
        // Arrange
        var systemComponentData = new SystemComponentData
        {
            System = ComponentData.Empty,
            Processors = [],
            Memory = [],
            Disks = []
        };

        // Act
        var json = systemComponentData.Serialize();

        // Assert
        Assert.IsNotNull(json);
        Assert.IsTrue(json.Length > 0);
    }

    [TestMethod]
    public void String_DeserializeAssetId_ReturnsAssetId()
    {
        // Arrange
        var json = """
        {
          "system_metadata": {
            "serial_number": "SYS-001",
            "manufacturer": "Dell",
            "product": "XPS 15"
          },
          "base_board_metadata": {
            "serial_number": "",
            "manufacturer": "",
            "product": ""
          },
          "bios_metadata": {
            "serial_number": "",
            "manufacturer": "",
            "product": ""
          }
        }
        """;

        // Act
        var assetId = json.DeserializeAssetId();

        // Assert
        Assert.IsNotNull(assetId);
        Assert.AreEqual("SYS-001", assetId.SystemMetadata.SerialNumber);
        Assert.AreEqual("Dell", assetId.SystemMetadata.Manufacturer);
    }

    [TestMethod]
    public void String_DeserializeAssetId_WithCustomSerializer_UsesCustomSerializer()
    {
        // Arrange
        var json = "test";
        var mockSerializer = new MockSerializer();

        // Act
        var result = json.DeserializeAssetId(mockSerializer);

        // Assert
        Assert.IsTrue(mockSerializer.DeserializeCalled);
    }

    [TestMethod]
    public void String_DeserializeComponentData_ReturnsComponentData()
    {
        // Arrange
        var json = """
        {
          "metadata": {
            "serial_number": "CPU-001",
            "manufacturer": "Intel",
            "product": "Core i7"
          },
          "caption": "Intel Core i7-9700K",
          "properties": {}
        }
        """;

        // Act
        var componentData = json.DeserializeComponentData();

        // Assert
        Assert.IsNotNull(componentData);
        Assert.AreEqual("CPU-001", componentData.Metadata.SerialNumber);
        Assert.AreEqual("Intel Core i7-9700K", componentData.Caption);
    }

    [TestMethod]
    public void String_DeserializeSystemComponentData_ReturnsSystemComponentData()
    {
        // Arrange
        var json = """
        {
          "system": {
            "metadata": {
              "serial_number": "",
              "manufacturer": "",
              "product": ""
            },
            "caption": "Test System",
            "properties": {}
          },
          "processors": [],
          "memory": [],
          "disks": []
        }
        """;

        // Act
        var systemComponentData = json.DeserializeSystemComponentData();

        // Assert
        Assert.IsNotNull(systemComponentData);
        Assert.AreEqual("Test System", systemComponentData.System.Caption);
    }

    [TestMethod]
    public void RoundTrip_AssetId_PreservesData()
    {
        // Arrange
        var original = new AssetId
        {
            SystemMetadata = new AssetMetadata
            {
                SerialNumber = "TEST-123",
                Manufacturer = "TestMfg",
                Product = "TestProd"
            },
            BaseBoardMetadata = AssetMetadata.Empty,
            BiosMetadata = AssetMetadata.Empty
        };

        // Act
        var json = original.Serialize();
        var deserialized = json.DeserializeAssetId();

        // Assert
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(original.SystemMetadata.SerialNumber, deserialized.SystemMetadata.SerialNumber);
        Assert.AreEqual(original.SystemMetadata.Manufacturer, deserialized.SystemMetadata.Manufacturer);
    }

    [TestMethod]
    public void RoundTrip_ComponentData_PreservesData()
    {
        // Arrange
        var original = new ComponentData
        {
            Metadata = new AssetMetadata
            {
                SerialNumber = "TEST-456",
                Manufacturer = "TestCo",
                Product = "Widget"
            },
            Caption = "Test Widget",
            Properties =
            [
                new ComponentProperty("Key1", "Value1"),
                new ComponentProperty("Key2", "Value2")
            ]
        };

        // Act
        var json = original.Serialize();
        var deserialized = json.DeserializeComponentData();

        // Assert
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(original.Caption, deserialized.Caption);
        Assert.AreEqual(original.Properties.Count, deserialized.Properties.Count);
    }

    [TestMethod]
    public void RoundTrip_SystemComponentData_PreservesData()
    {
        // Arrange
        var original = new SystemComponentData
        {
            System = new ComponentData
            {
                Metadata = AssetMetadata.Empty,
                Caption = "Main System",
                Properties = []
            },
            Processors =
            [
                new ComponentData
                {
                    Metadata = AssetMetadata.Empty,
                    Caption = "CPU 1",
                    Properties = []
                }
            ],
            Memory = [],
            Disks = []
        };

        // Act
        var json = original.Serialize();
        var deserialized = json.DeserializeSystemComponentData();

        // Assert
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(original.System.Caption, deserialized.System.Caption);
        Assert.AreEqual(original.Processors.Count, deserialized.Processors.Count);
    }

    private class MockSerializer : IIronLedgerSerializer
    {
        public string ContentType => "application/json";
        public bool SerializeCalled { get; private set; }
        public bool DeserializeCalled { get; private set; }

        public string Serialize<T>(T value)
        {
            SerializeCalled = true;
            return "MOCK";
        }

        public T? Deserialize<T>(string data)
        {
            DeserializeCalled = true;
            return default;
        }
    }
}
