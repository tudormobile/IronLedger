namespace Tudormobile.IronLedgerLib.Services;

/// <summary>
/// Represents the result of an IronLedger API call. Exactly one of <see cref="Data"/> or
/// <see cref="ErrorMessage"/> is non-null; use <see cref="IsSuccess"/> to distinguish them.
/// </summary>
/// <typeparam name="T">The type of data returned on success.</typeparam>
public sealed class IronLedgerResponse<T>
{
    /// <summary>
    /// Gets the data returned by the API call, or <see langword="null"/> if the request failed.
    /// </summary>
    public T? Data { get; }

    /// <summary>
    /// Gets the error message associated with a failed API call, or <see langword="null"/> on success.
    /// </summary>
    public string? ErrorMessage { get; }

    /// <summary>
    /// Gets a value indicating whether the API call was successful.
    /// </summary>
    public bool IsSuccess { get; }

    private IronLedgerResponse(T? data, string? errorMessage, bool isSuccess)
    {
        Data = data;
        ErrorMessage = errorMessage;
        IsSuccess = isSuccess;
    }

    /// <summary>
    /// Creates a successful response containing the given data.
    /// </summary>
    /// <param name="data">The data returned by the API call.</param>
    public static IronLedgerResponse<T> Success(T data) => new(data, null, true);

    /// <summary>
    /// Creates a failed response containing the given error message.
    /// </summary>
    /// <param name="errorMessage">The error message describing the failure.</param>
    public static IronLedgerResponse<T> Failure(string errorMessage) => new(default, errorMessage, false);
}
