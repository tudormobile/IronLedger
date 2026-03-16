using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Tudormobile.IronLedgerLib.Serialization;

namespace IronLedgerLib.Services.Tests;

[TestClass]
public class IronLedgerServiceTests
{
    [TestMethod]
    public async Task GetStatusAsync_ReturnsNonNullResult()
    {
        var service = new IronLedgerService(NullLogger<IronLedgerService>.Instance, new HttpContextAccessor(), new IronLedgerJsonSerializer(), new NullAssetRepository());

        var result = await service.GetStatusAsync(CancellationToken.None);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task GetStatusAsync_LogsInformationEntry()
    {
        var logger = new CapturingLogger<IronLedgerService>();
        var service = new IronLedgerService(logger, new HttpContextAccessor(), new IronLedgerJsonSerializer(), new NullAssetRepository());

        await service.GetStatusAsync(CancellationToken.None);

        Assert.IsTrue(logger.HasEntries(LogLevel.Information));
    }

    [TestMethod]
    public async Task GetStatusAsync_WhenInformationLevelDisabled_DoesNotAccessHttpContext()
    {
        var accessor = new TrackingHttpContextAccessor();
        var service = new IronLedgerService(new DisabledLogger<IronLedgerService>(), accessor, new IronLedgerJsonSerializer(), new NullAssetRepository());

        await service.GetStatusAsync(CancellationToken.None);

        Assert.IsFalse(accessor.WasAccessed);
    }

    private sealed class CapturingLogger<T> : ILogger<T>
    {
        private readonly List<(LogLevel Level, string Message, Exception? Exception)> _entries = [];

        public bool HasEntries(LogLevel level) => _entries.Any(e => e.Level == level);

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            => _entries.Add((logLevel, formatter(state, exception), exception));
    }

    private sealed class DisabledLogger<T> : ILogger<T>
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => false;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }
    }

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

    private sealed class TrackingHttpContextAccessor : IHttpContextAccessor
    {
        public bool WasAccessed { get; private set; }
        public HttpContext? HttpContext
        {
            get { WasAccessed = true; return null; }
            set { }
        }
    }
}
