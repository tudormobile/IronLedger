using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace IronLedgerLib.Services.Tests;

[TestClass]
public class IronLedgerServiceTests
{
    [TestMethod]
    public async Task GetStatusAsync_ReturnsNonNullResult()
    {
        var service = new IronLedgerService(NullLogger<IronLedgerService>.Instance);

        var result = await service.GetStatusAsync(new DefaultHttpContext(), CancellationToken.None);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task GetStatusAsync_LogsInformationEntry()
    {
        var logger = new CapturingLogger<IronLedgerService>();
        var service = new IronLedgerService(logger);

        await service.GetStatusAsync(new DefaultHttpContext(), CancellationToken.None);

        Assert.IsTrue(logger.HasEntries(LogLevel.Information));
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
}
