using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestRunner.Infrastructure;

namespace TestRunner.Tests
{

    [TestClass]
    public class TestRunnerTests
    {

        [TestMethod]
        public void Run_TestSuite_With_TestRunner()
        {
            //
            // Locate the TestRunner program and the test suite dll
            //
            var here = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var testRunner = Path.Combine(here, "TestRunner.exe");
            var testSuite = Path.Combine(here, "TestRunner.TestSuite.dll");

            //
            // Run TestRunner against the test suite dll
            //
            var results = ProcessExtensions.Execute(testRunner, string.Format("\"{0}\"", testSuite));

            //
            // Check stuff
            //
            Assert.AreEqual(
                0, results.ExitCode,
                "TestRunner.exe returned non-zero exit code");

            Assert.IsTrue(
                results.Output.Contains(TestSuite.TestSuiteTests.TestCleanupMessage),
                "[TestCleanup] method did not run");

            Assert.IsTrue(
                results.Output.Contains(TestSuite.TestSuiteTests.ClassCleanupMessage),
                "[ClassCleanup] method did not run");

            Assert.IsTrue(
                results.Output.Contains(TestSuite.TestSuiteTests.AssemblyCleanupMessage),
                "[AssemblyCleanup] method did not run");

            Assert.IsTrue(
                results.Output.Contains(TestSuite.TestSuiteTests.TraceTestMessage),
                "System.Diagnostics.Trace test message was not printed");

            Assert.IsFalse(
                results.Output.Contains(TestSuite.TestSuiteTests.IgnoredTestMessage),
                "An [Ignore]d test method ran");
        }

    }
}
