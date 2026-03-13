namespace IronLedgerLib.Tests;

[TestClass]
public class AssetIdUniqueIdTests
{
    [TestMethod]
    public void UniqueId_WithIdenticalData_ReturnsSameId()
    {
        // Arrange
        var assetId1 = new AssetId
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

        var assetId2 = new AssetId
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
        var id1 = assetId1.Id;
        var id2 = assetId2.Id;

        // Assert
        Assert.AreEqual(id1, id2);
    }

    [TestMethod]
    public void UniqueId_WithDifferentSystemMetadata_ReturnsDifferentId()
    {
        // Arrange
        var assetId1 = new AssetId
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

        var assetId2 = new AssetId
        {
            SystemMetadata = new AssetMetadata
            {
                SerialNumber = "SYS-002",  // Different
                Manufacturer = "Dell",
                Product = "XPS 15"
            },
            BaseBoardMetadata = AssetMetadata.Empty,
            BiosMetadata = AssetMetadata.Empty
        };

        // Act
        var id1 = assetId1.Id;
        var id2 = assetId2.Id;

        // Assert
        Assert.AreNotEqual(id1, id2);
    }

    [TestMethod]
    public void UniqueId_WithDifferentBaseBoardMetadata_ReturnsDifferentId()
    {
        // Arrange
        var assetId1 = new AssetId
        {
            SystemMetadata = AssetMetadata.Empty,
            BaseBoardMetadata = new AssetMetadata
            {
                SerialNumber = "BB-001",
                Manufacturer = "Intel",
                Product = "Board1"
            },
            BiosMetadata = AssetMetadata.Empty
        };

        var assetId2 = new AssetId
        {
            SystemMetadata = AssetMetadata.Empty,
            BaseBoardMetadata = new AssetMetadata
            {
                SerialNumber = "BB-002",  // Different
                Manufacturer = "Intel",
                Product = "Board1"
            },
            BiosMetadata = AssetMetadata.Empty
        };

        // Act
        var id1 = assetId1.Id;
        var id2 = assetId2.Id;

        // Assert
        Assert.AreNotEqual(id1, id2);
    }

    [TestMethod]
    public void UniqueId_WithDifferentBiosMetadata_ReturnsDifferentId()
    {
        // Arrange
        var assetId1 = new AssetId
        {
            SystemMetadata = AssetMetadata.Empty,
            BaseBoardMetadata = AssetMetadata.Empty,
            BiosMetadata = new AssetMetadata
            {
                SerialNumber = "BIOS-001",
                Manufacturer = "Phoenix",
                Product = "v1.0"
            }
        };

        var assetId2 = new AssetId
        {
            SystemMetadata = AssetMetadata.Empty,
            BaseBoardMetadata = AssetMetadata.Empty,
            BiosMetadata = new AssetMetadata
            {
                SerialNumber = "BIOS-002",  // Different
                Manufacturer = "Phoenix",
                Product = "v1.0"
            }
        };

        // Act
        var id1 = assetId1.Id;
        var id2 = assetId2.Id;

        // Assert
        Assert.AreNotEqual(id1, id2);
    }

    [TestMethod]
    public void UniqueId_IsNotNullOrEmpty()
    {
        // Arrange
        var assetId = new AssetId
        {
            SystemMetadata = AssetMetadata.Empty,
            BaseBoardMetadata = AssetMetadata.Empty,
            BiosMetadata = AssetMetadata.Empty
        };

        // Act
        var uniqueId = assetId.Id;

        // Assert
        Assert.IsFalse(string.IsNullOrEmpty(uniqueId));
    }

    [TestMethod]
    public void UniqueId_HasCorrectLength()
    {
        // Arrange
        var assetId = new AssetId
        {
            SystemMetadata = AssetMetadata.Empty,
            BaseBoardMetadata = AssetMetadata.Empty,
            BiosMetadata = AssetMetadata.Empty
        };

        // Act
        var uniqueId = assetId.Id;

        // Assert - SHA256 hash in hex is 64 characters (32 bytes * 2)
        Assert.AreEqual(64, uniqueId.Length);
    }

    [TestMethod]
    public void UniqueId_IsHexadecimalString()
    {
        // Arrange
        var assetId = new AssetId
        {
            SystemMetadata = AssetMetadata.Empty,
            BaseBoardMetadata = AssetMetadata.Empty,
            BiosMetadata = AssetMetadata.Empty
        };

        // Act
        var uniqueId = assetId.Id;

        // Assert - Should only contain hex characters (0-9, a-f)
        Assert.IsTrue(uniqueId.All(c => (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f')));
    }

    [TestMethod]
    public void UniqueId_IsCached_ReturnsSameInstance()
    {
        // Arrange
        var assetId = new AssetId
        {
            SystemMetadata = new AssetMetadata
            {
                SerialNumber = "TEST",
                Manufacturer = "Test",
                Product = "Test"
            },
            BaseBoardMetadata = AssetMetadata.Empty,
            BiosMetadata = AssetMetadata.Empty
        };

        // Act
        var id1 = assetId.Id;
        var id2 = assetId.Id;

        // Assert
        Assert.AreSame(id1, id2);
    }

    [TestMethod]
    public void UniqueId_WithAllEmptyMetadata_ReturnsConsistentId()
    {
        // Arrange
        var assetId1 = new AssetId
        {
            SystemMetadata = AssetMetadata.Empty,
            BaseBoardMetadata = AssetMetadata.Empty,
            BiosMetadata = AssetMetadata.Empty
        };

        var assetId2 = new AssetId
        {
            SystemMetadata = AssetMetadata.Empty,
            BaseBoardMetadata = AssetMetadata.Empty,
            BiosMetadata = AssetMetadata.Empty
        };

        // Act
        var id1 = assetId1.Id;
        var id2 = assetId2.Id;

        // Assert
        Assert.AreEqual(id1, id2);
    }

    [TestMethod]
    public void UniqueId_WithComplexMetadata_GeneratesId()
    {
        // Arrange
        var assetId = new AssetId
        {
            SystemMetadata = new AssetMetadata
            {
                SerialNumber = "SYS-12345-67890",
                Manufacturer = "Dell Inc.",
                Product = "XPS 15 9500 Laptop"
            },
            BaseBoardMetadata = new AssetMetadata
            {
                SerialNumber = "/BB.XYZ.123/456/",
                Manufacturer = "Intel Corporation",
                Product = "Z490 Chipset Motherboard"
            },
            BiosMetadata = new AssetMetadata
            {
                SerialNumber = "BIOS:v2.1.5:20210315",
                Manufacturer = "American Megatrends Inc.",
                Product = "UEFI BIOS Version 2.1.5"
            }
        };

        // Act
        var uniqueId = assetId.Id;

        // Assert
        Assert.IsNotNull(uniqueId);
        Assert.AreEqual(64, uniqueId.Length);
    }

    [TestMethod]
    public void UniqueId_WithSpecialCharacters_HandlesCorrectly()
    {
        // Arrange
        var assetId1 = new AssetId
        {
            SystemMetadata = new AssetMetadata
            {
                SerialNumber = "SN|123",  // Contains delimiter
                Manufacturer = "Test",
                Product = "Test"
            },
            BaseBoardMetadata = AssetMetadata.Empty,
            BiosMetadata = AssetMetadata.Empty
        };

        var assetId2 = new AssetId
        {
            SystemMetadata = new AssetMetadata
            {
                SerialNumber = "SN|124",  // Different after delimiter
                Manufacturer = "Test",
                Product = "Test"
            },
            BaseBoardMetadata = AssetMetadata.Empty,
            BiosMetadata = AssetMetadata.Empty
        };

        // Act
        var id1 = assetId1.Id;
        var id2 = assetId2.Id;

        // Assert
        Assert.AreNotEqual(id1, id2);
    }

    [TestMethod]
    public void UniqueId_DifferentOrderSameData_ProducesDifferentId()
    {
        // Arrange - Swap SystemMetadata and BaseBoardMetadata content
        var assetId1 = new AssetId
        {
            SystemMetadata = new AssetMetadata
            {
                SerialNumber = "A",
                Manufacturer = "B",
                Product = "C"
            },
            BaseBoardMetadata = new AssetMetadata
            {
                SerialNumber = "X",
                Manufacturer = "Y",
                Product = "Z"
            },
            BiosMetadata = AssetMetadata.Empty
        };

        var assetId2 = new AssetId
        {
            SystemMetadata = new AssetMetadata
            {
                SerialNumber = "X",
                Manufacturer = "Y",
                Product = "Z"
            },
            BaseBoardMetadata = new AssetMetadata
            {
                SerialNumber = "A",
                Manufacturer = "B",
                Product = "C"
            },
            BiosMetadata = AssetMetadata.Empty
        };

        // Act
        var id1 = assetId1.Id;
        var id2 = assetId2.Id;

        // Assert - Order matters, so IDs should be different
        Assert.AreNotEqual(id1, id2);
    }

    [TestMethod]
    public void UniqueId_AfterSerialization_RemainsConsistent()
    {
        // Arrange
        var original = new AssetId
        {
            SystemMetadata = new AssetMetadata
            {
                SerialNumber = "SYS-TEST",
                Manufacturer = "Test Mfg",
                Product = "Test Product"
            },
            BaseBoardMetadata = AssetMetadata.Empty,
            BiosMetadata = AssetMetadata.Empty
        };

        var originalId = original.Id;

        // Act - Serialize and deserialize
        var json = original.Serialize();
        var deserialized = json.DeserializeAssetId();

        // Assert
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(originalId, deserialized.Id);
    }

    [TestMethod]
    public void UniqueId_WithUnicodeCharacters_HandlesCorrectly()
    {
        // Arrange
        var assetId = new AssetId
        {
            SystemMetadata = new AssetMetadata
            {
                SerialNumber = "SN-中文-123",
                Manufacturer = "メーカー",
                Product = "Продукт"
            },
            BaseBoardMetadata = AssetMetadata.Empty,
            BiosMetadata = AssetMetadata.Empty
        };

        // Act
        var uniqueId = assetId.Id;

        // Assert
        Assert.IsNotNull(uniqueId);
        Assert.AreEqual(64, uniqueId.Length);
        Assert.IsTrue(uniqueId.All(c => (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f')));
    }

    [TestMethod]
    public void ToString_ReturnsUniqueId()
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
        var toStringResult = assetId.ToString();
        var uniqueId = assetId.Id;

        // Assert
        Assert.AreEqual(uniqueId, toStringResult);
    }

    [TestMethod]
    public void ToString_ReturnsHexadecimalString()
    {
        // Arrange
        var assetId = new AssetId
        {
            SystemMetadata = AssetMetadata.Empty,
            BaseBoardMetadata = AssetMetadata.Empty,
            BiosMetadata = AssetMetadata.Empty
        };

        // Act
        var result = assetId.ToString();

        // Assert
        Assert.AreEqual(64, result.Length);
        Assert.IsTrue(result.All(c => (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f')));
    }

    [TestMethod]
    public void ToString_WithDifferentData_ReturnsDifferentStrings()
    {
        // Arrange
        var assetId1 = new AssetId
        {
            SystemMetadata = new AssetMetadata
            {
                SerialNumber = "SYS-001",
                Manufacturer = "Dell",
                Product = "XPS"
            },
            BaseBoardMetadata = AssetMetadata.Empty,
            BiosMetadata = AssetMetadata.Empty
        };

        var assetId2 = new AssetId
        {
            SystemMetadata = new AssetMetadata
            {
                SerialNumber = "SYS-002",
                Manufacturer = "HP",
                Product = "Pavilion"
            },
            BaseBoardMetadata = AssetMetadata.Empty,
            BiosMetadata = AssetMetadata.Empty
        };

        // Act
        var str1 = assetId1.ToString();
        var str2 = assetId2.ToString();

        // Assert
        Assert.AreNotEqual(str1, str2);
    }

    [TestMethod]
    public void ToString_CanBeUsedInStringInterpolation()
    {
        // Arrange
        var assetId = new AssetId
        {
            SystemMetadata = new AssetMetadata
            {
                SerialNumber = "TEST",
                Manufacturer = "Test",
                Product = "Test"
            },
            BaseBoardMetadata = AssetMetadata.Empty,
            BiosMetadata = AssetMetadata.Empty
        };

        // Act
        var interpolated = $"Asset: {assetId}";

        // Assert
        Assert.IsTrue(interpolated.StartsWith("Asset: "));
        Assert.IsTrue(interpolated.Contains(assetId.Id));
    }

    [TestMethod]
    public void UniqueId_AfterWithExpression_ReflectsModifiedMetadata()
    {
        // Arrange - access Id first to populate the cache on the original
        var original = new AssetId
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
        var originalId = original.Id; // Populate the cache

        // Act - create a modified copy via `with` expression after the cache is warm
        var modified = original with
        {
            SystemMetadata = original.SystemMetadata with { SerialNumber = "SYS-999" }
        };

        // Assert - the modified copy must compute a new hash, not inherit the cached one
        Assert.AreNotEqual(originalId, modified.Id,
            "Modified AssetId must recompute its Id and not inherit the cached value from the original.");

        // Also verify the original is unchanged
        Assert.AreEqual(originalId, original.Id);
    }
}
