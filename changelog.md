TestRunner Changelog
====================


v1.10.0
-------

Add --method command line option


v1.9.0
------

Switch to MIT license

Add --class command line option

Add experimental machine-readable output format

Correctly capture stderr output from child processes

Correctly propagate --outputformat to child processes


v1.8.0
------

Add --outputformat command line option

Add --help command line option

Correctly consider \[Ignore\]d \[TestClass\]es to be successful


v1.7.1
------

Stop occasionally printing output from child processes out of order, by
correctly waiting until child processes are completely finished before
proceeding


v1.7.0
------

Add .NET Core support


v1.6.0
------

Add support for `TestContext` `.CurrentTestOutcome`,
`.FullyQualifiedTestClassName`, and `.TestName`


v1.5.1
------

Fix build on Mono 5 which no longer supports `__MonoCS__` preprocessor
variable


v1.5
----

Support multiple test assemblies in a single `TestRunner` invocation


v1.4
----

Stop hanging indefinitely if there are leftover threads, by exiting the
program using
[Environment.Exit](https://msdn.microsoft.com/en-us/library/system.environment.exit.aspx)


v1.3
----

Eliminate dependency on
`Microsoft.VisualStudio.QualityTools.UnitTestFramework.dll` by using
reflection to discover and run tests


v1.2
----

Don't try to run test or cleanup methods if relevant initialize methods fail


v1.1
----

Improve \[ExpectedException\] output

Don't crash in environments where the internal framework details required for
test assembly `.config` file loading aren't present


v1.0
----

<https://en.wikipedia.org/wiki/Minimum_viable_product>
