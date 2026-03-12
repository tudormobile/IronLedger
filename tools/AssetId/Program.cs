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
            var json = assetId.Serialize();
            Console.WriteLine(assetId);
            Console.WriteLine(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving asset information: {ex.Message}");
        }
    }
}