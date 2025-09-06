# Overview [![NuGet](https://img.shields.io/nuget/v/log4net.ObservableAppender.svg)](https://www.nuget.org/packages/log4net.ObservableAppender) [![Actions Status](../../workflows/.NET%209/badge.svg)](../../actions) [![Coverage Status](https://coveralls.io/repos/github/stop-cran/log4net.ObservableAppender/badge.svg?branch=master)](https://coveralls.io/github/stop-cran/log4net.ObservableAppender?branch=master)

An appender for [log4net](https://github.com/WolfeReiter/log4net), called `ObservableAppender` which exposes all matched log entries as `IObservable<LoggingEvent>` which allows to work with logs using [System.Reactive](https://github.com/dotnet/reactive) framework.
Unlike [MemoryAppender](https://logging.apache.org/log4net/release/sdk/html/T_log4net_Appender_MemoryAppender.htm), it does not persist any logging events by default and thus does not bloat memory in a long run.
However, it has such an option exposed as `Persist` property (false by default).
The appender also supports completion of `IObservable<LoggingEvent>` by logging repository shutdown ar an explicit `IAppender.Close()` call.

# Installation

NuGet package is available [here](https://www.nuget.org/packages/log4net.ObservableAppender/).

```PowerShell
PM> Install-Package log4net.ObservableAppender
```

# Examples

Configure `log4net` with `ObservableAppender` and subscribe on logging events:
```C#
var appender = new ObservableAppender();

Config.BasicConfigurator.Configure(LogManager.GetRepository(Assembly.GetEntryAssembly()), appender);

var logger = LogManager.GetLogger(typeof(ObservableAppenderTests));

appender.LoggingEvents.Subscribe(loggingEvent => ...);
```

Alternatively, configure `log4net` by the app config section:

```XML
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <configSections>
    <section name="log4net" type="log4net.Config.Log4NetConfigurationSectionHandler, log4net" />
    <!-- other sections -->
  </configSections>
  
  <log4net>
    <appender name="ObservableAppender" type="log4net.Appender.ObservableAppender, log4net.ObservableAppender">
      <Persist value="true" />
    </appender>
    <filter type="log4net.Filter.LevelRangeFilter">
      <param name="LevelMax" value="FATAL" />
      <param name="LevelMin" value="DEBUG" />
    </filter>
    <layout type="log4net.Layout.PatternLayout">
      <conversionPattern value="%-5p %d %5rms %-22.22c{1} %-32.32M - %m%n" />
    </layout>
  
    <root>
      <appender-ref ref="ObservableAppender" />
    </root>
  </log4net>
  
  <!-- other sections -->
</configuration>
```

See more examples at [the unit test project](https://github.com/stop-cran/log4net.ObservableAppender/blob/master/log4net.ObservableAppender.Tests/ObservableAppenderTests.cs).