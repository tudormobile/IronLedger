using System.Text.Json;
using System.Text.Json.Serialization;
using Tudormobile.IronLedgerLib;

namespace AssetId;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Retrieving Asset Identification ...\n");

        var factory = new AssetIdFactory();

        try
        {
            var assetId = factory.Create();
            Console.WriteLine($"AssetId: {assetId}");
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            };
            options.Converters.Add(new JsonStringEnumConverter());
            var json = JsonSerializer.Serialize(assetId, options);
            Console.WriteLine(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving serial number: {ex.Message}");
        }
    }
}