Lego
====
Warning this is a work in progress.

[Lego](http://www.latinwordlist.com/latin-words/lego-16886571.htm) is the Latin Word that has many meanings, mainly: to gather, choose, collect, pass through, read.

Lego is a Windows Service that gathers performance counter and metrics.  It will publish the performance counters to a [Graphite](http://graphite.readthedocs.org/en/latest/overview.html) server.

## Configuration

Lego allows the configuration of one or more sets of performance counters to be collected.  Each set is collected at the same time and has the same scheduled interval. There are two supported file formats:

1. Data Collector set Template file. These are standard Data Collector Set templates created using Performance monitor. http://technet.microsoft.com/en-ca/library/cc766318.aspx

2. Performance Monitor settings file

