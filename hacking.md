Building
========

.NET Framework
```
dotnet publish -f net461
```

.NET Core
```
dotnet publish -f netcoreapp2.0
```

Mono
```
msbuild /p:TargetFramework=net461 /t:Restore
msbuild /p:TargetFramework=net461 /t:Rebuild
msbuild /p:TargetFramework=net461 /t:Publish
```



Continuous Integration
======================

Appveyor (Windows)
------------------

[![Build status](https://ci.appveyor.com/api/projects/status/v8s72ij64an7kr87?svg=true)](https://ci.appveyor.com/project/macro187/testrunner)

net461 build and tests

netcoreapp2.0 build and tests


Travis (Linux)
--------------

[![Build Status](https://travis-ci.org/macro187/testrunner.svg?branch=master)](https://travis-ci.org/macro187/testrunner)

net461 build and tests (Mono)

netcoreapp2.0 build and tests (.NET Core)



Design
======

Program
-------

The `Program` namespace contains the `Main()` program code that sets up error
handlers, parses command line arguments, and takes appropriate top-level
actions.

The initial parent `testrunner` process runs the test file(s) specified on the
command-line by reinvoking separate child `testrunner` processes for each.
Child processes are instructed via command-line options to produce output in a
machine-readable format, which the parent interprets and combines into a unified
event stream for final analysis and output.


Runners
-------

Child processes run test files by delegating to runner routines in the `Runners`
namespace.  These routines activate test assembly `.config` files, locate test
classes, and run initialize, test, and cleanup methods in the right order.


MSTest
------

The runners interpret and interact with test assemblies through types in the
`MSTest` namespace.  These mirror the attributes and other elements found in the
*MSTest* DLLs referenced by test assemblies, except they bind to those elements
at runtime using reflection.  There is also a reflection-based `TestContext`
implementation.  Together, they allow `testrunner` to work with test assemblies
referencing any of the various available versions of the *MSTest* DLL.


Events
------

As the runners execute tests, they emit events into a chain of event handlers in
the `Events` namespace.  Individual handlers focus on single supporting
responsibilities like measurement, analysis, aggregation, and output.
Distributing responsibility across the runners and handlers keeps down their
individual complexity.


Results
-------

As tests run, results are recorded in types from the `Results` namespace.


Infrastructure
--------------

The `Infrastructure` namespace contains general support functionality used
throughout the rest of the application.

