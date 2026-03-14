using CommunityToolkit.Mvvm.ComponentModel;

namespace Tudormobile.IronLedgerLib.UI;

/// <summary>
/// Represents a data container that supports observation of changes to its system-related properties.
/// </summary>
public partial class ObservableSystemData : ObservableObject
{
    /// <summary>
    /// Gets or sets the data associated with the component.
    /// </summary>
    [ObservableProperty]
    public partial ComponentData Data { get; set; } = ComponentData.Empty;

    /// <summary>
    /// Gets or sets the display name associated with the object.
    /// </summary>
    [ObservableProperty]
    public partial string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description associated with this instance.
    /// </summary>
    [ObservableProperty]
    public partial string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the notes associated with the current object.
    /// </summary>
    [ObservableProperty]
    public partial string Notes { get; set; } = string.Empty;

    /// <summary>
    /// Initializes a new instance of the ObservableSystemData class with the specified system data.
    /// </summary>
    /// <param name="systemData">(Optional) The initial system data to associate with this instance. Can be null.</param>
    public ObservableSystemData(ComponentData? systemData = null)
    {
        Data = systemData ?? ComponentData.Empty;
    }
}