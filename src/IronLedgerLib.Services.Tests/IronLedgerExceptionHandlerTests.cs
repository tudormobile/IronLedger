using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace IronLedgerLib.Services.Tests;

[TestClass]
public class IronLedgerExceptionHandlerTests
{
    // --- TryHandleAsync return value ---

    [TestMethod]
    public async Task TryHandleAsync_ReturnsTrue()
    {
        var handler = CreateHandler();

        var result = await handler.TryHandleAsync(CreateContext(), new Exception(), CancellationToken.None);

        Assert.IsTrue(result);
    }

    // --- Status code mapping ---

    [TestMethod]
    public async Task TryHandleAsync_UnhandledException_Sets500StatusCode()
    {
        var handler = CreateHandler();
        var context = CreateContext();

        await handler.TryHandleAsync(context, new Exception(), CancellationToken.None);

        Assert.AreEqual(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
    }

    [TestMethod]
    public async Task TryHandleAsync_HttpRequestException_Sets502StatusCode()
    {
        var handler = CreateHandler();
        var context = CreateContext();

        await handler.TryHandleAsync(context, new HttpRequestException(), CancellationToken.None);

        Assert.AreEqual(StatusCodes.Status502BadGateway, context.Response.StatusCode);
    }

    [TestMethod]
    public async Task TryHandleAsync_TimeoutException_Sets504StatusCode()
    {
        var handler = CreateHandler();
        var context = CreateContext();

        await handler.TryHandleAsync(context, new TimeoutException(), CancellationToken.None);

        Assert.AreEqual(StatusCodes.Status504GatewayTimeout, context.Response.StatusCode);
    }

    // --- Logging ---

    [TestMethod]
    public async Task TryHandleAsync_LogsErrorEntry()
    {
        var logger = new CapturingLogger<IronLedgerExceptionHandler>();
        var handler = CreateHandler(logger);

        await handler.TryHandleAsync(CreateContext(), new Exception("test error"), CancellationToken.None);

        Assert.IsTrue(logger.HasEntries(LogLevel.Error));
    }

    // --- Helpers ---

    private static IronLedgerExceptionHandler CreateHandler(ILogger<IronLedgerExceptionHandler>? logger = null)
        => new(logger ?? NullLogger<IronLedgerExceptionHandler>.Instance, new MockProblemDetailsService());

    private static DefaultHttpContext CreateContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }

    [ExcludeFromCodeCoverage]
    private sealed class MockProblemDetailsService : IProblemDetailsService
    {
        public ValueTask WriteAsync(ProblemDetailsContext context) => ValueTask.CompletedTask;
    }

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
}
