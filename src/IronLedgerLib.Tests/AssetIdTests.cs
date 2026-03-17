namespace IronLedgerLib.Tests;

[TestClass]
public class AssetIdTests
{
    private AssetMetadata CreateSystemMetadata()
    {
        return new AssetMetadata
        {
            SerialNumber = "SYS-123456",
            Manufacturer = "Dell Inc.",
            Product = "OptiPlex 7090"
        };
    }

    private AssetMetadata CreateBaseBoardMetadata()
    {
        return new AssetMetadata
        {
            SerialNumber = "BB-789012",
            Manufacturer = "Dell Inc.",
            Product = "0DYH1K"
        };
    }

    private AssetMetadata CreateBiosMetadata()
    {
        return new AssetMetadata
        {
            SerialNumber = "BIOS-345678",
            Manufacturer = "Dell Inc.",
            Product = "2.15.0"
        };
    }

    [TestMethod]
    public void AssetId_IsValid_ReturnsFalseWhenNull()
    {
        // Arrange
        string? id = null;

        // Act
        var isValid = AssetId.IsValid(id);

        // Assert
        Assert.IsFalse(isValid);
    }

    [TestMethod]
    public void AssetId_IsValid_ReturnsFalseWhenEmpty()
    {
        // Arrange
        var id = string.Empty;

        // Act
        var isValid = AssetId.IsValid(id);

        // Assert
        Assert.IsFalse(isValid);
    }

    [TestMethod]
    public void AssetId_IsValid_ReturnsFalseWhenTooLong()
    {
        // Arrange
        var id = "00123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef";

        // Act
        var isValid = AssetId.IsValid(id);

        // Assert
        Assert.IsFalse(isValid);
    }

    [TestMethod]
    public void AssetId_IsValid_ReturnsTrueWhenValid()
    {
        // Arrange
        var id = "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef";

        // Act
        var isValid = AssetId.IsValid(id);

        // Assert
        Assert.IsTrue(isValid);
    }

    [TestMethod]
    public void AssetId_IsValid_ReturnsFalseWhenInvalid()
    {
        // Arrange
        string[] invalidIds =
            [
                "/123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef",
                ":123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef",
                "[123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef",
                "A123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef",
                "~123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef",
                "123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef",
            ];

        // Act & Assert
        foreach (var invalidId in invalidIds)
        {
            Assert.IsFalse(AssetId.IsValid(invalidId));
        }
    }

    [TestMethod]
    public void AssetId_CanBeCreated_WithRequiredProperties()
    {
        // Arrange
        var systemMetadata = CreateSystemMetadata();
        var baseBoardMetadata = CreateBaseBoardMetadata();
        var biosMetadata = CreateBiosMetadata();

        // Act
        var assetId = new AssetId
        {
            SystemMetadata = systemMetadata,
            BaseBoardMetadata = baseBoardMetadata,
            BiosMetadata = biosMetadata
        };

        // Assert
        Assert.IsNotNull(assetId);
        Assert.AreEqual(systemMetadata, assetId.SystemMetadata);
        Assert.AreEqual(baseBoardMetadata, assetId.BaseBoardMetadata);
        Assert.AreEqual(biosMetadata, assetId.BiosMetadata);
    }

    [TestMethod]
    public void AssetId_SupportsRecordEquality_WhenAllPropertiesMatch()
    {
        // Arrange
        var assetId1 = new AssetId
        {
            SystemMetadata = CreateSystemMetadata(),
            BaseBoardMetadata = CreateBaseBoardMetadata(),
            BiosMetadata = CreateBiosMetadata()
        };

        var assetId2 = new AssetId
        {
            SystemMetadata = CreateSystemMetadata(),
            BaseBoardMetadata = CreateBaseBoardMetadata(),
            BiosMetadata = CreateBiosMetadata()
        };

        // Act & Assert
        Assert.AreEqual(assetId1, assetId2);
        Assert.IsTrue(assetId1 == assetId2);
        Assert.IsFalse(assetId1 != assetId2);
    }

    [TestMethod]
    public void AssetId_RecordInequality_WhenSystemMetadataDiffers()
    {
        // Arrange
        var assetId1 = new AssetId
        {
            SystemMetadata = CreateSystemMetadata(),
            BaseBoardMetadata = CreateBaseBoardMetadata(),
            BiosMetadata = CreateBiosMetadata()
        };

        var assetId2 = new AssetId
        {
            SystemMetadata = new AssetMetadata
            {
                SerialNumber = "DIFFERENT-SN",
                Manufacturer = "Dell Inc.",
                Product = "OptiPlex 7090"
            },
            BaseBoardMetadata = CreateBaseBoardMetadata(),
            BiosMetadata = CreateBiosMetadata()
        };

        // Act & Assert
        Assert.AreNotEqual(assetId1, assetId2);
        Assert.IsFalse(assetId1 == assetId2);
        Assert.IsTrue(assetId1 != assetId2);
    }

    [TestMethod]
    public void AssetId_RecordInequality_WhenBaseBoardMetadataDiffers()
    {
        // Arrange
        var assetId1 = new AssetId
        {
            SystemMetadata = CreateSystemMetadata(),
            BaseBoardMetadata = CreateBaseBoardMetadata(),
            BiosMetadata = CreateBiosMetadata()
        };

        var assetId2 = new AssetId
        {
            SystemMetadata = CreateSystemMetadata(),
            BaseBoardMetadata = new AssetMetadata
            {
                SerialNumber = "DIFFERENT-BB",
                Manufacturer = "Dell Inc.",
                Product = "0DYH1K"
            },
            BiosMetadata = CreateBiosMetadata()
        };

        // Act & Assert
        Assert.AreNotEqual(assetId1, assetId2);
    }

    [TestMethod]
    public void AssetId_RecordInequality_WhenBiosMetadataDiffers()
    {
        // Arrange
        var assetId1 = new AssetId
        {
            SystemMetadata = CreateSystemMetadata(),
            BaseBoardMetadata = CreateBaseBoardMetadata(),
            BiosMetadata = CreateBiosMetadata()
        };

        var assetId2 = new AssetId
        {
            SystemMetadata = CreateSystemMetadata(),
            BaseBoardMetadata = CreateBaseBoardMetadata(),
            BiosMetadata = new AssetMetadata
            {
                SerialNumber = "DIFFERENT-BIOS",
                Manufacturer = "Dell Inc.",
                Product = "2.15.0"
            }
        };

        // Act & Assert
        Assert.AreNotEqual(assetId1, assetId2);
    }

    [TestMethod]
    public void AssetId_GetHashCode_IsConsistent()
    {
        // Arrange
        var assetId = new AssetId
        {
            SystemMetadata = CreateSystemMetadata(),
            BaseBoardMetadata = CreateBaseBoardMetadata(),
            BiosMetadata = CreateBiosMetadata()
        };

        // Act
        var hashCode1 = assetId.GetHashCode();
        var hashCode2 = assetId.GetHashCode();

        // Assert
        Assert.AreEqual(hashCode1, hashCode2);
    }

    [TestMethod]
    public void AssetId_GetHashCode_IsSameForEqualRecords()
    {
        // Arrange
        var assetId1 = new AssetId
        {
            SystemMetadata = CreateSystemMetadata(),
            BaseBoardMetadata = CreateBaseBoardMetadata(),
            BiosMetadata = CreateBiosMetadata()
        };

        var assetId2 = new AssetId
        {
            SystemMetadata = CreateSystemMetadata(),
            BaseBoardMetadata = CreateBaseBoardMetadata(),
            BiosMetadata = CreateBiosMetadata()
        };

        // Act & Assert
        Assert.AreEqual(assetId1.GetHashCode(), assetId2.GetHashCode());
    }

    [TestMethod]
    public void AssetId_ToString_ReturnsFormattedString()
    {
        // Arrange
        var assetId = new AssetId
        {
            SystemMetadata = CreateSystemMetadata(),
            BaseBoardMetadata = CreateBaseBoardMetadata(),
            BiosMetadata = CreateBiosMetadata()
        };

        // Act
        var result = assetId.ToString();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Length > 0);
    }

    [TestMethod]
    public void AssetId_WithClause_CanModifySystemMetadata()
    {
        // Arrange
        var original = new AssetId
        {
            SystemMetadata = CreateSystemMetadata(),
            BaseBoardMetadata = CreateBaseBoardMetadata(),
            BiosMetadata = CreateBiosMetadata()
        };

        var newSystemMetadata = new AssetMetadata
        {
            SerialNumber = "NEW-SYS-SN",
            Manufacturer = "HP Inc.",
            Product = "EliteDesk 800"
        };

        // Act
        var modified = original with { SystemMetadata = newSystemMetadata };

        // Assert
        Assert.AreNotEqual(original, modified);
        Assert.AreEqual(CreateSystemMetadata(), original.SystemMetadata);
        Assert.AreEqual(newSystemMetadata, modified.SystemMetadata);
        Assert.AreEqual(original.BaseBoardMetadata, modified.BaseBoardMetadata);
        Assert.AreEqual(original.BiosMetadata, modified.BiosMetadata);
    }

    [TestMethod]
    public void AssetId_WithClause_CanModifyBaseBoardMetadata()
    {
        // Arrange
        var original = new AssetId
        {
            SystemMetadata = CreateSystemMetadata(),
            BaseBoardMetadata = CreateBaseBoardMetadata(),
            BiosMetadata = CreateBiosMetadata()
        };

        var newBaseBoardMetadata = new AssetMetadata
        {
            SerialNumber = "NEW-BB-SN",
            Manufacturer = "ASUS",
            Product = "PRIME Z690"
        };

        // Act
        var modified = original with { BaseBoardMetadata = newBaseBoardMetadata };

        // Assert
        Assert.AreNotEqual(original, modified);
        Assert.AreEqual(original.SystemMetadata, modified.SystemMetadata);
        Assert.AreEqual(newBaseBoardMetadata, modified.BaseBoardMetadata);
        Assert.AreEqual(original.BiosMetadata, modified.BiosMetadata);
    }

    [TestMethod]
    public void AssetId_WithClause_CanModifyBiosMetadata()
    {
        // Arrange
        var original = new AssetId
        {
            SystemMetadata = CreateSystemMetadata(),
            BaseBoardMetadata = CreateBaseBoardMetadata(),
            BiosMetadata = CreateBiosMetadata()
        };

        var newBiosMetadata = new AssetMetadata
        {
            SerialNumber = "NEW-BIOS-SN",
            Manufacturer = "American Megatrends",
            Product = "3.0.0"
        };

        // Act
        var modified = original with { BiosMetadata = newBiosMetadata };

        // Assert
        Assert.AreNotEqual(original, modified);
        Assert.AreEqual(original.SystemMetadata, modified.SystemMetadata);
        Assert.AreEqual(original.BaseBoardMetadata, modified.BaseBoardMetadata);
        Assert.AreEqual(newBiosMetadata, modified.BiosMetadata);
    }

    [TestMethod]
    public void AssetId_WithClause_CanModifyMultipleProperties()
    {
        // Arrange
        var original = new AssetId
        {
            SystemMetadata = CreateSystemMetadata(),
            BaseBoardMetadata = CreateBaseBoardMetadata(),
            BiosMetadata = CreateBiosMetadata()
        };

        var newSystemMetadata = new AssetMetadata
        {
            SerialNumber = "NEW-SYS",
            Manufacturer = "HP",
            Product = "EliteDesk"
        };

        var newBiosMetadata = new AssetMetadata
        {
            SerialNumber = "NEW-BIOS",
            Manufacturer = "AMI",
            Product = "3.0"
        };

        // Act
        var modified = original with
        {
            SystemMetadata = newSystemMetadata,
            BiosMetadata = newBiosMetadata
        };

        // Assert
        Assert.AreNotEqual(original, modified);
        Assert.AreEqual(newSystemMetadata, modified.SystemMetadata);
        Assert.AreEqual(original.BaseBoardMetadata, modified.BaseBoardMetadata);
        Assert.AreEqual(newBiosMetadata, modified.BiosMetadata);
    }

    [TestMethod]
    public void AssetId_PropertiesAreImmutable()
    {
        // Arrange
        var original = new AssetId
        {
            SystemMetadata = CreateSystemMetadata(),
            BaseBoardMetadata = CreateBaseBoardMetadata(),
            BiosMetadata = CreateBiosMetadata()
        };

        var newMetadata = new AssetMetadata
        {
            SerialNumber = "NEW",
            Manufacturer = "NEW",
            Product = "NEW"
        };

        // Act
        var modified = original with { SystemMetadata = newMetadata };

        // Assert - Original is unchanged
        Assert.AreEqual(CreateSystemMetadata(), original.SystemMetadata);
        Assert.AreEqual(newMetadata, modified.SystemMetadata);
    }

    [TestMethod]
    public void AssetId_CanBeUsedInCollections()
    {
        // Arrange
        var assetId1 = new AssetId
        {
            SystemMetadata = CreateSystemMetadata(),
            BaseBoardMetadata = CreateBaseBoardMetadata(),
            BiosMetadata = CreateBiosMetadata()
        };

        var assetId2 = new AssetId
        {
            SystemMetadata = CreateSystemMetadata(),
            BaseBoardMetadata = CreateBaseBoardMetadata(),
            BiosMetadata = CreateBiosMetadata()
        };

        var assetId3 = new AssetId
        {
            SystemMetadata = new AssetMetadata
            {
                SerialNumber = "DIFFERENT",
                Manufacturer = "Different",
                Product = "Different"
            },
            BaseBoardMetadata = CreateBaseBoardMetadata(),
            BiosMetadata = CreateBiosMetadata()
        };

        // Act
        var set = new HashSet<AssetId> { assetId1, assetId2, assetId3 };
        var dict = new Dictionary<AssetId, string>
        {
            { assetId1, "First" },
            { assetId3, "Second" }
        };

        // Assert
        Assert.AreEqual(2, set.Count); // assetId1 and assetId2 are equal
        Assert.IsTrue(set.Contains(assetId1));
        Assert.IsTrue(set.Contains(assetId2));
        Assert.IsTrue(set.Contains(assetId3));
        Assert.AreEqual("First", dict[assetId2]); // Can lookup by equal key
    }

    [TestMethod]
    public void AssetId_ReferenceEquality_DifferentInstances()
    {
        // Arrange
        var assetId1 = new AssetId
        {
            SystemMetadata = CreateSystemMetadata(),
            BaseBoardMetadata = CreateBaseBoardMetadata(),
            BiosMetadata = CreateBiosMetadata()
        };

        var assetId2 = new AssetId
        {
            SystemMetadata = CreateSystemMetadata(),
            BaseBoardMetadata = CreateBaseBoardMetadata(),
            BiosMetadata = CreateBiosMetadata()
        };

        // Act & Assert
        Assert.IsFalse(ReferenceEquals(assetId1, assetId2)); // Different instances
        Assert.AreEqual(assetId1, assetId2); // But value-equal
    }

    [TestMethod]
    public void AssetId_ReferenceEquality_SameInstance()
    {
        // Arrange
        var assetId = new AssetId
        {
            SystemMetadata = CreateSystemMetadata(),
            BaseBoardMetadata = CreateBaseBoardMetadata(),
            BiosMetadata = CreateBiosMetadata()
        };

        // Act
        var sameReference = assetId;

        // Assert
        Assert.IsTrue(ReferenceEquals(assetId, sameReference));
        Assert.AreEqual(assetId, sameReference);
    }

    [TestMethod]
    public void AssetId_CanHandleIdenticalMetadataAcrossLevels()
    {
        // Arrange - Same metadata for all levels (edge case)
        var identicalMetadata = new AssetMetadata
        {
            SerialNumber = "SAME-SN",
            Manufacturer = "Same Mfr",
            Product = "Same Product"
        };

        // Act
        var assetId = new AssetId
        {
            SystemMetadata = identicalMetadata,
            BaseBoardMetadata = identicalMetadata,
            BiosMetadata = identicalMetadata
        };

        // Assert
        Assert.IsNotNull(assetId);
        Assert.AreSame(assetId.SystemMetadata, assetId.BaseBoardMetadata);
        Assert.AreSame(assetId.BaseBoardMetadata, assetId.BiosMetadata);
    }

    [TestMethod]
    public void AssetId_CanHandleEmptyStringsInMetadata()
    {
        // Arrange
        var emptyMetadata = new AssetMetadata
        {
            SerialNumber = "",
            Manufacturer = "",
            Product = ""
        };

        // Act
        var assetId = new AssetId
        {
            SystemMetadata = emptyMetadata,
            BaseBoardMetadata = emptyMetadata,
            BiosMetadata = emptyMetadata
        };

        // Assert
        Assert.IsNotNull(assetId);
        Assert.AreEqual("", assetId.SystemMetadata.SerialNumber);
        Assert.AreEqual("", assetId.BaseBoardMetadata.SerialNumber);
        Assert.AreEqual("", assetId.BiosMetadata.SerialNumber);
    }

    [TestMethod]
    public void AssetId_DifferentMetadataInstances_WithSameValues_AreEqual()
    {
        // Arrange
        var systemMetadata1 = new AssetMetadata
        {
            SerialNumber = "SYS-123",
            Manufacturer = "Dell",
            Product = "OptiPlex"
        };

        var systemMetadata2 = new AssetMetadata
        {
            SerialNumber = "SYS-123",
            Manufacturer = "Dell",
            Product = "OptiPlex"
        };

        var baseBoardMetadata = CreateBaseBoardMetadata();
        var biosMetadata = CreateBiosMetadata();

        var assetId1 = new AssetId
        {
            SystemMetadata = systemMetadata1,
            BaseBoardMetadata = baseBoardMetadata,
            BiosMetadata = biosMetadata
        };

        var assetId2 = new AssetId
        {
            SystemMetadata = systemMetadata2,
            BaseBoardMetadata = baseBoardMetadata,
            BiosMetadata = biosMetadata
        };

        // Act & Assert
        Assert.AreNotSame(systemMetadata1, systemMetadata2); // Different instances
        Assert.AreEqual(systemMetadata1, systemMetadata2); // But value-equal
        Assert.AreEqual(assetId1, assetId2); // AssetIds are also equal
    }

    [TestMethod]
    public void AssetId_WithClause_CreatesDeepCopy()
    {
        // Arrange
        var original = new AssetId
        {
            SystemMetadata = CreateSystemMetadata(),
            BaseBoardMetadata = CreateBaseBoardMetadata(),
            BiosMetadata = CreateBiosMetadata()
        };

        // Act
        var copy = original with { };

        // Assert
        Assert.AreNotSame(original, copy); // Different instances
        Assert.AreEqual(original, copy); // But value-equal
        Assert.AreSame(original.SystemMetadata, copy.SystemMetadata); // Metadata references are shared
        Assert.AreSame(original.BaseBoardMetadata, copy.BaseBoardMetadata);
        Assert.AreSame(original.BiosMetadata, copy.BiosMetadata);
    }
}