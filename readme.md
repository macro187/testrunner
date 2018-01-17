TestRunner
==========

A console MSTest runner


Description
-----------

Lightweight, standalone, no external dependencies.

Cross-platform, works on Microsoft .NET, Mono, Windows, Linux, and Mac OSX.

Reflection-based, works with test assemblies built against any version/variant of the MSTest dll.

Multi-process, runs test assemblies in their own isolated `TestRunner` processes.

Provides [System.Diagnostics.Trace](https://msdn.microsoft.com/en-us/library/system.diagnostics.trace\(v=vs.110\).aspx)
output.

Provides test timing information.

Provides detailed exception information in the case of failures.

Supports
[\[TestClass\]](https://msdn.microsoft.com/en-us/library/microsoft.visualstudio.testtools.unittesting.testclassattribute.aspx),
[\[TestMethod\]](https://msdn.microsoft.com/en-us/library/microsoft.visualstudio.testtools.unittesting.testmethodattribute.aspx),
[\[TestInitialize\]](https://msdn.microsoft.com/en-us/library/microsoft.visualstudio.testtools.unittesting.testinitializeattribute.aspx),
[\[TestCleanup\]](https://msdn.microsoft.com/en-us/library/microsoft.visualstudio.testtools.unittesting.testcleanupattribute.aspx),
[\[ClassInitialize\]](https://msdn.microsoft.com/en-us/library/microsoft.visualstudio.testtools.unittesting.classinitializeattribute.aspx),
[\[ClassCleanup\]](https://msdn.microsoft.com/en-us/library/microsoft.visualstudio.testtools.unittesting.classcleanupattribute.aspx),
[\[AssemblyInitialize\]](https://msdn.microsoft.com/en-us/library/microsoft.visualstudio.testtools.unittesting.assemblyinitializeattribute.aspx),
[\[AssemblyCleanup\]](https://msdn.microsoft.com/en-us/library/microsoft.visualstudio.testtools.unittesting.assemblycleanupattribute.aspx),
[\[ExpectedException\]](https://msdn.microsoft.com/en-us/library/microsoft.visualstudio.testtools.unittesting.expectedexceptionattribute.aspx),
and
[\[Ignore\]](https://msdn.microsoft.com/en-us/library/microsoft.visualstudio.testtools.unittesting.ignoreattribute.aspx).

Supports test assembly `.config` files (but not on Mono yet due to a
[bug](https://bugzilla.xamarin.com/show_bug.cgi?id=15741))


Requirements
------------

Microsoft .NET Framework v4.0 or newer, or [Mono](http://www.mono-project.com/) v2.10 or newer.


Synopsis
--------

    TestRunner.exe <testassembly> [...]


Options
-------

    <testassembly> - Path to an assembly containing MSTest tests


Exit Status
-----------

    0 if all <testassembly>s listed on the command line PASS
    - OR -
    Non-zero in all other cases

    A <testassembly> is said to PASS if all test, initialization, and cleanup
    methods it contains execute successfully
    - OR -
    It contains no tests
    - OR -
    It is not a .NET assembly

    A <testassembly> is said to FAIL in all other cases, including if the file
    does not exist.


Examples
--------

### Windows

    C:\> TestRunner.exe C:\Path\To\TestAssembly.dll C:\Path\To\AnotherTestAssembly.dll


### Unix or Mac

    $ mono --debug TestRunner.exe /path/to/TestAssembly.dll /path/to/AnotherTestAssembly.dll


NuGet Package
-------------

Available as a NuGet package named [TestRunner](https://www.nuget.org/packages/TestRunner/).


Building
--------

Use Visual Studio, MSBuild, or [XBuild](http://www.mono-project.com/docs/tools+libraries/tools/xbuild/).

Older versions of XBuild may not understand the tools and language versions in the project file(s).
Try `xbuild /toolsversion:4.0 /property:LangVersion=default`.


License
-------

[MS-PL](https://github.com/macro187/testrunner/blob/master/license.txt)


Copyright
---------

Copyright (c) 2012-2017  
Ron MacNeil \<<https://github.com/macro187>\>  
Rickenberg \<<https://www.codeplex.com/site/users/view/Rickenberg>\>  
JDeltorp \<<https://github.com/JDeltorp>\>  


History
-------

Forked from <https://testrunner.codeplex.com/> revision 87713 on September 24th, 2016.


Continuous Integration
----------------------

### Appveyor (Windows)

[![Build status](https://ci.appveyor.com/api/projects/status/v8s72ij64an7kr87?svg=true)](https://ci.appveyor.com/project/macro187/testrunner)

### Travis (Linux)

[![Build Status](https://travis-ci.org/macro187/testrunner.svg?branch=master)](https://travis-ci.org/macro187/testrunner)

