# Exception Handling in IronLedger

This document describes the exception handling strategy used in the IronLedger library, including when to use custom exceptions vs. standard .NET exceptions.

## Exception Hierarchy

IronLedger defines a custom exception hierarchy to provide clear, actionable error information:

```
System.Exception
└── IronLedgerException (base for all IronLedger errors)
    └── ComponentDataException (component data operations)
        └── ComponentDataProviderException (provider-specific failures)
```

## Custom Exception Classes

### IronLedgerException

**Purpose:** Base exception for all IronLedger library errors.

**When to use:** This is the root of the IronLedger exception hierarchy. Generally, you should throw more specific derived exceptions rather than this base class directly.

**Example:**
```csharp
// Rarely used directly - prefer more specific exceptions
throw new IronLedgerException("An unexpected error occurred in IronLedger");
```

### ComponentDataException

**Purpose:** Thrown when component data operations fail.

**When to use:** 
- General component data manipulation errors
- Data validation failures specific to component data
- Operations on `ComponentData`, `AssetMetadata`, or related types

**Example:**
```csharp
throw new ComponentDataException("Failed to process component data");
```

### ComponentDataProviderException

**Purpose:** Thrown when a component data provider fails to retrieve data from the underlying data source (WMI/CIM, etc.).

**When to use:**
- WMI/CIM query failures
- Hardware data retrieval errors
- Provider initialization failures

**Properties:**
- `ProviderName`: The name of the provider that failed (e.g., "ProcessorDataProvider")
- `WmiClassName`: The WMI class being queried when the failure occurred (e.g., "Win32_Processor")

**Example:**
```csharp
try
{
    // Query WMI
}
catch (Exception ex)
{
    throw new ComponentDataProviderException(
        $"Failed to retrieve data from WMI class '{WmiClassName}'.",
        ex)
    {
        ProviderName = GetType().Name,
        WmiClassName = WmiClassName
    };
}
```

## Standard .NET Exceptions

IronLedger also uses standard .NET exceptions where appropriate. Use these instead of custom exceptions for common programming errors:

### ArgumentNullException

**When to use:** A required parameter or argument is null.

**Example:**
```csharp
public IronLedgerJsonSerializer(JsonSerializerOptions options)
{
    _options = options ?? throw new ArgumentNullException(nameof(options));
}
```

### ArgumentException

**When to use:** 
- An argument value is invalid (but not null)
- String arguments that are empty or whitespace
- Arguments outside valid ranges

**Example:**
```csharp
public T? Deserialize<T>(string data)
{
    if (string.IsNullOrWhiteSpace(data))
        throw new ArgumentException("Data cannot be null or whitespace.", nameof(data));
    
    // ...
}
```

### InvalidOperationException

**When to use:** 
- An operation is invalid for the object's current state
- Method calls in the wrong sequence
- Operations attempted when the object is not properly initialized

**Example:**
```csharp
if (!_isInitialized)
    throw new InvalidOperationException("Provider must be initialized before use");
```

## Exception Handling Guidelines

### 1. When to Catch Exceptions

**DO catch exceptions when:**
- You can handle the error meaningfully
- You need to wrap lower-level exceptions with domain-specific exceptions
- You need to add context before re-throwing
- You need to log and recover gracefully

**DON'T catch exceptions when:**
- You can't do anything meaningful with them
- You're just going to re-throw without adding value
- The calling code needs to handle the error

### 2. Exception Wrapping in Providers

All provider base classes (`CimDataProviderBase`, `CimMetadataProviderBase`) wrap low-level CIM/WMI exceptions:

```csharp
public IReadOnlyList<ComponentData> GetData()
{
    try
    {
        // WMI query logic
    }
    catch (Exception ex) when (ex is not ComponentDataProviderException)
    {
        throw new ComponentDataProviderException(
            $"Failed to retrieve data from WMI class '{WmiClassName}'.",
            ex)
        {
            ProviderName = GetType().Name,
            WmiClassName = WmiClassName
        };
    }
}
```

**Key points:**
- Only catch exceptions that are not already `ComponentDataProviderException` (using exception filter)
- Always include the original exception as `InnerException`
- Populate `ProviderName` and `WmiClassName` properties for diagnostics

### 3. Factory Methods and Exception Propagation

Factory classes (`ComponentDataFactory`, `AssetIdFactory`) do NOT catch provider exceptions - they let them propagate:

```csharp
public SystemComponentData Create()
{
    // These calls may throw ComponentDataProviderException
    var systems = _systemProvider.GetData();
    return new SystemComponentData
    {
        System = systems.Count > 0 ? systems[0] : ComponentData.Empty,
        Processors = _processorProvider.GetData(),
        Memory = _memoryProvider.GetData(),
        Disks = _diskProvider.GetData()
    };
}
```

**Rationale:** 
- Factories are orchestration layers - they shouldn't hide provider failures
- Calling code should decide how to handle provider exceptions
- All factory methods document that they may throw `ComponentDataProviderException`

### 4. Client Code Exception Handling

Client code should handle exceptions at appropriate boundaries:

```csharp
try
{
    var factory = new ComponentDataFactory();
    var data = factory.Create();
    // Use data...
}
catch (ComponentDataProviderException ex)
{
    // Log provider details
    _logger.LogError(ex, 
        "Provider {ProviderName} failed querying {WmiClassName}", 
        ex.ProviderName, 
        ex.WmiClassName);
    
    // Handle gracefully - maybe use cached data or retry
}
```

### 5. Serialization Exceptions

Serialization classes use standard .NET exceptions since they deal with input validation:

```csharp
public string Serialize<T>(T value)
{
    if (value == null)
        throw new ArgumentNullException(nameof(value));
    
    return JsonSerializer.Serialize(value, _options);
}

public T? Deserialize<T>(string data)
{
    if (string.IsNullOrWhiteSpace(data))
        throw new ArgumentException("Data cannot be null or whitespace.", nameof(data));
    
    return JsonSerializer.Deserialize<T>(data, _options);
}
```

## Exception Documentation

All public methods that can throw exceptions should document them in XML comments:

```csharp
/// <summary>
/// Retrieves the component data from hardware components.
/// </summary>
/// <returns>A collection of component data instances.</returns>
/// <exception cref="ComponentDataProviderException">
/// Thrown when the provider fails to retrieve data from the underlying data source.
/// </exception>
IReadOnlyList<ComponentData> GetData();
```

## Testing Exception Scenarios

All exception paths should be tested:

```csharp
[TestMethod]
public void ComponentDataFactory_GetProcessors_PropagatesProviderException()
{
    // Arrange
    var throwingProvider = new ThrowingComponentDataProvider();
    var factory = new ComponentDataFactory(throwingProvider, null, null, null);

    // Act & Assert
    var exception = Assert.ThrowsExactly<ComponentDataProviderException>(
        () => factory.GetProcessors());
    
    Assert.AreEqual("TestProvider", exception.ProviderName);
}
```

## Decision Tree: Which Exception to Use?

```
Is it a programming error (null argument, invalid state)?
├─ YES → Use standard .NET exception
│  ├─ Null argument? → ArgumentNullException
│  ├─ Invalid argument value? → ArgumentException  
│  └─ Invalid operation for current state? → InvalidOperationException
│
└─ NO → Is it a domain/business logic error?
   └─ YES → Use IronLedger custom exception
      ├─ Provider failure (WMI/CIM)? → ComponentDataProviderException
      ├─ Component data operation? → ComponentDataException
      └─ Other IronLedger error? → IronLedgerException
```

## Best Practices Summary

1. ✅ **DO** use `ArgumentNullException` for null parameters
2. ✅ **DO** use `ArgumentException` for invalid parameter values
3. ✅ **DO** use `ComponentDataProviderException` for provider failures
4. ✅ **DO** include the original exception as `InnerException` when wrapping
5. ✅ **DO** populate `ProviderName` and `WmiClassName` in provider exceptions
6. ✅ **DO** document all exceptions in XML comments
7. ✅ **DO** write tests for exception scenarios
8. ❌ **DON'T** catch exceptions you can't handle meaningfully
9. ❌ **DON'T** catch and re-throw without adding value
10. ❌ **DON'T** hide failures in orchestration layers (factories)

## See Also

- [API Documentation](api/Tudormobile.md) - Complete API reference
- [Source Code](https://github.com/tudormobile/IronLedger) - GitHub repository
- [IronLedgerException.cs](https://github.com/tudormobile/IronLedger/blob/main/src/IronLedgerLib/IronLedgerException.cs) - Exception class definitions
- [ComponentDataProviderExceptionTests.cs](https://github.com/tudormobile/IronLedger/blob/main/src/IronLedgerLib.Tests/Providers/ComponentDataProviderExceptionTests.cs) - Provider exception tests
- [Serialization Guide](serialization.md) - Serialization exception handling examples
