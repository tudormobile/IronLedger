using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace IronLedgerLib.Services.Tests;

[TestClass]
public class IronLedgerClientTests
{
    private static HttpClient MakeClient(string response, HttpStatusCode status = HttpStatusCode.OK)
        => new(new FakeHandler(response, status)) { BaseAddress = new Uri("https://myserver:5037/") };

    public TestContext TestContext { get; set; }    // MSTest will set this property 

    [TestMethod]
    public void IronLedgerClient_Constructor_NullHttpClient_Throws()
    => Assert.ThrowsExactly<ArgumentNullException>(() => new IronLedgerClient(null!));

    [TestMethod]
    public void IronLedgerClient_Constructor_ValidHttpClient_CreatesInstance()
        => Assert.IsNotNull(new IronLedgerClient(new HttpClient() { BaseAddress = new Uri("http://www.examplecom") }));

    [TestMethod]
    public void IronLedgerClient_Create_ReturnsIIronLedgerClientInstance()
    {
        using var httpClient = new HttpClient() { BaseAddress = new Uri("http://www.examplecom") };
        var client = IIronLedgerClient.Create(httpClient);

        Assert.IsNotNull(client);
        Assert.IsInstanceOfType<IIronLedgerClient>(client);
    }

    [TestMethod]
    public async Task IronLedgerClient_CreateAssetAsync_ThrowsIfNull()
    {
        // Arrange
        using var httpClient = new HttpClient(new MockHttpMessageHandler()) { BaseAddress = new Uri("http://www.examplecom") };
        var client = new IronLedgerClient(httpClient);

        // Act & Assert
        var exception = await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => client.CreateAssetAsync(null!, TestContext.CancellationToken));
        Assert.IsNotNull(exception.ParamName);
        Assert.Contains("assetId", exception.ParamName);
    }

    [TestMethod]
    public async Task IronLedgerClient_CreateAssetAsync_WithErrorResponse_ReturnsError()
    {
        // Arrange
        using var httpClient = new HttpClient(new MockHttpMessageHandler() { AlwaysResponds = new HttpResponseMessage(HttpStatusCode.InternalServerError), })
        {
            BaseAddress = new Uri("http://www.example.com/")
        };
        var client = new IronLedgerClient(httpClient);
        var assetId = new AssetId()
        {
            BaseBoardMetadata = AssetMetadata.Empty,
            BiosMetadata = AssetMetadata.Empty,
            SystemMetadata = AssetMetadata.Empty,
        };

        // Act
        var response = await client.CreateAssetAsync(assetId, TestContext.CancellationToken);

        // Assert
        Assert.IsFalse(response.IsSuccess);
        Assert.IsNotNull(response.ErrorMessage);
        Assert.Contains("500", response.ErrorMessage);
    }

    [TestMethod]
    public async Task IronLedgerClient_CreateAssetAsync_WithNoResponseContent_ReturnsError()
    {
        // Arrange
        using var httpClient = new HttpClient(new MockHttpMessageHandler() { AlwaysResponds = new HttpResponseMessage(HttpStatusCode.OK), })
        {
            BaseAddress = new Uri("http://www.example.com/")
        };
        var client = new IronLedgerClient(httpClient);
        var assetId = new AssetId()
        {
            BaseBoardMetadata = AssetMetadata.Empty,
            BiosMetadata = AssetMetadata.Empty,
            SystemMetadata = AssetMetadata.Empty,
        };

        // Act
        var response = await client.CreateAssetAsync(assetId, TestContext.CancellationToken);

        // Assert
        Assert.IsFalse(response.IsSuccess);
    }

    [TestMethod]
    public async Task IronLedgerClient_CreateAssetAsync_WithInvalidResponseContent_ReturnsError()
    {
        // Arrange
        using var httpClient = new HttpClient(new MockHttpMessageHandler() { JsonResponse = "{}" })
        {
            BaseAddress = new Uri("http://www.example.com/")
        };
        var client = new IronLedgerClient(httpClient);
        var assetId = new AssetId()
        {
            BaseBoardMetadata = AssetMetadata.Empty,
            BiosMetadata = AssetMetadata.Empty,
            SystemMetadata = AssetMetadata.Empty,
        };

        // Act
        var response = await client.CreateAssetAsync(assetId, TestContext.CancellationToken);

        // Assert
        Assert.IsFalse(response.IsSuccess);
    }

    [TestMethod]
    public async Task IronLedgerClient_CreateAssetAsync_WithValidResponseContent_ReturnsSuccessAndId()
    {
        // Arrange
        var assetId = new AssetId()
        {
            BaseBoardMetadata = AssetMetadata.Empty,
            BiosMetadata = AssetMetadata.Empty,
            SystemMetadata = AssetMetadata.Empty,
        };
        var handler = new MockHttpMessageHandler() { JsonResponse = $"\"{assetId.Id}\"" };
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://www.example.com/") };
        var client = new IronLedgerClient(httpClient);

        // Act
        var response = await client.CreateAssetAsync(assetId, TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(response.IsSuccess);
        Assert.AreEqual(response.Data, assetId.Id);
        Assert.AreEqual((new Uri("http://www.example.com/api/v1/assets/ingest")), handler.ProvidedRequestUri);
    }

    [TestMethod]
    public async Task IronLedgerClient_CreateAssetAsync_WithMismatchedResponseContent_ReturnsFailure()
    {
        // Arrange
        var assetId = new AssetId()
        {
            BaseBoardMetadata = AssetMetadata.Empty,
            BiosMetadata = AssetMetadata.Empty,
            SystemMetadata = AssetMetadata.Empty,
        };
        var handler = new MockHttpMessageHandler() { JsonResponse = $"\"0000000000000000000000000000000000000000000000000000000000000000\"" };
        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://www.example.com/")
        };
        var client = new IronLedgerClient(httpClient);

        // Act
        var response = await client.CreateAssetAsync(assetId, TestContext.CancellationToken);

        // Assert
        Assert.IsFalse(response.IsSuccess);
        Assert.AreEqual((new Uri("http://www.example.com/api/v1/assets/ingest")), handler.ProvidedRequestUri);

    }

    [TestMethod]
    public async Task IronLedgerClient_GetAssetAsync_WithNullAssetId_Throws()
    {
        // Arrange
        using var httpClient = new HttpClient(new MockHttpMessageHandler() { AlwaysResponds = new HttpResponseMessage(HttpStatusCode.OK) })
        {
            BaseAddress = new Uri("http://www.example.com/")
        };
        var client = new IronLedgerClient(httpClient);

        // Act
        var exception = await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => client.GetAssetAsync(null!, TestContext.CancellationToken));

        // Assert
        Assert.Contains("assetIdString", exception.Message);
    }

    [TestMethod]
    public async Task IronLedgerClient_GetAssetAsync_WithEmptyAssetId_Throws()
    {
        // Arrange
        using var httpClient = new HttpClient(new MockHttpMessageHandler()
        {
            AlwaysResponds = new HttpResponseMessage(HttpStatusCode.OK)
        })
        { BaseAddress = new Uri("http://www.example.com/") };

        var client = new IronLedgerClient(httpClient);
        var assetId = string.Empty;

        // Act
        var exception = await Assert.ThrowsExactlyAsync<ArgumentException>(() => client.GetAssetAsync(assetId, TestContext.CancellationToken));

        // Assert
        Assert.Contains("assetIdString", exception.Message);
    }

    [TestMethod]
    public async Task IronLedgerClient_GetAssetAsync_ReturnsSuccess()
    {
        // Arrange
        using var httpClient = new HttpClient(new MockHttpMessageHandler()
        {
            JsonResponse = @"{
  ""id"": {

  ""system_metadata"": {
    ""serial_number"": """",
    ""manufacturer"": ""Microsoft Corporation"",
    ""product"": ""Microsoft Surface Laptop, 7th Edition""
  },
  ""base_board_metadata"": {
    ""serial_number"": ""A000000000000000"",
    ""manufacturer"": ""Microsoft Corporation"",
    ""product"": ""Microsoft Surface Laptop, 7th Edition""
  },
  ""bios_metadata"": {
    ""serial_number"": ""00000000000000"",
    ""manufacturer"": ""Microsoft Corporation"",
    ""product"": ""175.138.235 QCOM   - 8380""
  },
  ""id"": ""7c8b87125ce37d3eaa02b4c5434ba68a5dca656783edca0266e9fcaac1baaf82""
},
""components"": {
    ""system"": {
      ""metadata"": {
        ""serial_number"": """",
        ""manufacturer"": """",
        ""product"": """"
      },
      ""caption"": """",
      ""properties"": {}
    },
    ""processors"": [],
    ""memory"": [],
    ""disks"": []
  }
}",
        })
        { BaseAddress = new Uri("http://www.example.com/") };

        var client = new IronLedgerClient(httpClient);
        var assetIdString = "7c8b87125ce37d3eaa02b4c5434ba68a5dca656783edca0266e9fcaac1baaf82";

        // Act
        var response = await client.GetAssetAsync(assetIdString, TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(response.IsSuccess);
        Assert.IsNotNull(response.Data);
        Assert.AreEqual(assetIdString, response.Data.Id.Id);
    }

    [TestMethod]
    public async Task IronLedgerClient_GetAssetAsyncByAssetId_ReturnsSuccess()
    {
        // Arrange
        using var httpClient = new HttpClient(new MockHttpMessageHandler()
        {
            JsonResponse = @"{
  ""id"": {

  ""system_metadata"": {
    ""serial_number"": """",
    ""manufacturer"": ""Microsoft Corporation"",
    ""product"": ""Microsoft Surface Laptop, 7th Edition""
  },
  ""base_board_metadata"": {
    ""serial_number"": ""A000000000000000"",
    ""manufacturer"": ""Microsoft Corporation"",
    ""product"": ""Microsoft Surface Laptop, 7th Edition""
  },
  ""bios_metadata"": {
    ""serial_number"": ""00000000000000"",
    ""manufacturer"": ""Microsoft Corporation"",
    ""product"": ""175.138.235 QCOM   - 8380""
  },
  ""id"": ""7c8b87125ce37d3eaa02b4c5434ba68a5dca656783edca0266e9fcaac1baaf82""
},
""components"": {
    ""system"": {
      ""metadata"": {
        ""serial_number"": """",
        ""manufacturer"": """",
        ""product"": """"
      },
      ""caption"": """",
      ""properties"": {}
    },
    ""processors"": [],
    ""memory"": [],
    ""disks"": []
  }
}",
        })
        { BaseAddress = new Uri("http://www.example.com/") };

        var client = IIronLedgerClient.Create(httpClient);
        var assetId = new AssetId()
        {
            BaseBoardMetadata = new AssetMetadata() { Manufacturer = "Microsoft Corporation", Product = "Microsoft Surface Laptop, 7th Edition", SerialNumber = "A000000000000000" },
            SystemMetadata = new AssetMetadata() { Manufacturer = "Microsoft Corporation", Product = "Microsoft Surface Laptop, 7th Edition", SerialNumber = "" },
            BiosMetadata = new AssetMetadata() { Manufacturer = "Microsoft Corporation", Product = "175.138.235 QCOM   - 8380", SerialNumber = "00000000000000" },
        };

        // Act
        var response = await client.GetAssetAsync(assetId, TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(response.IsSuccess);
        Assert.IsNotNull(response.Data);
        Assert.AreEqual(assetId.Id, response.Data.Id.Id);
    }

    [TestMethod]
    public async Task IronLedgerClient_GetAssetIdsAsync_ReturnsAssetIdList()
    {
        // Arrange
        var handler = new MockHttpMessageHandler()
        {
            JsonResponse = @"
[
  ""1111111111111111111111111111111111111111111111111111111111111111"",  
  ""0000000000000000000000000000000000000000000000000000000000000000""
]",
        };
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://www.example.com/") };
        var client = new IronLedgerClient(httpClient);

        // Act
        var response = await client.GetAssetIdsAsync(TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(response.IsSuccess);
        Assert.IsNotNull(response.Data);
        Assert.HasCount(2, response.Data);
        Assert.AreEqual("1111111111111111111111111111111111111111111111111111111111111111", response.Data[0]);
        Assert.AreEqual("0000000000000000000000000000000000000000000000000000000000000000", response.Data[1]);
        Assert.AreEqual((new Uri("http://www.example.com/api/v1/assets/")), handler.ProvidedRequestUri);
    }

    [TestMethod]
    public async Task IronLedgerClient_GetAssetIdsAsync_WhenCancelled_ReturnsFailure()
    {
        // Arrange
        using var httpClient = new HttpClient(new MockHttpMessageHandler()) { BaseAddress = new Uri("http://www.example.com/") };
        var client = new IronLedgerClient(httpClient);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var response = await client.GetAssetIdsAsync(cts.Token);

        // Assert
        Assert.IsFalse(response.IsSuccess);
        Assert.IsNotNull(response.ErrorMessage);
        Assert.Contains("cancelled", response.ErrorMessage);
    }


    // --- GetStatusAsync ---

    [TestMethod]
    public async Task GetStatusAsync_RequestsRelativeStatusEndpoint()
    {
        var handler = new FakeHandler("{}", HttpStatusCode.OK);
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://myserver:5037/") };
        var client = IIronLedgerClient.Create(httpClient);

        await client.GetStatusAsync(TestContext.CancellationToken);

        Assert.IsNotNull(handler.LastRequestUri);
        Assert.AreEqual("https://myserver:5037/api/v1/status", handler.LastRequestUri.ToString());
    }

    [TestMethod]
    public async Task GetStatusAsync_OnSuccess_IsSuccess()
    {
        var client = IIronLedgerClient.Create(MakeClient("{ \"version\": \"1.0\" }"));

        var result = await client.GetStatusAsync(TestContext.CancellationToken);

        Assert.IsTrue(result.IsSuccess);
    }

    [TestMethod]
    public async Task GetStatusAsync_OnSuccess_DataIsNotNull()
    {
        var client = IIronLedgerClient.Create(MakeClient("{ \"version\": \"1.0\" }"));

        var result = await client.GetStatusAsync(TestContext.CancellationToken);

        Assert.IsNotNull(result.Data);
    }

    [TestMethod]
    public async Task GetStatusAsync_OnHttpException_ReturnsFailure()
    {
        var handler = new ThrowingHandler();
        var client = IIronLedgerClient.Create(new HttpClient(handler) { BaseAddress = new Uri("https://myserver:5037/") });

        var result = await client.GetStatusAsync(TestContext.CancellationToken);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.ErrorMessage);
    }

    [TestMethod]
    public async Task GetStatusAsync_OnGenericException_ReturnsFailure()
    {
        var handler = new MockHttpMessageHandler() { AlwaysThrows = new Exception("test") };
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://myserver:5037/") };
        var client = IIronLedgerClient.Create(httpClient);

        var result = await client.GetStatusAsync(TestContext.CancellationToken);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.ErrorMessage);
        Assert.AreEqual("Unexpected error occurred.", result.ErrorMessage);
    }

    [TestMethod]
    public async Task GetAssetIdsAsync_NullSerializer_ReturnsFailure()
    {
        var handler = new MockHttpMessageHandler() { JsonResponse = "{}" };
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://myserver:5037/") };
        var client = IIronLedgerClient.Create(httpClient, serializer: new NullSerializer());

        var result = await client.GetAssetIdsAsync(TestContext.CancellationToken);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.ErrorMessage);
        Assert.AreEqual("Response contained no data.", result.ErrorMessage);
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

    [ExcludeFromCodeCoverage]
    private sealed class ThrowingHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => throw new HttpRequestException("Network failure");
    }

    [ExcludeFromCodeCoverage]
    private sealed class NullSerializer : IIronLedgerSerializer
    {
        public string ContentType => "application/json";

        public T? Deserialize<T>(string data) => default;

        public string Serialize<T>(T value) => null!;
    }

}
