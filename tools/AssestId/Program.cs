using Microsoft.Management.Infrastructure;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AssestId;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Retrieving Asset Identification ...\n");

        // root\cimv2 namespace is used to access system information, including BIOS details
        // CIM (Common Information Model) is a standard for representing system information, and WQL (WMI Query Language) is used to query this information
        // CIM_Processor class contains properties related to the system's processor, but for serial number, we typically query Win32_BIOS class
        // Win32_Processor class does not contain a SerialNumber property, so we will query Win32_BIOS instead to get the serial number of the system
        // CIM_Memory class contains properties related to the system's memory, but it does not contain a SerialNumber property either
        // CIM_System class contains properties related to the system as a whole, but it does not contain a SerialNumber property either
        // CIM_ComputerSystem class contains properties related to the computer system, but it does not contain a SerialNumber property either
        // Win32_ComputerSystem class contains properties related to the computer system, but it does not contain a SerialNumber property either
        // Win32_Bios class contains properties related to the BIOS, including the SerialNumber property, which is why we will query this class to get the serial number of the system
        // 

        /*
         * CIM_ComputerSystem - Name, Manufacturer, Model, TotalPhysicalMemory
         * Win32_Bios - SerialNumber, Manufacturer, Version, ReleaseDate (BIOS Serial Number - generic in my case)
         * Win32_BaseBoard - SerialNumber, Manufacturer, Product (Motherboard Serial Number - unique in my case)
         * CIM_Processor - Name, DeviceID, Caption, MaxClockSpeed, SocketDesignation
        */


        try
        {
            var assetId = GetSystemAssetId();
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

    private static AssetId GetSystemAssetId()
    {
        (var serialNumber, var manufacturer, var product) = GetBaseBoardInfo();
        (var biosManufacturer, var biosSerialNumber, var biosVersion, var biosName) = GetBiosInfo();

        var deviceId = $"BIOS|{biosManufacturer}|{biosName}|{biosSerialNumber}|{biosVersion}";

        (var systemName, var systemManufacturer, var systemModel, var systemMemory) = GetComputerSystemInfo();

        //var serialNumber = GetSystemSerialNumber();
        //var deviceId = GetDeviceId();
        //var name = GetName();
        //var manufacturer = GetManufacturer();
        //var model = GetModel();

        return new AssetId
        {
            IdType = AssetIdType.sn,
            Metadata = new AssetMetadata()
            {
                SerialNumber = serialNumber,
                DeviceId = deviceId,
                Name = $"{systemName}",
                Manufacturer = $"{systemManufacturer}|{manufacturer}",
                Model = $"{systemModel}|{product}"
            }
        };
    }

    private static (string, string, string, string) GetComputerSystemInfo()
    {
        using var session = CimSession.Create(null);
        var instances = session.QueryInstances(@"root\cimv2", "WQL", "SELECT Name, Manufacturer, Model, TotalPhysicalMemory FROM Win32_ComputerSystem");
        foreach (var instance in instances)
        {
            var name = instance.CimInstanceProperties["Name"]?.Value?.ToString();
            var manufacturer = instance.CimInstanceProperties["Manufacturer"]?.Value?.ToString();
            var model = instance.CimInstanceProperties["Model"]?.Value?.ToString();
            var memory = instance.CimInstanceProperties["TotalPhysicalMemory"]?.Value?.ToString();
            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(manufacturer) && !string.IsNullOrWhiteSpace(model) && !string.IsNullOrWhiteSpace(memory))
            {
                return (name, manufacturer, model, memory);
            }
        }
        return ("Name not found", "Manufacturer not found", "Model not found", "Memory not found");
    }

    private static (string, string, string, string) GetBiosInfo()
    {
        using var session = CimSession.Create(null);
        var instances = session.QueryInstances(@"root\cimv2", "WQL", "SELECT SerialNumber, Manufacturer, Version, Name FROM Win32_BIOS");
        foreach (var instance in instances)
        {
            var serialNumber = instance.CimInstanceProperties["SerialNumber"]?.Value?.ToString();
            var manufacturer = instance.CimInstanceProperties["Manufacturer"]?.Value?.ToString();
            var version = instance.CimInstanceProperties["Version"]?.Value?.ToString();
            var name = instance.CimInstanceProperties["Name"]?.Value?.ToString();
            if (!string.IsNullOrWhiteSpace(serialNumber) && !string.IsNullOrWhiteSpace(manufacturer) && !string.IsNullOrWhiteSpace(version) && !string.IsNullOrWhiteSpace(name))
            {
                return (manufacturer, serialNumber, version, name);
            }
        }
        return ("Manufacturer not found", "Serial number not found", "Version not found", "Name not found");
    }
    private static (string, string, string) GetBaseBoardInfo()
    {
        using var session = CimSession.Create(null);
        var instances = session.QueryInstances(@"root\cimv2", "WQL", "SELECT SerialNumber, Manufacturer, Product FROM Win32_BaseBoard");
        foreach (var instance in instances)
        {
            var serialNumber = instance.CimInstanceProperties["SerialNumber"]?.Value?.ToString();
            var manufacturer = instance.CimInstanceProperties["Manufacturer"]?.Value?.ToString();
            var product = instance.CimInstanceProperties["Product"]?.Value?.ToString();
            if (!string.IsNullOrWhiteSpace(serialNumber) && !string.IsNullOrWhiteSpace(manufacturer) && !string.IsNullOrWhiteSpace(product))
            {
                return (serialNumber, manufacturer, product);
            }
        }
        return ("Serial number not found", "Manufacturer not found", "Product not found");
    }
}

public record AssetId
{
    public AssetIdType IdType { get; init; }
    public required AssetMetadata Metadata { get; init; }
    public override string ToString() => IdType switch
    {
        AssetIdType.sn => Metadata.SerialNumber,
        AssetIdType.did => Metadata.DeviceId,
        _ => Metadata.GeneratedId
    };
}

public record AssetMetadata
{
    // GeneratedId is a unique identifier that can be used when SerialNumber and DeviceId are not unqiue.
    // It is generated using the SHA256 hash of the SerialNumber, DeviceId, Name, Manufacturer, and Model,
    // which ensures that it is unique for each combination of AssetMetadata.
    private readonly Lazy<string> _generatedId;
    public required string SerialNumber { get; init; }
    public required string DeviceId { get; init; }
    public required string Name { get; init; }
    public required string Manufacturer { get; init; }
    public required string Model { get; init; }
    public string GeneratedId => _generatedId.Value;

    public AssetMetadata()
    {
        _generatedId = new Lazy<string>(() =>
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var input = $"{SerialNumber}|{DeviceId}|{Name}|{Manufacturer}|{Model}";
            var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
            return Convert.ToHexStringLower(hashBytes);
        });
    }
}

public enum AssetIdType
{
    id,
    sn,
    did
}
