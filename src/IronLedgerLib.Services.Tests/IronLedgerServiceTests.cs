using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using Tudormobile.IronLedgerLib.Serialization;

namespace IronLedgerLib.Services.Tests;

[TestClass]
public class IronLedgerServiceTests
{
    public TestContext TestContext { get; set; }    // MSTest will set this property 

    [TestMethod]
    public async Task IronLedgerService_GetStatusAsync_ReturnsNonNullResult()
    {
        // Arrange
        var service = new IronLedgerService(NullLogger<IronLedgerService>.Instance, new HttpContextAccessor(), new IronLedgerJsonSerializer(), new NullAssetRepository());

        // Act
        var result = await service.GetStatusAsync(TestContext.CancellationToken);

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
        await service.GetStatusAsync(TestContext.CancellationToken);

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
        await service.GetStatusAsync(TestContext.CancellationToken);

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
        var result = await service.IngestAssetAsync(MakeBodyStream(serializer.Serialize(assetId)), TestContext.CancellationToken);

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
        var result = await service.IngestAssetAsync(MakeBodyStream(serializer.Serialize(assetId)), TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(409, ((IStatusCodeHttpResult)result).StatusCode);
    }

    [TestMethod]
    public async Task IngestAssetAsync_WithInvalidPayload_ReturnsBadRequest()
    {
        // Arrange
        var service = new IronLedgerService(NullLogger<IronLedgerService>.Instance, new HttpContextAccessor(), new NullReturningSerializer(), new NullAssetRepository());

        // Act
        var result = await service.IngestAssetAsync(MakeBodyStream("{}"), TestContext.CancellationToken);

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
        var result = await service.GetAssetAsync(null, TestContext.CancellationToken);

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
        var result = await service.GetAssetAsync(assetId.Id, TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(200, ((IStatusCodeHttpResult)result).StatusCode);
    }

    [TestMethod]
    public async Task GetAssetAsync_WithUnknownAssetId_ReturnsNotFound()
    {
        // Arrange
        var service = new IronLedgerService(NullLogger<IronLedgerService>.Instance, new HttpContextAccessor(), new IronLedgerJsonSerializer(), new NullAssetRepository());

        // Act
        var result = await service.GetAssetAsync("aabbccdd11223344aabbccdd11223344aabbccdd11223344aabbccdd11223344", TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(404, ((IStatusCodeHttpResult)result).StatusCode);
    }

    [TestMethod]
    public async Task GetComponentsAsync_WithValidAsset_ReturnsOk()
    {
        // Arrange
        var serializer = new IronLedgerJsonSerializer();
        var assetId = CreateAssetId();
        var record = CreateRecord(assetId);
        var service = new IronLedgerService(NullLogger<IronLedgerService>.Instance, new HttpContextAccessor(), serializer, new StubAssetRepository { GetResult = record });

        // Act
        var result = await service.GetComponentsAsync(assetId.Id, TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(200, ((IStatusCodeHttpResult)result).StatusCode);
    }

    [TestMethod]
    public async Task GetAssetAsync_WithInValidAssetId_ReturnsBadRequest()
    {
        // Arrange
        var serializer = new IronLedgerJsonSerializer();
        var assetId = CreateAssetId();
        var record = CreateRecord(assetId);
        var service = new IronLedgerService(NullLogger<IronLedgerService>.Instance, new HttpContextAccessor(), serializer, new StubAssetRepository { GetResult = record });

        // Act
        var result = await service.GetAssetAsync("", TestContext.CancellationToken);

        // Assert
        Assert.AreEqual((int)HttpStatusCode.BadRequest, ((IStatusCodeHttpResult)result).StatusCode);
    }

    [TestMethod]
    public async Task GetComponentsAsync_WithInValidAssetId_ReturnsBadRequest()
    {
        // Arrange
        var serializer = new IronLedgerJsonSerializer();
        var assetId = CreateAssetId();
        var record = CreateRecord(assetId);
        var service = new IronLedgerService(NullLogger<IronLedgerService>.Instance, new HttpContextAccessor(), serializer, new StubAssetRepository { GetResult = record });

        // Act
        var result = await service.GetComponentsAsync("bad-asset-id", TestContext.CancellationToken);

        // Assert
        Assert.AreEqual((int)HttpStatusCode.BadRequest, ((IStatusCodeHttpResult)result).StatusCode);
    }

    [TestMethod]
    public async Task GetComponentsAsync_WithInValidAsset_ReturnsNotFound()
    {
        // Arrange
        var serializer = new IronLedgerJsonSerializer();
        var assetId = CreateAssetId();
        var service = new IronLedgerService(NullLogger<IronLedgerService>.Instance, new HttpContextAccessor(), serializer, new NullAssetRepository());

        // Act
        var result = await service.GetComponentsAsync(assetId.Id, TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(404, ((IStatusCodeHttpResult)result).StatusCode);
    }

    [TestMethod]
    public async Task UpdateComponentsAsync_WithValidAsset_ReturnsOk()
    {
        // Arrange
        var serializer = new IronLedgerJsonSerializer();
        var assetId = CreateAssetId();
        var record = CreateRecord(assetId);
        var service = new IronLedgerService(NullLogger<IronLedgerService>.Instance, new HttpContextAccessor(), serializer, new StubAssetRepository { GetResult = record });

        // Act
        var result = await service.UpdateComponentsAsync(record.Id.Id, MakeBodyStream(serializer.Serialize(record.Components)), TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(200, ((IStatusCodeHttpResult)result).StatusCode);
    }

    [TestMethod]
    public async Task UpdateComponentsAsync_WithMissingRecord_ReturnsNotFound()
    {
        // Arrange
        var serializer = new IronLedgerJsonSerializer();
        var assetId = CreateAssetId();
        var record = CreateRecord(assetId);
        var service = new IronLedgerService(NullLogger<IronLedgerService>.Instance, new HttpContextAccessor(), serializer, new NullAssetRepository());

        // Act
        var result = await service.UpdateComponentsAsync(assetId.Id, MakeBodyStream(serializer.Serialize(record.Components)), TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(404, ((IStatusCodeHttpResult)result).StatusCode);
    }

    [TestMethod]
    public async Task UpdateComponentsAsync_WithBadComponentData_ReturnsBadRequest()
    {
        // Arrange
        var serializer = new NullReturningSerializer();
        var assetId = CreateAssetId();
        var record = CreateRecord(assetId);
        var service = new IronLedgerService(NullLogger<IronLedgerService>.Instance, new HttpContextAccessor(), serializer, new StubAssetRepository { GetResult = record });

        // Act
        var result = await service.UpdateComponentsAsync(record.Id.Id, MakeBodyStream("bad-component-data"), TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(400, ((IStatusCodeHttpResult)result).StatusCode);
    }

    [TestMethod]
    public async Task UpdateComponentsAsync_WithInValidAssetId_ReturnsBadRequest()
    {
        // Arrange
        var serializer = new IronLedgerJsonSerializer();
        var assetId = CreateAssetId();
        var record = CreateRecord(assetId);
        var service = new IronLedgerService(NullLogger<IronLedgerService>.Instance, new HttpContextAccessor(), serializer, new StubAssetRepository { GetResult = record });

        // Act
        var result = await service.UpdateComponentsAsync("invalid-record-id", MakeBodyStream(serializer.Serialize(record.Components)), TestContext.CancellationToken);

        // Assert
        Assert.AreEqual((int)HttpStatusCode.BadRequest, ((IStatusCodeHttpResult)result).StatusCode);
    }


    [TestMethod]
    public async Task GetNotesAsync_WithValidAsset_ReturnsOk()
    {
        // Arrange
        var serializer = new IronLedgerJsonSerializer();
        var assetId = CreateAssetId();
        var record = CreateRecord(assetId);
        var service = new IronLedgerService(NullLogger<IronLedgerService>.Instance, new HttpContextAccessor(), serializer, new StubAssetRepository { GetResult = record });

        // Act
        var result = await service.GetNotesAsync(assetId.Id, TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(200, ((IStatusCodeHttpResult)result).StatusCode);
    }

    [TestMethod]
    public async Task GetNotesAsync_WithInvalidAssetId_ReturnsBadRequest()
    {
        // Arrange
        var serializer = new IronLedgerJsonSerializer();
        var assetId = CreateAssetId();
        var record = CreateRecord(assetId);
        var service = new IronLedgerService(NullLogger<IronLedgerService>.Instance, new HttpContextAccessor(), serializer, new StubAssetRepository { GetResult = record });

        // Act
        var result = await service.GetNotesAsync("bad-asset-id", TestContext.CancellationToken);

        // Assert
        Assert.AreEqual((int)HttpStatusCode.BadRequest, ((IStatusCodeHttpResult)result).StatusCode);
    }

    [TestMethod]
    public async Task GetNotesAsync_WithMissingAsset_ReturnsOkWithEmptyNotes()
    {
        // Arrange
        var serializer = new IronLedgerJsonSerializer();
        var assetId = CreateAssetId();
        var record = CreateRecord(assetId);
        var service = new IronLedgerService(NullLogger<IronLedgerService>.Instance, new HttpContextAccessor(), serializer, new NullAssetRepository());

        // Act
        var result = await service.GetNotesAsync(assetId.Id, TestContext.CancellationToken);

        // Assert
        Assert.AreEqual((int)HttpStatusCode.OK, ((IStatusCodeHttpResult)result).StatusCode);
        var value = ((IValueHttpResult<string>)result).Value;
        Assert.AreEqual(string.Empty, value);
    }

    [TestMethod]
    public async Task UpdateNotesAsync_WithValidAsset_ReturnsOk()
    {
        // Arrange
        var serializer = new IronLedgerJsonSerializer();
        var assetId = CreateAssetId();
        var record = CreateRecord(assetId);
        var service = new IronLedgerService(NullLogger<IronLedgerService>.Instance, new HttpContextAccessor(), serializer, new StubAssetRepository { GetResult = record });
        var markdown = "this is notes";

        // Act
        var result = await service.UpdateNotesAsync(assetId.Id, MakeBodyStream(markdown), TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(200, ((IStatusCodeHttpResult)result).StatusCode);
    }

    [TestMethod]
    public async Task UpdateNotesAsync_WithInValidAssetId_ReturnsBadRequest()
    {
        // Arrange
        var serializer = new IronLedgerJsonSerializer();
        var assetId = CreateAssetId();
        var record = CreateRecord(assetId);
        var service = new IronLedgerService(NullLogger<IronLedgerService>.Instance, new HttpContextAccessor(), serializer, new StubAssetRepository { GetResult = record });
        var markdown = "this is notes";

        // Act
        var result = await service.UpdateNotesAsync("invalid-id", MakeBodyStream(markdown), TestContext.CancellationToken);

        // Assert
        Assert.AreEqual((int)HttpStatusCode.BadRequest, ((IStatusCodeHttpResult)result).StatusCode);
    }

    // --- Exception handling: IOException from body stream ---

    [TestMethod]
    public async Task IngestAssetAsync_WhenBodyThrowsIOException_ReturnsBadRequest()
    {
        // Arrange
        var service = new IronLedgerService(NullLogger<IronLedgerService>.Instance, new HttpContextAccessor(), new IronLedgerJsonSerializer(), new NullAssetRepository());

        // Act
        var result = await service.IngestAssetAsync(new ThrowingStream(), TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(400, ((IStatusCodeHttpResult)result).StatusCode);
    }

    [TestMethod]
    public async Task UpdateComponentsAsync_WhenBodyThrowsIOException_ReturnsBadRequest()
    {
        // Arrange
        var assetId = CreateAssetId();
        var service = new IronLedgerService(NullLogger<IronLedgerService>.Instance, new HttpContextAccessor(), new IronLedgerJsonSerializer(), new NullAssetRepository());

        // Act
        var result = await service.UpdateComponentsAsync(assetId.Id, new ThrowingStream(), TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(400, ((IStatusCodeHttpResult)result).StatusCode);
    }

    [TestMethod]
    public async Task UpdateNotesAsync_WhenBodyThrowsIOException_ReturnsBadRequest()
    {
        // Arrange
        var assetId = CreateAssetId();
        var service = new IronLedgerService(NullLogger<IronLedgerService>.Instance, new HttpContextAccessor(), new IronLedgerJsonSerializer(), new NullAssetRepository());

        // Act
        var result = await service.UpdateNotesAsync(assetId.Id, new ThrowingStream(), TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(400, ((IStatusCodeHttpResult)result).StatusCode);
    }

    // --- Exception handling: repository throws from no-body methods ---

    [TestMethod]
    public async Task GetAssetAsync_WithNullId_WhenRepositoryThrows_ReturnsProblem()
    {
        // Arrange
        var service = new IronLedgerService(NullLogger<IronLedgerService>.Instance, new HttpContextAccessor(), new IronLedgerJsonSerializer(), new ThrowingAssetRepository());

        // Act
        var result = await service.GetAssetAsync(null, TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(500, ((IStatusCodeHttpResult)result).StatusCode);
    }

    [TestMethod]
    public async Task GetAssetAsync_WhenRepositoryThrows_ReturnsProblem()
    {
        // Arrange
        var assetId = CreateAssetId();
        var service = new IronLedgerService(NullLogger<IronLedgerService>.Instance, new HttpContextAccessor(), new IronLedgerJsonSerializer(), new ThrowingAssetRepository());

        // Act
        var result = await service.GetAssetAsync(assetId.Id, TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(500, ((IStatusCodeHttpResult)result).StatusCode);
    }

    [TestMethod]
    public async Task GetComponentsAsync_WhenRepositoryThrows_ReturnsProblem()
    {
        // Arrange
        var assetId = CreateAssetId();
        var service = new IronLedgerService(NullLogger<IronLedgerService>.Instance, new HttpContextAccessor(), new IronLedgerJsonSerializer(), new ThrowingAssetRepository());

        // Act
        var result = await service.GetComponentsAsync(assetId.Id, TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(500, ((IStatusCodeHttpResult)result).StatusCode);
    }

    [TestMethod]
    public async Task GetNotesAsync_WhenRepositoryThrows_ReturnsProblem()
    {
        // Arrange
        var assetId = CreateAssetId();
        var service = new IronLedgerService(NullLogger<IronLedgerService>.Instance, new HttpContextAccessor(), new IronLedgerJsonSerializer(), new ThrowingAssetRepository());

        // Act
        var result = await service.GetNotesAsync(assetId.Id, TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(500, ((IStatusCodeHttpResult)result).StatusCode);
    }

    // --- Exception handling: repository throws from body methods ---

    [TestMethod]
    public async Task IngestAssetAsync_WhenRepositoryThrows_ReturnsProblem()
    {
        // Arrange
        var serializer = new IronLedgerJsonSerializer();
        var assetId = CreateAssetId();
        var service = new IronLedgerService(NullLogger<IronLedgerService>.Instance, new HttpContextAccessor(), serializer, new ThrowingAssetRepository());

        // Act
        var result = await service.IngestAssetAsync(MakeBodyStream(serializer.Serialize(assetId)), TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(500, ((IStatusCodeHttpResult)result).StatusCode);
    }

    [TestMethod]
    public async Task UpdateComponentsAsync_WhenRepositoryThrows_ReturnsProblem()
    {
        // Arrange
        var serializer = new IronLedgerJsonSerializer();
        var assetId = CreateAssetId();
        var record = CreateRecord(assetId);
        var service = new IronLedgerService(NullLogger<IronLedgerService>.Instance, new HttpContextAccessor(), serializer, new ThrowingAssetRepository());

        // Act
        var result = await service.UpdateComponentsAsync(assetId.Id, MakeBodyStream(serializer.Serialize(record.Components)), TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(500, ((IStatusCodeHttpResult)result).StatusCode);
    }

    [TestMethod]
    public async Task UpdateNotesAsync_WhenRepositoryThrows_ReturnsProblem()
    {
        // Arrange
        var assetId = CreateAssetId();
        var service = new IronLedgerService(NullLogger<IronLedgerService>.Instance, new HttpContextAccessor(), new IronLedgerJsonSerializer(), new ThrowingAssetRepository());

        // Act
        var result = await service.UpdateNotesAsync(assetId.Id, MakeBodyStream("some notes"), TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(500, ((IStatusCodeHttpResult)result).StatusCode);
    }

    // --- Exception handling: errors are logged ---

    [TestMethod]
    public async Task GetAssetAsync_WhenRepositoryThrows_LogsError()
    {
        // Arrange
        var logger = new CapturingLogger<IronLedgerService>();
        var assetId = CreateAssetId();
        var service = new IronLedgerService(logger, new HttpContextAccessor(), new IronLedgerJsonSerializer(), new ThrowingAssetRepository());

        // Act
        await service.GetAssetAsync(assetId.Id, TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(logger.HasEntries(LogLevel.Error));
    }

    [TestMethod]
    public async Task UpdateNotesAsync_WhenBodyThrowsIOException_LogsError()
    {
        // Arrange
        var logger = new CapturingLogger<IronLedgerService>();
        var assetId = CreateAssetId();
        var service = new IronLedgerService(logger, new HttpContextAccessor(), new IronLedgerJsonSerializer(), new NullAssetRepository());

        // Act
        await service.UpdateNotesAsync(assetId.Id, new ThrowingStream(), TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(logger.HasEntries(LogLevel.Error));
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
        public Task<bool> ExistsAsync(AssetRecord asset, CancellationToken cancellationToken = default)
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
        public Task<bool> ExistsAsync(AssetRecord asset, CancellationToken cancellationToken = default)
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

    [ExcludeFromCodeCoverage]
    private sealed class ThrowingStream : Stream
    {
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position { get => 0; set => throw new NotSupportedException(); }
        public override void Flush() { }
        public override int Read(byte[] buffer, int offset, int count) => throw new IOException("Simulated I/O failure.");
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
            => throw new IOException("Simulated I/O failure.");
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }

    [ExcludeFromCodeCoverage]
    private sealed class ThrowingAssetRepository : IAssetRepository
    {
        public Task<IReadOnlyList<string>> GetAllIdentifiersAsync(CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("Simulated repository failure.");
        public Task<IReadOnlyList<AssetRecord>> GetAllAsync(CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("Simulated repository failure.");
        public Task<AssetRecord?> GetAsync(string assetId, CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("Simulated repository failure.");
        public Task<bool> ExistsAsync(AssetRecord asset, CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("Simulated repository failure.");
        public Task SaveAsync(AssetRecord asset, CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("Simulated repository failure.");
        public Task DeleteAsync(string assetId, CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("Simulated repository failure.");
        public Task<string> GetNotesAsync(string assetId, CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("Simulated repository failure.");
        public Task SaveNotesAsync(string assetId, string markdown, CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("Simulated repository failure.");
    }
}
