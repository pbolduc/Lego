Lego
====
Warning this is a work in progress.

[Lego](http://www.latinwordlist.com/latin-words/lego-16886571.htm) is the Latin Word that has many meanings, mainly: to gather, choose, collect, pass through, read.

Lego is a Windows Service that gathers performance counter and metrics.  It will publish the performance counters to a [Graphite](http://graphite.readthedocs.org/en/latest/overview.html) server.

## Motivation

The [Tools That Work With Graphite](http://graphite.readthedocs.org/en/latest/tools.html) page lists 
[Graphite PowerShell Functions](https://github.com/MattHodge/Graphite-PowerShell-Functions) as tool
can do exactly what Lego does plus more.  You can also find a number of other libraries on Github that
can publish performance counters to Graphite. 

All of the components I reviewed use the built in 
[PerformanceCounter](http://msdn.microsoft.com/en-us/library/system.diagnostics.performancecounter(v=vs.110).aspx)
class to read the performance counters.

Enter [Tx](https://github.com/MSOpenTech/Tx). [Tx](https://github.com/MSOpenTech/Tx) uses interop services to call directly into 
Performance Data Helper DLL (pdh.dll). [Tx](https://github.com/MSOpenTech/Tx) makes it easy to set up an observer that is pushed each 
sample. Some early benchmarks (to be published later) show that querying 4 local counters using the 
[PerformanceCounter](http://msdn.microsoft.com/en-us/library/system.diagnostics.performancecounter(v=vs.110).aspx) class takes
50% - 60% more elapsed time as compared to using Performance Data Helper.

## Goals

* Minimal CPU consumption - should not impact other applications/services that are running on the same machine.
* Minimal Memory consumption
* Fault Tolerant - will buffer samples in a ring buffer (based on Signal-R's ring buffer) until connection to Graphite server is re-established

## Configuration

*As stated above this is a work in progress.  Right now the counters to collected are hard coded in two referenced files.  This will be changed shortly to allows
the end user to configure as many sets as required.*

Lego allows the configuration of one or more sets of performance counters to be collected.  Each set is collected at the same time and has the same scheduled interval. There are two supported file formats:

1. Data Collector set Template file. These are standard Data Collector Set templates created using Performance monitor. http://technet.microsoft.com/en-ca/library/cc766318.aspx

2. Performance Monitor settings file

## Dependencies

I am attempting to keep the core library and Windows Service only requiring .NET Framework 4.   

* .NET Framework 4 (note Lego.Service currently depends on .NET Framework 4.5, will be downgraded)
* [Tx.Windows](http://www.nuget.org/packages/Tx.Windows/)
* [Topshelf](http://www.nuget.org/packages/Topshelf/)
* [Serilog](http://www.nuget.org/packages/Serilog/)
