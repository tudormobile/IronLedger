using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Tudormobile.IronLedgerLib.Serialization;

namespace IronLedgerLib.Services.Tests;

[TestClass]
public class IronLedgerServiceTests
{
    [TestMethod]
    public async Task IronLedgerService_GetStatusAsync_ReturnsNonNullResult()
    {
        // Arrange
        var service = new IronLedgerService(NullLogger<IronLedgerService>.Instance, new HttpContextAccessor(), new IronLedgerJsonSerializer(), new NullAssetRepository());

        // Act
        var result = await service.GetStatusAsync(CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task IronLedgerService_GetStatusAsync_LogsInformationEntry()
    {
        // Arrange
        var logger = new CapturingLogger<IronLedgerService>();
        var service = new IronLedgerService(logger, new HttpContextAccessor(), new IronLedgerJsonSerializer(), new NullAssetRepository());

        // Act
        await service.GetStatusAsync(CancellationToken.None);

        // Assert
        Assert.IsTrue(logger.HasEntries(LogLevel.Information));
    }

    [TestMethod]
    public async Task IronLedgerService_GetStatusAsync_WhenInformationLevelDisabled_DoesNotAccessHttpContext()
    {
        // Arrange
        var accessor = new TrackingHttpContextAccessor();
        var service = new IronLedgerService(new DisabledLogger<IronLedgerService>(), accessor, new IronLedgerJsonSerializer(), new NullAssetRepository());

        // Act
        await service.GetStatusAsync(CancellationToken.None);

        // Assert
        Assert.IsFalse(accessor.WasAccessed);
    }

    // --- IngestAssetAsync ---

    [TestMethod]
    public async Task IngestAssetAsync_WithValidNewAsset_ReturnsOk()
    {
        // Arrange
        var serializer = new IronLedgerJsonSerializer();
        var assetId = CreateAssetId();
        var service = new IronLedgerService(NullLogger<IronLedgerService>.Instance, new HttpContextAccessor(), serializer, new NullAssetRepository());

        // Act
        var result = await service.IngestAssetAsync(MakeBodyStream(serializer.Serialize(assetId)), CancellationToken.None);

        // Assert
        Assert.AreEqual(200, ((IStatusCodeHttpResult)result).StatusCode);
    }

    [TestMethod]
    public async Task IngestAssetAsync_WithExistingAsset_ReturnsConflict()
    {
        // Arrange
        var serializer = new IronLedgerJsonSerializer();
        var assetId = CreateAssetId();
        var service = new IronLedgerService(NullLogger<IronLedgerService>.Instance, new HttpContextAccessor(), serializer, new StubAssetRepository { ExistsResult = true });

        // Act
        var result = await service.IngestAssetAsync(MakeBodyStream(serializer.Serialize(assetId)), CancellationToken.None);

        // Assert
        Assert.AreEqual(409, ((IStatusCodeHttpResult)result).StatusCode);
    }

    [TestMethod]
    public async Task IngestAssetAsync_WithInvalidPayload_ReturnsBadRequest()
    {
        // Arrange
        var service = new IronLedgerService(NullLogger<IronLedgerService>.Instance, new HttpContextAccessor(), new NullReturningSerializer(), new NullAssetRepository());

        // Act
        var result = await service.IngestAssetAsync(MakeBodyStream("{}"), CancellationToken.None);

        // Assert
        Assert.AreEqual(400, ((IStatusCodeHttpResult)result).StatusCode);
    }

    // --- GetAssetAsync ---

    [TestMethod]
    public async Task GetAssetAsync_WithNullAssetId_ReturnsOkWithIdentifiers()
    {
        // Arrange
        IReadOnlyList<string> ids = ["aabbccdd11223344aabbccdd11223344aabbccdd11223344aabbccdd11223344"];
        var service = new IronLedgerService(NullLogger<IronLedgerService>.Instance, new HttpContextAccessor(), new IronLedgerJsonSerializer(), new StubAssetRepository { IdentifiersResult = ids });

        // Act
        var result = await service.GetAssetAsync(null, CancellationToken.None);

        // Assert
        Assert.AreEqual(200, ((IStatusCodeHttpResult)result).StatusCode);
        var value = ((IValueHttpResult<string[]>)result).Value;
        Assert.IsNotNull(value);
        Assert.HasCount(1, value);
        Assert.AreEqual(ids[0], value[0]);
    }

    [TestMethod]
    public async Task GetAssetAsync_WithKnownAssetId_ReturnsOk()
    {
        // Arrange
        var assetId = CreateAssetId();
        var record = CreateRecord(assetId);
        var service = new IronLedgerService(NullLogger<IronLedgerService>.Instance, new HttpContextAccessor(), new IronLedgerJsonSerializer(), new StubAssetRepository { GetResult = record });

        // Act
        var result = await service.GetAssetAsync(assetId.Id, CancellationToken.None);

        // Assert
        Assert.AreEqual(200, ((IStatusCodeHttpResult)result).StatusCode);
    }

    [TestMethod]
    public async Task GetAssetAsync_WithUnknownAssetId_ReturnsNotFound()
    {
        // Arrange
        var service = new IronLedgerService(NullLogger<IronLedgerService>.Instance, new HttpContextAccessor(), new IronLedgerJsonSerializer(), new NullAssetRepository());

        // Act
        var result = await service.GetAssetAsync("aabbccdd11223344aabbccdd11223344aabbccdd11223344aabbccdd11223344", CancellationToken.None);

        // Assert
        Assert.AreEqual(404, ((IStatusCodeHttpResult)result).StatusCode);
    }

    private static AssetId CreateAssetId() => new()
    {
        SystemMetadata = AssetMetadata.Empty,
        BaseBoardMetadata = AssetMetadata.Empty,
        BiosMetadata = AssetMetadata.Empty,
    };

    private static AssetRecord CreateRecord(AssetId assetId) => new()
    {
        Id = assetId,
        Components = new SystemComponentData { System = ComponentData.Empty, Processors = [], Memory = [], Disks = [] }
    };

    private static Stream MakeBodyStream(string json)
        => new MemoryStream(Encoding.UTF8.GetBytes(json));

    [ExcludeFromCodeCoverage]
    private sealed class CapturingLogger<T> : ILogger<T>
    {
        private readonly List<(LogLevel Level, string Message, Exception? Exception)> _entries = [];

        public bool HasEntries(LogLevel level) => _entries.Any(e => e.Level == level);

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            => _entries.Add((logLevel, formatter(state, exception), exception));
    }

    [ExcludeFromCodeCoverage]
    private sealed class DisabledLogger<T> : ILogger<T>
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => false;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }
    }

    [ExcludeFromCodeCoverage]
    private sealed class NullAssetRepository : IAssetRepository
    {
        public Task<IReadOnlyList<string>> GetAllIdentifiersAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<string>>([]);
        public Task<IReadOnlyList<AssetRecord>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<AssetRecord>>([]);
        public Task<AssetRecord?> GetAsync(string assetId, CancellationToken cancellationToken = default)
            => Task.FromResult<AssetRecord?>(null);
        public Task<bool> Exists(AssetRecord asset, CancellationToken cancellationToken = default)
            => Task.FromResult(false);
        public Task SaveAsync(AssetRecord asset, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
        public Task DeleteAsync(string assetId, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
        public Task<string> GetNotesAsync(string assetId, CancellationToken cancellationToken = default)
            => Task.FromResult(string.Empty);
        public Task SaveNotesAsync(string assetId, string markdown, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }

    [ExcludeFromCodeCoverage]
    private sealed class TrackingHttpContextAccessor : IHttpContextAccessor
    {
        public bool WasAccessed { get; private set; }
        public HttpContext? HttpContext
        {
            get { WasAccessed = true; return null; }
            set { }
        }
    }

    [ExcludeFromCodeCoverage]
    private sealed class StubAssetRepository : IAssetRepository
    {
        public bool ExistsResult { get; init; } = false;
        public AssetRecord? GetResult { get; init; } = null;
        public IReadOnlyList<string> IdentifiersResult { get; init; } = [];

        public Task<IReadOnlyList<string>> GetAllIdentifiersAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(IdentifiersResult);
        public Task<IReadOnlyList<AssetRecord>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<AssetRecord>>([]);
        public Task<AssetRecord?> GetAsync(string assetId, CancellationToken cancellationToken = default)
            => Task.FromResult(GetResult);
        public Task<bool> Exists(AssetRecord asset, CancellationToken cancellationToken = default)
            => Task.FromResult(ExistsResult);
        public Task SaveAsync(AssetRecord asset, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
        public Task DeleteAsync(string assetId, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
        public Task<string> GetNotesAsync(string assetId, CancellationToken cancellationToken = default)
            => Task.FromResult(string.Empty);
        public Task SaveNotesAsync(string assetId, string markdown, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }

    [ExcludeFromCodeCoverage]
    private sealed class NullReturningSerializer : IIronLedgerSerializer
    {
        public string ContentType => "application/json";
        public string Serialize<T>(T value) => "{}";
        public T? Deserialize<T>(string data) => default;
    }
}
