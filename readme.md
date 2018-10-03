TestRunner
==========

A console MSTest runner


Features
========

Lightweight, standalone, no external dependencies.

Cross-platform, tested on Microsoft .NET Framework and .NET Core on Windows,
and Mono and .NET Core on Linux.

Multi-process, runs test assemblies in their own isolated processes.

Reflection-based, runs test assemblies built against any variant or version of
the MSTest dll.

Provides
[System.Diagnostics.Trace](https://docs.microsoft.com/en-gb/dotnet/api/system.diagnostics.trace)
output.

Provides test timing information.

Provides detailed exception information in the case of failures.

Supports test attributes
[\[TestClass\]](https://docs.microsoft.com/en-gb/dotnet/api/microsoft.visualstudio.testtools.unittesting.testclassattribute),
[\[TestMethod\]](https://docs.microsoft.com/en-gb/dotnet/api/microsoft.visualstudio.testtools.unittesting.testmethodattribute),
[\[TestInitialize\]](https://docs.microsoft.com/en-gb/dotnet/api/microsoft.visualstudio.testtools.unittesting.testinitializeattribute),
[\[TestCleanup\]](https://docs.microsoft.com/en-gb/dotnet/api/microsoft.visualstudio.testtools.unittesting.testcleanupattribute),
[\[ClassInitialize\]](https://docs.microsoft.com/en-gb/dotnet/api/microsoft.visualstudio.testtools.unittesting.classinitializeattribute),
[\[ClassCleanup\]](https://docs.microsoft.com/en-gb/dotnet/api/microsoft.visualstudio.testtools.unittesting.classcleanupattribute),
[\[AssemblyInitialize\]](https://docs.microsoft.com/en-gb/dotnet/api/microsoft.visualstudio.testtools.unittesting.assemblyinitializeattribute),
[\[AssemblyCleanup\]](https://docs.microsoft.com/en-gb/dotnet/api/microsoft.visualstudio.testtools.unittesting.assemblycleanupattribute),
[\[ExpectedException\]](https://docs.microsoft.com/en-gb/dotnet/api/microsoft.visualstudio.testtools.unittesting.expectedexceptionattribute),
and
[\[Ignore\]](https://docs.microsoft.com/en-gb/dotnet/api/microsoft.visualstudio.testtools.unittesting.ignoreattribute).

Supports [TestContext](https://docs.microsoft.com/en-gb/dotnet/api/microsoft.visualstudio.testtools.unittesting.testcontext)
members `CurrentTestOutcome`, `FullyQualifiedTestClassName`, and `TestName`.

Supports test assembly `.config` files.


Limitations
===========

Partial test attribute coverage (see Features).

Partial `TestContext` coverage on .NET Framework (see Features).

Test assembly `.config` files don't work on Mono because of a
[bug in Mono](https://bugzilla.xamarin.com/show_bug.cgi?id=15741).

`<bindingRedirect>`s in test assembly `.config` files may not work.


Requirements
============

.NET Framework v4.6.1 (or newer) OR .NET Core 2.0 (or newer) OR Mono v5.0.0
(or newer).


NuGet Package
=============

Available as a NuGet package named [TestRunner](https://www.nuget.org/packages/TestRunner/).


Synopsis
========

```
testrunner.exe <testfile>...
testrunner.exe --help
```


Options
=======

```
<testfile>
    Path(s) to one or more .NET assembly file(s) containing tests

--help
    Show usage information
```


Exit Status
===========

0 if all specified test files succeed, non-zero otherwise.

Test files succeed if all test, initialization, and cleanup methods run
successfully.

Test files succeed if they contain no tests.

Test files succeed if they are not .NET assemblies.

Test files fail if any test, initialization, or cleanup methods fail.

Test files fail if they do not exist.


Examples
========

.NET Framework
--------------

```
C:\> testrunner.exe C:\Path\To\TestAssembly.dll C:\Path\To\AnotherTestAssembly.dll
```


.NET Core
---------

```
C:\> dotnet testrunner.dll C:\Path\To\TestAssembly.dll C:\Path\To\AnotherTestAssembly.dll
```


Mono
----

```
$ mono --debug testrunner.exe /path/to/TestAssembly.dll /path/to/AnotherTestAssembly.dll
```


Building
========

.NET Framework
--------------

```
dotnet publish -f net461
```


.NET Core
---------

```
dotnet publish -f netcoreapp2.0
```


Mono
----

```
msbuild /p:TargetFramework=net461 /t:Restore
msbuild /p:TargetFramework=net461 /t:Rebuild
msbuild /p:TargetFramework=net461 /t:Publish
```


License
=======

MS-PL \<<https://github.com/macro187/testrunner/blob/master/license.txt>\>


Copyright
=========

Copyright (c) 2012-2018  
Ron MacNeil \<<https://github.com/macro187>\>  
Rickenberg \<<https://www.codeplex.com/site/users/view/Rickenberg>\>  
JDeltorp \<<https://github.com/JDeltorp>\>  


History
=======

Forked from \<<https://testrunner.codeplex.com/>\> revision 87713 on September 24th, 2016


Continuous Integration
======================


Appveyor (Windows)
------------------------------------

[![Build status](https://ci.appveyor.com/api/projects/status/v8s72ij64an7kr87?svg=true)](https://ci.appveyor.com/project/macro187/testrunner)

net461 build and tests

netcoreapp2.0 build and tests


Travis (Linux)
------------------------------------

[![Build Status](https://travis-ci.org/macro187/testrunner.svg?branch=master)](https://travis-ci.org/macro187/testrunner)

net461 build and tests (Mono)

netcoreapp2.0 build and tests (.NET Core)

