using System.Text.Json;
using System.Text.Json.Serialization;
using Tudormobile.IronLedgerLib;

namespace ComponentList;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Retrieving Compnents ...\n");
        try
        {
            // Get everything at once
            var factory = new ComponentDataFactory();
            var allComponents = factory.Create();
            Console.WriteLine($"System: {allComponents.System.Caption}");
            Console.WriteLine($"Processors: {allComponents.Processors.Count}");
            Console.WriteLine($"Memory: {allComponents.Memory.Count}");
            Console.WriteLine($"Disks: {allComponents.Disks.Count}");

            // Or get specific components only (more efficient if you don't need everything)
            var processors = factory.GetProcessors();
            var disks = factory.GetDisks();
            var system = factory.GetSystem();

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            };
            options.Converters.Add(new JsonStringEnumConverter());
            options.Converters.Add(new ComponentPropertyConverter());
            var json = JsonSerializer.Serialize(allComponents, options);
            Console.WriteLine(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving component information: {ex.Message}");
        }
    }
}