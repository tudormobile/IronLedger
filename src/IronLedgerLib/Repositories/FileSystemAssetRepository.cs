namespace Tudormobile.IronLedgerLib;

/// <summary>
/// An <see cref="IAssetRepository"/> implementation that stores each asset as a JSON file
/// and an optional Markdown notes file under a configurable root directory.
/// </summary>
/// <remarks>
/// Each asset is stored in its own subdirectory named after <see cref="AssetId.Id"/>:
/// <code>
/// {dataPath}/
///   {assetId}/
///     asset.json
///     notes.md
/// </code>
/// </remarks>
public class FileSystemAssetRepository : IAssetRepository
{
    private const string AssetFileName = "asset.json";
    private const string NotesFileName = "notes.md";

    private readonly string _dataPath;
    private readonly IIronLedgerSerializer _serializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileSystemAssetRepository"/> class.
    /// </summary>
    /// <param name="dataPath">The root directory under which asset folders are stored.</param>
    /// <param name="serializer">The serializer used to read and write asset JSON files.</param>
    public FileSystemAssetRepository(string dataPath, IIronLedgerSerializer serializer)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dataPath);
        ArgumentNullException.ThrowIfNull(serializer);
        _dataPath = dataPath;
        _serializer = serializer;
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<string>> GetAllIdentifiersAsync(CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            if (Directory.Exists(_dataPath))
            {
                var results = new List<string>();
                foreach (var dir in Directory.EnumerateDirectories(_dataPath))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var assetFile = Path.Combine(dir, AssetFileName);
                    if (File.Exists(assetFile))
                    {
                        var id = Path.GetFileName(dir);
                        CheckForValidId(id);
                        results.Add(id);
                    }
                }
                return (IReadOnlyList<string>)results;
            }
            return [];
        }, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<AssetRecord>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(_dataPath))
            return [];

        var results = new List<AssetRecord>();
        foreach (var dir in Directory.EnumerateDirectories(_dataPath))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var assetFile = Path.Combine(dir, AssetFileName);
            if (!File.Exists(assetFile))
                continue;
            var json = await File.ReadAllTextAsync(assetFile, cancellationToken);
            var record = _serializer.Deserialize<AssetRecord>(json);
            if (record is not null)
                results.Add(record);
        }
        return results;
    }

    /// <inheritdoc/>
    public Task<bool> ExistsAsync(AssetRecord asset, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(asset);
        cancellationToken.ThrowIfCancellationRequested();
        var dir = AssetDirectory(asset.Id.Id);
        var exists = Directory.Exists(dir) && File.Exists(AssetFilePath(asset.Id.Id));
        return Task.FromResult(exists);
    }

    /// <inheritdoc/>
    public async Task<AssetRecord?> GetAsync(string assetId, CancellationToken cancellationToken = default)
    {
        var assetFile = AssetFilePath(assetId);
        if (!File.Exists(assetFile))
            return null;
        var json = await File.ReadAllTextAsync(assetFile, cancellationToken);
        return _serializer.Deserialize<AssetRecord>(json);
    }

    /// <inheritdoc/>
    public async Task SaveAsync(AssetRecord asset, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(asset);
        var dir = AssetDirectory(asset.Id.Id);
        Directory.CreateDirectory(dir);
        var json = _serializer.Serialize(asset);
        await File.WriteAllTextAsync(AssetFilePath(asset.Id.Id), json, cancellationToken);
    }

    /// <inheritdoc/>
    public Task DeleteAsync(string assetId, CancellationToken cancellationToken = default)
    {
        var dir = AssetDirectory(assetId);
        if (Directory.Exists(dir))
            Directory.Delete(dir, recursive: true);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task<string> GetNotesAsync(string assetId, CancellationToken cancellationToken = default)
    {
        var notesFile = NotesFilePath(assetId);
        if (!File.Exists(notesFile))
            return string.Empty;
        return await File.ReadAllTextAsync(notesFile, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task SaveNotesAsync(string assetId, string markdown, CancellationToken cancellationToken = default)
    {
        var dir = AssetDirectory(assetId);
        Directory.CreateDirectory(dir);
        await File.WriteAllTextAsync(NotesFilePath(assetId), markdown, cancellationToken);
    }

    private string AssetDirectory(string assetId) => Path.Combine(_dataPath, CheckForValidId(assetId));
    private string AssetFilePath(string assetId) => Path.Combine(AssetDirectory(assetId), AssetFileName);
    private string NotesFilePath(string assetId) => Path.Combine(AssetDirectory(assetId), NotesFileName);

    private static string CheckForValidId(string id)
    {
        if (string.IsNullOrEmpty(id) || id.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            throw new FormatException($"'{id}' is not a valid asset identifier: contains invalid path characters.");

        if (id.Length != 64)
            throw new FormatException($"'{id}' is not a valid asset identifier: expected 64 hex characters, got {id.Length}.");

        foreach (var c in id)
        {
            if (c is not ((>= '0' and <= '9') or (>= 'a' and <= 'f')))
                throw new FormatException($"'{id}' is not a valid asset identifier: '{c}' is not a lowercase hex character.");
        }
        return id;
    }

}
