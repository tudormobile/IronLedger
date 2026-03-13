
namespace Tudormobile.IronLedgerLib;

/// <summary>
/// Base exception for all IronLedger library errors.
/// </summary>
public class IronLedgerException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IronLedgerException"/> class.
    /// </summary>
    public IronLedgerException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="IronLedgerException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public IronLedgerException(string message)
        : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="IronLedgerException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public IronLedgerException(string message, Exception innerException)
        : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when component data operations fail.
/// </summary>
public class ComponentDataException : IronLedgerException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentDataException"/> class.
    /// </summary>
    public ComponentDataException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentDataException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ComponentDataException(string message)
        : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentDataException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public ComponentDataException(string message, Exception innerException)
        : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when a component data provider fails to retrieve data.
/// </summary>
public class ComponentDataProviderException : ComponentDataException
{
    /// <summary>
    /// Gets or initializes the name of the provider that failed.
    /// </summary>
    public string? ProviderName { get; init; }

    /// <summary>
    /// Gets or initializes the WMI class name that was being queried when the provider failed.
    /// </summary>
    public string? WmiClassName { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentDataProviderException"/> class.
    /// </summary>
    public ComponentDataProviderException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentDataProviderException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ComponentDataProviderException(string message)
        : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentDataProviderException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public ComponentDataProviderException(string message, Exception innerException)
        : base(message, innerException) { }
}