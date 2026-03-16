namespace Tudormobile.IronLedgerLib.Services;

/// <summary>
/// Configuration options for the IronLedger HTTP client.
/// </summary>
public class IronLedgerClientOptions
{
    /// <summary>
    /// Gets or sets the base URL of the IronLedger server.
    /// </summary>
    /// <remarks>
    /// A trailing slash is recommended when the URL includes a path segment
    /// (e.g., <c>https://myserver/ironledger/</c>) to ensure relative endpoint
    /// paths resolve correctly.
    /// </remarks>
    public Uri? ServerUrl { get; set; }
}
