TestRunner
==========

A console MSTest runner


Requirements
------------

Microsoft .NET Framework 4.0 or newer, or equivalent [Mono](http://www.mono-project.com/).


Features
--------

Lightweight, stand-alone, no external dependencies.

Includes test timing information.

Includes detailed exception information in the case of failures.

Includes [System.Diagnostics.Trace](https://msdn.microsoft.com/en-us/library/system.diagnostics.trace\(v=vs.110\).aspx)
output.

Supports [\[TestClass\]](https://msdn.microsoft.com/en-us/library/microsoft.visualstudio.testtools.unittesting.testclassattribute.aspx)

Supports [\[TestMethod\]](https://msdn.microsoft.com/en-us/library/microsoft.visualstudio.testtools.unittesting.testmethodattribute.aspx)

Supports [\[TestInitialize\]](https://msdn.microsoft.com/en-us/library/microsoft.visualstudio.testtools.unittesting.testinitializeattribute.aspx)

Supports [\[TestCleanup\]](https://msdn.microsoft.com/en-us/library/microsoft.visualstudio.testtools.unittesting.testcleanupattribute.aspx)

Supports [\[Ignore\]](https://msdn.microsoft.com/en-us/library/microsoft.visualstudio.testtools.unittesting.ignoreattribute.aspx)


Usage
-----

`TestRunner.exe` *`<testassembly>`*

*`testassembly`* - Path to an assembly containing MSTest tests


Returns
-------

Exit code 0 if all test, initialization, and cleanup methods succeeded

*- OR -*

A non-zero exit code if anything failed


License
-------

[MS-PL](https://opensource.org/licenses/MS-PL)


History
-------

Forked from <https://testrunner.codeplex.com/> revision 87713 on September 24th, 2016.
