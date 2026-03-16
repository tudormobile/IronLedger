using System.Net;
using Tudormobile.IronLedgerLib.Services;

namespace IronLedgerLib.Services.Tests;

[TestClass]
public class IronLedgerClientTests
{
    private static HttpClient MakeClient(string response, HttpStatusCode status = HttpStatusCode.OK)
        => new(new FakeHandler(response, status)) { BaseAddress = new Uri("https://myserver:5037/") };

    // --- IronLedgerClientOptions ---

    [TestMethod]
    public void IronLedgerClientOptions_DefaultServerUrl_IsNull()
        => Assert.IsNull(new IronLedgerClientOptions().ServerUrl);

    [TestMethod]
    public void IronLedgerClientOptions_ServerUrl_CanBeSetAndRetrieved()
    {
        var url = new Uri("https://myserver:5037");
        var options = new IronLedgerClientOptions { ServerUrl = url };

        Assert.AreEqual(url, options.ServerUrl);
    }

    // --- constructor ---

    [TestMethod]
    public void Constructor_NullHttpClient_Throws()
        => Assert.ThrowsExactly<ArgumentNullException>(() => new IronLedgerClient(null!));

    [TestMethod]
    public void Constructor_ValidHttpClient_CreatesInstance()
        => Assert.IsNotNull(new IronLedgerClient(new HttpClient()));

    // --- IIronLedgerClient.Create (no-DI scenario) ---

    [TestMethod]
    public void Create_ReturnsIIronLedgerClientInstance()
    {
        var client = IIronLedgerClient.Create(new HttpClient());

        Assert.IsNotNull(client);
        Assert.IsInstanceOfType<IIronLedgerClient>(client);
    }

    // --- GetStatusAsync ---

    [TestMethod]
    public async Task GetStatusAsync_RequestsRelativeStatusEndpoint()
    {
        var handler = new FakeHandler("{}", HttpStatusCode.OK);
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://myserver:5037/") };
        var client = IIronLedgerClient.Create(httpClient);

        await client.GetStatusAsync();

        Assert.AreEqual("https://myserver:5037/api/v1/status", handler.LastRequestUri?.ToString());
    }

    [TestMethod]
    public async Task GetStatusAsync_OnSuccess_IsSuccess()
    {
        var client = IIronLedgerClient.Create(MakeClient("{ \"version\": \"1.0\" }"));

        var result = await client.GetStatusAsync();

        Assert.IsTrue(result.IsSuccess);
    }

    [TestMethod]
    public async Task GetStatusAsync_OnSuccess_DataIsNotNull()
    {
        var client = IIronLedgerClient.Create(MakeClient("{ \"version\": \"1.0\" }"));

        var result = await client.GetStatusAsync();

        Assert.IsNotNull(result.Data);
    }

    [TestMethod]
    public async Task GetStatusAsync_OnHttpException_ReturnsFailure()
    {
        var handler = new ThrowingHandler();
        var client = IIronLedgerClient.Create(new HttpClient(handler) { BaseAddress = new Uri("https://myserver:5037/") });

        var result = await client.GetStatusAsync();

        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.ErrorMessage);
    }

    // --- helpers ---

    private sealed class FakeHandler(string content, HttpStatusCode status) : HttpMessageHandler
    {
        public Uri? LastRequestUri { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequestUri = request.RequestUri;
            return Task.FromResult(new HttpResponseMessage(status)
            {
                Content = new StringContent(content)
            });
        }
    }

    private sealed class ThrowingHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => throw new HttpRequestException("Network failure");
    }
}
