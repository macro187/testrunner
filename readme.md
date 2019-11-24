TestRunner
==========

A console **MSTest** runner



Features
========

**Lightweight**, with no external dependencies.

**Cross-platform**, tested on .NET Framework (Windows), .NET Core (Windows and
Linux), and Mono (Linux).

**Process isolation** runs test assemblies in separate processes.

**Reflection-based test discovery** supports test assemblies built against any
variant or version of the MSTest dll.

**Test output** captured from Console.Out, Console.Error, and
System.Diagnostics.Trace.

**Timings** measured for all test, initialize, and cleanup methods.

**Exception details** provided for expected exceptions in tests, and unexpected
failures in test, initialize, and cleanup methods.

**Test attributes** supported include
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

**TestContext** members supported include
[CurrentTestOutcome](https://docs.microsoft.com/en-gb/dotnet/api/microsoft.visualstudio.testtools.unittesting.testcontext.currenttestoutcome),
[FullyQualifiedTestClassName](https://docs.microsoft.com/en-gb/dotnet/api/microsoft.visualstudio.testtools.unittesting.testcontext.fullyqualifiedtestclassname),
and
[TestName](https://docs.microsoft.com/en-gb/dotnet/api/microsoft.visualstudio.testtools.unittesting.testcontext.testname).

**Assembly .config files** are supported.



Limitations
===========

Some test attributes are unsupported and are ignored and have no effect.

Some TestContext members are unsupported and return `null` at runtime.

Assembly .config files don't work on Mono.
[Issue #17](https://github.com/macro187/testrunner/issues/17).

`<bindingRedirect>`s in assembly .config files have no effect.



Requirements
============

.NET Framework 4.6.1 (or newer)

.NET Core 2.0 (or newer)

Mono 5.0.0 (or newer)



NuGet Package
=============

<https://www.nuget.org/packages/TestRunner/>

Includes `net461` binaries for .NET Framework and Mono, and `netcoreapp2.0`
binaries for .NET Core.



Usage
=====

```
SYNOPSIS

    testrunner.exe [options] <testfile>...
    testrunner.exe --help

DESCRIPTION

    Run tests in <testfile>(s)

OPTIONS

    --outputformat <outputformat>
        Set the output format

        human
            Human-readable text format (default)

        machine
            Machine-readable JSON-based format (experimental)

    --class <namespace>.<class>
    --class <class>
        Run the specified test class.

        If <namespace> is omitted, run all test classes with the specified
        name.

        If not specified, run all test classes.

        Can be specified multiple times.

        Case-sensitive.

        Does not override [Ignore] attributes.

    --method <namespace>.<class>.<method>
    --method <method>
        Run the specified test method.

        If <namespace> and <class> are omitted, run all test methods with
        the specified name (constrained by --class).

        If not specified, run all test methods (constrained by --class).

        Can be specified multiple times.

        Case-sensitive.

        Does not override [Ignore] attributes.

    --help
        Show usage information

EXIT STATUS

    0 if all specified test files succeed, non-zero otherwise.

    Test files succeed if all test, initialization, and cleanup methods run
    successfully.

    Test files succeed if they contain no tests.

    Test files succeed if they are not .NET assemblies.

    Test files fail if any test, initialization, or cleanup methods fail.

    Test files fail if they do not exist.

EXAMPLES

    .NET Framework

        testrunner.exe C:\Path\To\TestAssembly.dll C:\Path\To\AnotherTestAssembly.dll

    .NET Core

        dotnet testrunner.dll C:\Path\To\TestAssembly.dll C:\Path\To\AnotherTestAssembly.dll

    Mono

        mono --debug testrunner.exe /path/to/TestAssembly.dll /path/to/AnotherTestAssembly.dll
```



History
=======

Forked from [Bernd Rickenberg](https://github.com/rickenberg)'s
<https://testrunner.codeplex.com/> revision 87713 on September 24th, 2016.

