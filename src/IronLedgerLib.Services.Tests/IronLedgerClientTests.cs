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
    public void IronLedgerClient_Constructor_EnsuresTrailingSlashOnBaseAddress()
    {
        using var httpClient = new HttpClient() { BaseAddress = new Uri("https://myserver:5037/prefix") };
        _ = new IronLedgerClient(httpClient);

        Assert.IsTrue(httpClient.BaseAddress!.ToString().EndsWith('/'));
    }

    [TestMethod]
    public void IronLedgerClient_Constructor_PreservesTrailingSlashWhenAlreadyPresent()
    {
        var original = new Uri("https://myserver:5037/prefix/");
        using var httpClient = new HttpClient() { BaseAddress = original };
        _ = new IronLedgerClient(httpClient);

        Assert.AreEqual(original, httpClient.BaseAddress);
    }

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

    // --- GetNotesAsync ---

    [TestMethod]
    public async Task GetNotesAsync_WithNullAssetId_Throws()
    {
        using var httpClient = new HttpClient(new MockHttpMessageHandler()) { BaseAddress = new Uri("http://www.example.com/") };
        var client = new IronLedgerClient(httpClient);

        var exception = await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => client.GetNotesAsync(null!, TestContext.CancellationToken));

        Assert.Contains("assetIdString", exception.Message);
    }

    [TestMethod]
    public async Task GetNotesAsync_WithEmptyAssetId_Throws()
    {
        using var httpClient = new HttpClient(new MockHttpMessageHandler()) { BaseAddress = new Uri("http://www.example.com/") };
        var client = new IronLedgerClient(httpClient);

        var exception = await Assert.ThrowsExactlyAsync<ArgumentException>(() => client.GetNotesAsync(string.Empty, TestContext.CancellationToken));

        Assert.Contains("assetIdString", exception.Message);
    }

    [TestMethod]
    public async Task GetNotesAsync_RequestsCorrectEndpoint()
    {
        var handler = new MockHttpMessageHandler() { JsonResponse = "some notes" };
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://myserver:5037/") };
        var client = new IronLedgerClient(httpClient);

        await client.GetNotesAsync("my-asset-id", TestContext.CancellationToken);

        Assert.AreEqual("https://myserver:5037/api/v1/assets/my-asset-id/notes", handler.ProvidedRequestUri!.ToString());
    }

    [TestMethod]
    public async Task GetNotesAsync_OnSuccess_ReturnsNotes()
    {
        const string notes = "these are the asset notes";
        var handler = new MockHttpMessageHandler() { JsonResponse = notes };
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://myserver:5037/") };
        var client = new IronLedgerClient(httpClient);

        var result = await client.GetNotesAsync("my-asset-id", TestContext.CancellationToken);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(notes, result.Data);
    }

    [TestMethod]
    public async Task GetNotesAsync_OnHttpError_ReturnsFailure()
    {
        var handler = new MockHttpMessageHandler() { AlwaysResponds = new HttpResponseMessage(HttpStatusCode.InternalServerError) };
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://myserver:5037/") };
        var client = new IronLedgerClient(httpClient);

        var result = await client.GetNotesAsync("my-asset-id", TestContext.CancellationToken);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.ErrorMessage);
    }

    [TestMethod]
    public async Task GetNotesAsync_ByAssetId_RequestsCorrectEndpoint()
    {
        var assetId = new AssetId() { BaseBoardMetadata = AssetMetadata.Empty, BiosMetadata = AssetMetadata.Empty, SystemMetadata = AssetMetadata.Empty };
        var handler = new MockHttpMessageHandler() { JsonResponse = "some notes" };
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://myserver:5037/") };
        var client = IIronLedgerClient.Create(httpClient);

        var result = await client.GetNotesAsync(assetId, TestContext.CancellationToken);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual($"https://myserver:5037/api/v1/assets/{assetId.Id}/notes", handler.ProvidedRequestUri!.ToString());
    }

    // --- SetNotesAsync ---

    [TestMethod]
    public async Task SetNotesAsync_WithNullAssetId_Throws()
    {
        using var httpClient = new HttpClient(new MockHttpMessageHandler()) { BaseAddress = new Uri("http://www.example.com/") };
        var client = new IronLedgerClient(httpClient);

        await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => client.SetNotesAsync(null!, "notes", TestContext.CancellationToken));
    }

    [TestMethod]
    public async Task SetNotesAsync_RequestsCorrectEndpoint()
    {
        const string assetIdString = "my-asset-id";
        var handler = new MockHttpMessageHandler() { JsonResponse = $"\"{assetIdString}\"" };
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://myserver:5037/") };
        var client = new IronLedgerClient(httpClient);

        await client.SetNotesAsync(assetIdString, "notes", TestContext.CancellationToken);

        Assert.AreEqual($"https://myserver:5037/api/v1/assets/{assetIdString}/notes", handler.ProvidedRequestUri!.ToString());
    }

    [TestMethod]
    public async Task SetNotesAsync_OnMatchingResponse_ReturnsSuccess()
    {
        const string assetIdString = "my-asset-id";
        var handler = new MockHttpMessageHandler() { JsonResponse = $"\"{assetIdString}\"" };
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://myserver:5037/") };
        var client = new IronLedgerClient(httpClient);

        var result = await client.SetNotesAsync(assetIdString, "these are notes", TestContext.CancellationToken);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(assetIdString, result.Data);
    }

    [TestMethod]
    public async Task SetNotesAsync_WithFullAssetId_OnMatchingResponse_ReturnsSuccess()
    {
        var assetId = new AssetId()
        {
            BaseBoardMetadata = AssetMetadata.Empty,
            BiosMetadata = AssetMetadata.Empty,
            SystemMetadata = AssetMetadata.Empty,
        };
        var assetIdString = assetId.Id;

        var handler = new MockHttpMessageHandler() { JsonResponse = $"\"{assetIdString}\"" };
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://myserver:5037/") };
        var client = IIronLedgerClient.Create(httpClient);

        var result = await client.SetNotesAsync(assetId, "these are notes", TestContext.CancellationToken);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(assetIdString, result.Data);

    }

    [TestMethod]
    public async Task SetNotesAsync_OnMismatchedResponse_ReturnsFailure()
    {
        var handler = new MockHttpMessageHandler() { JsonResponse = "\"different-id\"" };
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://myserver:5037/") };
        var client = new IronLedgerClient(httpClient);

        var result = await client.SetNotesAsync("my-asset-id", "notes", TestContext.CancellationToken);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.ErrorMessage);
    }

    [TestMethod]
    public async Task SetNotesAsync_OnHttpError_ReturnsFailure()
    {
        var handler = new MockHttpMessageHandler() { AlwaysResponds = new HttpResponseMessage(HttpStatusCode.InternalServerError) };
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://myserver:5037/") };
        var client = new IronLedgerClient(httpClient);

        var result = await client.SetNotesAsync("my-asset-id", "notes", TestContext.CancellationToken);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.ErrorMessage);
    }

    // --- GetComponentsAsync ---

    [TestMethod]
    public async Task GetComponentsAsync_WithNullAssetId_Throws()
    {
        using var httpClient = new HttpClient(new MockHttpMessageHandler()) { BaseAddress = new Uri("http://www.example.com/") };
        var client = new IronLedgerClient(httpClient);

        var exception = await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => client.GetComponentsAsync(null!, TestContext.CancellationToken));

        Assert.Contains("assetIdString", exception.Message);
    }

    [TestMethod]
    public async Task GetComponentsAsync_WithEmptyAssetId_Throws()
    {
        using var httpClient = new HttpClient(new MockHttpMessageHandler()) { BaseAddress = new Uri("http://www.example.com/") };
        var client = new IronLedgerClient(httpClient);

        var exception = await Assert.ThrowsExactlyAsync<ArgumentException>(() => client.GetComponentsAsync(string.Empty, TestContext.CancellationToken));

        Assert.Contains("assetIdString", exception.Message);
    }

    [TestMethod]
    public async Task GetComponentsAsync_RequestsCorrectEndpoint()
    {
        var handler = new MockHttpMessageHandler() { JsonResponse = @"{""system"":{""metadata"":{""serial_number"":"""",""manufacturer"":"""",""product"":""""},""caption"":"""",""properties"":{}},""processors"":[],""memory"":[],""disks"":[]}" };
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://myserver:5037/") };
        var client = new IronLedgerClient(httpClient);

        await client.GetComponentsAsync("my-asset-id", TestContext.CancellationToken);

        Assert.AreEqual("https://myserver:5037/api/v1/assets/my-asset-id/components", handler.ProvidedRequestUri!.ToString());
    }

    [TestMethod]
    public async Task GetComponentsAsync_OnSuccess_ReturnsComponentData()
    {
        var handler = new MockHttpMessageHandler() { JsonResponse = @"{""system"":{""metadata"":{""serial_number"":"""",""manufacturer"":"""",""product"":""""},""caption"":"""",""properties"":{}},""processors"":[],""memory"":[],""disks"":[]}" };
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://myserver:5037/") };
        var client = new IronLedgerClient(httpClient);

        var result = await client.GetComponentsAsync("my-asset-id", TestContext.CancellationToken);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Data);
        Assert.IsNotNull(result.Data.System);
        Assert.HasCount(0, result.Data.Processors);
        Assert.HasCount(0, result.Data.Memory);
        Assert.HasCount(0, result.Data.Disks);
    }

    [TestMethod]
    public async Task GetComponentsAsync_WithFullAssetId_PassesId()
    {
        var handler = new MockHttpMessageHandler() { JsonResponse = @"{""system"":{""metadata"":{""serial_number"":"""",""manufacturer"":"""",""product"":""""},""caption"":"""",""properties"":{}},""processors"":[],""memory"":[],""disks"":[]}" };
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://myserver:5037/") };
        var assetId = new AssetId()
        {
            BaseBoardMetadata = AssetMetadata.Empty,
            BiosMetadata = AssetMetadata.Empty,
            SystemMetadata = AssetMetadata.Empty,
        };
        var id = assetId.Id;
        var client = IIronLedgerClient.Create(httpClient);

        var result = await client.GetComponentsAsync(assetId, TestContext.CancellationToken);

        Assert.Contains(id, handler.ProvidedRequestUri!.ToString());
    }



    [TestMethod]
    public async Task GetComponentsAsync_OnHttpError_ReturnsFailure()
    {
        var handler = new MockHttpMessageHandler() { AlwaysResponds = new HttpResponseMessage(HttpStatusCode.InternalServerError) };
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://myserver:5037/") };
        var client = new IronLedgerClient(httpClient);

        var result = await client.GetComponentsAsync("my-asset-id", TestContext.CancellationToken);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.ErrorMessage);
    }

    // --- SetComponentsAsync ---

    [TestMethod]
    public async Task SetComponentsAsync_WithNullAssetId_Throws()
    {
        using var httpClient = new HttpClient(new MockHttpMessageHandler()) { BaseAddress = new Uri("http://www.example.com/") };
        var client = new IronLedgerClient(httpClient);
        var components = new SystemComponentData { System = ComponentData.Empty, Processors = [], Memory = [], Disks = [] };

        await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => client.SetComponentsAsync(null!, components, TestContext.CancellationToken));
    }

    [TestMethod]
    public async Task SetComponentsAsync_RequestsCorrectEndpoint()
    {
        const string assetIdString = "my-asset-id";
        var handler = new MockHttpMessageHandler() { JsonResponse = $"\"{assetIdString}\"" };
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://myserver:5037/") };
        var client = new IronLedgerClient(httpClient);
        var components = new SystemComponentData { System = ComponentData.Empty, Processors = [], Memory = [], Disks = [] };

        await client.SetComponentsAsync(assetIdString, components, TestContext.CancellationToken);

        Assert.AreEqual($"https://myserver:5037/api/v1/assets/{assetIdString}/components", handler.ProvidedRequestUri!.ToString());
    }

    [TestMethod]
    public async Task SetComponentsAsync_WithFullAssetId_RequestsCorrectEndpoint()
    {
        var assetId = new AssetId()
        {
            BaseBoardMetadata = AssetMetadata.Empty,
            BiosMetadata = AssetMetadata.Empty,
            SystemMetadata = AssetMetadata.Empty,
        };
        var id = assetId.Id;

        var handler = new MockHttpMessageHandler() { JsonResponse = $"\"{id}\"" };
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://myserver:5037/") };
        var client = IIronLedgerClient.Create(httpClient);
        var components = new SystemComponentData { System = ComponentData.Empty, Processors = [], Memory = [], Disks = [] };

        await client.SetComponentsAsync(assetId, components, TestContext.CancellationToken);

        Assert.AreEqual($"https://myserver:5037/api/v1/assets/{id}/components", handler.ProvidedRequestUri!.ToString());

    }

    [TestMethod]
    public async Task SetComponentsAsync_OnMatchingResponse_ReturnsSuccess()
    {
        const string assetIdString = "my-asset-id";
        var handler = new MockHttpMessageHandler() { JsonResponse = $"\"{assetIdString}\"" };
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://myserver:5037/") };
        var client = new IronLedgerClient(httpClient);
        var components = new SystemComponentData { System = ComponentData.Empty, Processors = [], Memory = [], Disks = [] };

        var result = await client.SetComponentsAsync(assetIdString, components, TestContext.CancellationToken);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(assetIdString, result.Data);
    }

    [TestMethod]
    public async Task SetComponentsAsync_OnMismatchedResponse_ReturnsFailure()
    {
        var handler = new MockHttpMessageHandler() { JsonResponse = "\"different-id\"" };
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://myserver:5037/") };
        var client = new IronLedgerClient(httpClient);
        var components = new SystemComponentData { System = ComponentData.Empty, Processors = [], Memory = [], Disks = [] };

        var result = await client.SetComponentsAsync("my-asset-id", components, TestContext.CancellationToken);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.ErrorMessage);
    }

    [TestMethod]
    public async Task SetComponentsAsync_OnHttpError_ReturnsFailure()
    {
        var handler = new MockHttpMessageHandler() { AlwaysResponds = new HttpResponseMessage(HttpStatusCode.InternalServerError) };
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://myserver:5037/") };
        var client = new IronLedgerClient(httpClient);
        var components = new SystemComponentData { System = ComponentData.Empty, Processors = [], Memory = [], Disks = [] };

        var result = await client.SetComponentsAsync("my-asset-id", components, TestContext.CancellationToken);

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
