# Information about Logging

This library uses my [RJCP.DLL.Trace](https://github.com/jcurl/RJCP.DLL.Trace)
module to enable logging using .NET Core in a simple way.

The filter this library uses is `RJCP.Diagnostics.Log.Dlt`.

## Enabling Logging in Your Applications

By using the [RJCP.DLL.Trace](https://github.com/jcurl/RJCP.DLL.Trace) module,
logging for the library can be initialized as a static singleton (the same way
it was done in .NET 4.x).

In your application, reference the `RJCP.Diagnostics.Trace` library, and add the
following code somewhere in your initialization path.

```csharp
internal static ILoggerFactory GetConsoleFactory() {
    return LoggerFactory.Create(builder => {
        builder
            .AddFilter("RJCP.Diagnostics.Log", LogLevel.Debug)
            .AddConsole();
    });
}
```

And then assign it:

```csharp
LogSource.SetLoggerFactory(GetConsoleFactory());
```

For more information, see
[README.md](https://github.com/jcurl/RJCP.DLL.Trace/blob/master/README.md)

## Logging with NUnit

If you use NUnit for your logging, you can include the project
[RJCP.DLL.CodeQuality](https://github.com/jcurl/RJCP.DLL.CodeQuality) and use
the extensions to get an `ILogger` that debugs output to NUnit.

```csharp
// This file is only for .NET Core

using Microsoft.Extensions.Logging;
using RJCP.CodeQuality.NUnitExtensions.Trace;
using RJCP.Diagnostics.Trace;

internal static class GlobalLogger {
    static GlobalLogger() {
        ILoggerFactory factory = LoggerFactory.Create(builder => {
            builder
                .AddFilter("Microsoft", LogLevel.Warning)
                .AddFilter("System", LogLevel.Warning)
                .AddFilter("RJCP.Diagnostics.Log", LogLevel.Debug)
                .AddNUnitLogger();
        });
        LogSource.SetLoggerFactory(factory);
    }

    // Just calling this method will result in the static constructor being executed.
    public static void Initialize() {
        /* Can be empty, reference will initialize static constructor */
    }
}
```

This exact code is used by this project to log while running test cases.
