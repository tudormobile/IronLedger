using CommunityToolkit.Mvvm.ComponentModel;

namespace Tudormobile.IronLedgerLib.UI;

/// <summary>
/// Represents an observable asset that exposes system, memory, disk, and processor data for monitoring 
/// or data binding scenarios.
/// </summary>
public partial class ObservableAsset : ObservableObject
{
    /// <summary>
    /// Gets the unique identifier for the associated asset.
    /// </summary>
    public AssetId AssetId { get; init; }

    /// <summary>
    /// Gets or sets the system data associated with the observable entity.
    /// </summary>
    [ObservableProperty]
    public partial ObservableSystemData System { get; set; }

    /// <summary>
    /// Gets or sets the current memory data for observation and binding.
    /// </summary>
    [ObservableProperty]
    public partial ObservableMemoryData Memory { get; set; }

    /// <summary>
    /// Gets or sets the collection of disk data being observed.
    /// </summary>
    [ObservableProperty]
    public partial ObservableDiskData Disks { get; set; }

    /// <summary>
    /// Gets or sets the collection of processor data used for observation.
    /// </summary>
    [ObservableProperty]
    public partial ObservableProcessorData Processors { get; set; }

    /// <summary>
    /// Initializes a new instance of the ObservableAsset class with the specified asset identifier 
    /// and component data collections.
    /// </summary>
    /// <param name="assetId">The unique identifier for the asset to be observed.</param>
    /// <param name="systemData">(Optional) The component data representing system-level information for the asset.</param>
    /// <param name="diskData">(Optional) A collection of component data representing disk-related information for the asset.</param>
    /// <param name="memoryData">Optional) A collection of component data representing memory-related information for the asset.</param>
    /// <param name="processorData">(Optional) A collection of component data representing processor-related information for the asset.</param>
    public ObservableAsset(AssetId assetId,
        ComponentData? systemData = null,
        IEnumerable<ComponentData>? diskData = null,
        IEnumerable<ComponentData>? memoryData = null,
        IEnumerable<ComponentData>? processorData = null)
    {
        AssetId = assetId;
        System = new ObservableSystemData(systemData);
        Disks = new ObservableDiskData(diskData);
        Memory = new ObservableMemoryData(memoryData);
        Processors = new ObservableProcessorData(processorData);
    }
}
