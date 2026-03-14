using System.Collections.ObjectModel;

namespace Tudormobile.IronLedgerLib.UI;

/// <summary>
/// Represents a dynamic collection of disk component data that provides notifications when items are added, removed, or
/// the entire list is refreshed.
/// </summary>
public class ObservableDiskData : ObservableCollection<ComponentData>
{
    /// <summary>
    /// Initializes a new instance of the ObservableDiskData class with the specified collection of component data.
    /// </summary>
    /// <param name="collection">(Optional) The collection of ComponentData objects to initialize the data with. Can be null.</param>
    public ObservableDiskData(IEnumerable<ComponentData>? collection = null) : base(collection ?? []) { }

}