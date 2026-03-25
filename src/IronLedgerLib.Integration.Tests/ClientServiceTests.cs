using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;

namespace IronLedgerLib.Integration.Tests;

[TestCategory("Integration")]
[TestClass]
public class ClientServiceTests
{
    private static WebApplication _app = null!;
    private static IIronLedgerClient _client = null!;
    private static string _tempPath = null!;

    public TestContext TestContext { get; set; } = null!;

    [ClassInitialize]
    public static async Task ClassInitialize(TestContext _)
    {
        _tempPath = Path.Combine(Path.GetTempPath(), $"IronLedgerIntTest_{Guid.NewGuid():N}");

        var builder = WebApplication.CreateBuilder();
        builder.Services.AddIronLedgerService(o => o.DataPath = _tempPath);

        _app = builder.Build();
        _app.Urls.Add("http://127.0.0.1:0");
        _app.UseIronLedgerService();

        await _app.StartAsync();

        var address = _app.Services
            .GetRequiredService<IServer>()
            .Features.Get<IServerAddressesFeature>()!
            .Addresses.First();

        _client = IIronLedgerClient.Create(new HttpClient { BaseAddress = new Uri(address) });
    }

    [ClassCleanup]
    public static async Task ClassCleanup()
    {
        await _app.StopAsync();
        await _app.DisposeAsync();
        if (Directory.Exists(_tempPath))
            Directory.Delete(_tempPath, recursive: true);
    }

    [TestMethod]
    public async Task GetStatus_ReturnsSuccess()
    {
        var response = await _client.GetStatusAsync(TestContext.CancellationToken);

        Assert.IsTrue(response.IsSuccess);
        Assert.IsNotNull(response.Data);
    }

    [TestMethod]
    public async Task CreateAsset_NewAsset_ReturnsMatchingId()
    {
        var assetId = MakeUniqueAssetId();

        var response = await _client.CreateAssetAsync(assetId, TestContext.CancellationToken);

        Assert.IsTrue(response.IsSuccess, response.ErrorMessage);
        Assert.AreEqual(assetId.Id, response.Data);
    }

    [TestMethod]
    public async Task CreateAsset_DuplicateAsset_ReturnsFailure()
    {
        var assetId = MakeUniqueAssetId();
        await _client.CreateAssetAsync(assetId, TestContext.CancellationToken);

        var second = await _client.CreateAssetAsync(assetId, TestContext.CancellationToken);

        Assert.IsFalse(second.IsSuccess);
    }

    [TestMethod]
    public async Task GetAssetIds_AfterCreatingAsset_ContainsCreatedId()
    {
        var assetId = MakeUniqueAssetId();
        await _client.CreateAssetAsync(assetId, TestContext.CancellationToken);

        var response = await _client.GetAssetIdsAsync(TestContext.CancellationToken);

        Assert.IsTrue(response.IsSuccess, response.ErrorMessage);
        CollectionAssert.Contains(response.Data, assetId.Id);
    }

    [TestMethod]
    public async Task GetAsset_AfterCreating_ReturnsMatchingRecord()
    {
        var assetId = MakeUniqueAssetId();
        await _client.CreateAssetAsync(assetId, TestContext.CancellationToken);

        var response = await _client.GetAssetAsync(assetId.Id, TestContext.CancellationToken);

        Assert.IsTrue(response.IsSuccess, response.ErrorMessage);
        Assert.AreEqual(assetId.Id, response.Data?.Id.Id);
    }

    private static AssetId MakeUniqueAssetId() => new()
    {
        SystemMetadata = new AssetMetadata { SerialNumber = Guid.NewGuid().ToString("N"), Manufacturer = "IntegrationTest", Product = "TestProduct" },
        BaseBoardMetadata = AssetMetadata.Empty,
        BiosMetadata = AssetMetadata.Empty,
    };
}
