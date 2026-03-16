using Tudormobile.IronLedgerLib.Serialization;

namespace IronLedgerLib.Tests;

[TestClass]
public class FileSystemAssetRepositoryTests
{
    private string _dataPath = null!;
    private FileSystemAssetRepository _repository = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _dataPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        _repository = new FileSystemAssetRepository(_dataPath, new IronLedgerJsonSerializer());
    }

    [TestCleanup]
    public void TestCleanup()
    {
        if (Directory.Exists(_dataPath))
            Directory.Delete(_dataPath, recursive: true);
    }

    private static AssetRecord CreateRecord(string serial = "SN001")
    {
        var metadata = new AssetMetadata { SerialNumber = serial, Manufacturer = "Acme", Product = "Widget" };
        var assetId = new AssetId
        {
            SystemMetadata = metadata,
            BaseBoardMetadata = metadata,
            BiosMetadata = metadata
        };
        var component = ComponentData.Empty with { Caption = "Test", Metadata = metadata };
        var components = new SystemComponentData
        {
            System = component,
            Processors = [],
            Memory = [],
            Disks = []
        };
        return new AssetRecord { Id = assetId, Components = components };
    }

    // --- constructor ---

    [TestMethod]
    public void Constructor_NullDataPath_Throws()
        => Assert.ThrowsExactly<ArgumentException>(() => new FileSystemAssetRepository("   ", new IronLedgerJsonSerializer()));

    [TestMethod]
    public void Constructor_NullSerializer_Throws()
        => Assert.ThrowsExactly<ArgumentNullException>(() => new FileSystemAssetRepository(_dataPath, null!));

    // --- GetAllAsync ---

    [TestMethod]
    public async Task GetAllAsync_WhenDirectoryDoesNotExist_ReturnsEmptyList()
    {
        var result = await _repository.GetAllAsync();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public async Task GetAllAsync_AfterSavingTwoRecords_ReturnsBothRecords()
    {
        await _repository.SaveAsync(CreateRecord("SN001"));
        await _repository.SaveAsync(CreateRecord("SN002"));

        var result = await _repository.GetAllAsync();

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public async Task GetAllAsync_SkipsDirectoriesWithoutAssetFile()
    {
        Directory.CreateDirectory(Path.Combine(_dataPath, "empty-dir"));

        var result = await _repository.GetAllAsync();

        Assert.AreEqual(0, result.Count);
    }

    // --- GetAsync ---

    [TestMethod]
    public async Task GetAsync_WhenAssetDoesNotExist_ReturnsNull()
    {
        var result = await _repository.GetAsync("0000000000000000000000000000000000000000000000000000000000000000");

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetAsync_AfterSave_ReturnsMatchingRecord()
    {
        var record = CreateRecord();
        await _repository.SaveAsync(record);

        var result = await _repository.GetAsync(record.Id.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual(record.Id.Id, result.Id.Id);
    }

    // --- SaveAsync ---

    [TestMethod]
    public async Task SaveAsync_CreatesAssetJsonFile()
    {
        var record = CreateRecord();

        await _repository.SaveAsync(record);

        var expectedPath = Path.Combine(_dataPath, record.Id.Id, "asset.json");
        Assert.IsTrue(File.Exists(expectedPath));
    }

    [TestMethod]
    public async Task SaveAsync_OverwritesExistingRecord()
    {
        var record = CreateRecord();
        await _repository.SaveAsync(record);
        await _repository.SaveAsync(record);

        var all = await _repository.GetAllAsync();
        Assert.AreEqual(1, all.Count);
    }

    [TestMethod]
    public async Task SaveAsync_NullAsset_Throws()
        => await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => _repository.SaveAsync(null!));

    // --- DeleteAsync ---

    [TestMethod]
    public async Task DeleteAsync_AfterSave_AssetNoLongerRetrievable()
    {
        var record = CreateRecord();
        await _repository.SaveAsync(record);

        await _repository.DeleteAsync(record.Id.Id);

        Assert.IsNull(await _repository.GetAsync(record.Id.Id));
    }

    [TestMethod]
    public async Task DeleteAsync_NonexistentId_DoesNotThrow()
        => await _repository.DeleteAsync("0000000000000000000000000000000000000000000000000000000000000000");

    // --- GetNotesAsync ---

    [TestMethod]
    public async Task GetNotesAsync_WhenNoNotesFile_ReturnsEmptyString()
    {
        var result = await _repository.GetNotesAsync("0000000000000000000000000000000000000000000000000000000000000000");

        Assert.AreEqual(string.Empty, result);
    }

    // --- SaveNotesAsync / GetNotesAsync round-trip ---

    [TestMethod]
    public async Task SaveNotesAsync_ThenGetNotesAsync_ReturnsNotes()
    {
        var record = CreateRecord();
        await _repository.SaveAsync(record);
        var markdown = "# Test Asset\n\nSome notes here.";

        await _repository.SaveNotesAsync(record.Id.Id, markdown);
        var result = await _repository.GetNotesAsync(record.Id.Id);

        Assert.AreEqual(markdown, result);
    }

    [TestMethod]
    public async Task SaveNotesAsync_CreatesNotesFileAlongsideAsset()
    {
        var record = CreateRecord();
        await _repository.SaveAsync(record);

        await _repository.SaveNotesAsync(record.Id.Id, "# Notes");

        var expectedPath = Path.Combine(_dataPath, record.Id.Id, "notes.md");
        Assert.IsTrue(File.Exists(expectedPath));
    }

    [TestMethod]
    public async Task DeleteAsync_RemovesNotesAlongWithAsset()
    {
        var record = CreateRecord();
        await _repository.SaveAsync(record);
        await _repository.SaveNotesAsync(record.Id.Id, "# Notes");

        await _repository.DeleteAsync(record.Id.Id);

        Assert.AreEqual(string.Empty, await _repository.GetNotesAsync(record.Id.Id));
    }
}
