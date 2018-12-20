# Overview [![NuGet](https://img.shields.io/nuget/v/log4net.ObservableAppender.svg)](https://www.nuget.org/packages/log4net.ObservableAppender) [![Build Status](https://travis-ci.com/stop-cran/log4net.ObservableAppender.svg?branch=master)](https://travis-ci.com/stop-cran/log4net.ObservableAppender)

An appender for [log4net](https://github.com/WolfeReiter/log4net), called `ObservableAppender` which exposes all matched log entries as `IObservable<LoggingEvent>` which allows to work with logs using [System.Reactive](https://github.com/dotnet/reactive) framework.
Unlike [MemoryAppender](https://logging.apache.org/log4net/release/sdk/html/T_log4net_Appender_MemoryAppender.htm), it does not persist any logging events by default.
However, it has such an option exposed as `Persist` property (false by default).
The appender also supports completion of `IObservable<LoggingEvent>` by logging repository shutdown ar an explicit `IAppender.Close()` call.

# Installation

NuGet package is available [here](https://www.nuget.org/packages/log4net.ObservableAppender/).

```PowerShell
PM> Install-Package log4net.ObservableAppender
```

# Examples

TODO