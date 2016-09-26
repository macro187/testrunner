TestRunner
==========

A console MSTest runner


Description
-----------

Lightweight, stand-alone, no external dependencies.

Cross-platform, works on Microsoft .NET, Mono, Windows, Linux, and Mac OSX.

Includes test timing information.

Includes detailed exception information in the case of failures.

Includes [System.Diagnostics.Trace](https://msdn.microsoft.com/en-us/library/system.diagnostics.trace\(v=vs.110\).aspx)
output.

Supports [\[TestClass\]](https://msdn.microsoft.com/en-us/library/microsoft.visualstudio.testtools.unittesting.testclassattribute.aspx)

Supports [\[TestMethod\]](https://msdn.microsoft.com/en-us/library/microsoft.visualstudio.testtools.unittesting.testmethodattribute.aspx)

Supports [\[TestInitialize\]](https://msdn.microsoft.com/en-us/library/microsoft.visualstudio.testtools.unittesting.testinitializeattribute.aspx)

Supports [\[TestCleanup\]](https://msdn.microsoft.com/en-us/library/microsoft.visualstudio.testtools.unittesting.testcleanupattribute.aspx)

Supports [\[Ignore\]](https://msdn.microsoft.com/en-us/library/microsoft.visualstudio.testtools.unittesting.ignoreattribute.aspx)


Requirements
------------

Microsoft .NET Framework v4.0 or newer, or [Mono](http://www.mono-project.com/) v2.10 or newer.


Building
--------

Use Visual Studio, MSBuild, or [XBuild](http://www.mono-project.com/docs/tools+libraries/tools/xbuild/).

Older versions of XBuild may not understand the tools and language versions in the project file(s).
Try `xbuild /toolsversion:4.0 /property:LangVersion=default`.


Synopsis
--------

`TestRunner.exe` *`<testassembly>`*


Options
-------

*`testassembly`* - Path to an assembly containing MSTest tests


Exit Status
-----------

0 if all test, initialization, and cleanup methods succeeded.  Otherwise, non-zero.


Copyright
---------

Copyright (c) 2012-2016  
Rickenberg \<<https://www.codeplex.com/site/users/view/Rickenberg>\>  
Ron MacNeil \<<macro@hotmail.com>\>  


License
-------

MS-PL, see [license.txt](license.txt).


History
-------

Forked from <https://testrunner.codeplex.com/> revision 87713 on September 24th, 2016.
