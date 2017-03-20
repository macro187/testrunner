using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestRunner.Infrastructure;

namespace TestRunner.Tests
{

    [TestClass]
    public class TestRunnerTests
    {
        
        static string here = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        static string testRunner = Path.Combine(here, "TestRunner.exe");
        static string msTestTests = Path.Combine(here, "TestRunner.Tests.MSTest.dll");
        static string differentConfigTests = Path.Combine(here, "TestRunner.Tests.DifferentConfigValue.dll");
        static string fakeDll = Path.Combine(here, "FakeDll.dll");
        static string referencedAssembly = Path.Combine(here, "TestRunner.Tests.ReferencedAssembly.dll");


        [TestMethod]
        public void MSTest_Suite_Passes()
        {
            //
            // Run TestRunner against the test suite dll
            //
            var results = ProcessExtensions.Execute(testRunner, string.Format("\"{0}\"", msTestTests));

            //
            // Check stuff
            //
            Assert.AreEqual(
                0, results.ExitCode,
                "TestRunner.exe returned non-zero exit code");

            Assert.IsTrue(
                results.Output.Contains(Tests.MSTest.MSTestTests.TestCleanupMessage),
                "[TestCleanup] method did not run");

            Assert.IsTrue(
                results.Output.Contains(Tests.MSTest.MSTestTests.ClassCleanupMessage),
                "[ClassCleanup] method did not run");

            Assert.IsTrue(
                results.Output.Contains(Tests.MSTest.MSTestTests.AssemblyCleanupMessage),
                "[AssemblyCleanup] method did not run");

            Assert.IsTrue(
                results.Output.Contains(Tests.MSTest.MSTestTests.TraceTestMessage),
                "System.Diagnostics.Trace test message was not printed");

            Assert.IsFalse(
                results.Output.Contains(Tests.MSTest.MSTestTests.IgnoredTestMessage),
                "An [Ignore]d test method ran");
        }


        [TestMethod]
        public void Multiple_Passing_Assemblies_Yields_ExitCode_0()
        {
            //
            // Use TestRunner to run the test suite twice in the same invocation
            //
            var results = ProcessExtensions.Execute(testRunner, string.Format("\"{0}\" \"{0}\"", msTestTests));

            //
            // Check stuff
            //
            Assert.AreEqual(
                0, results.ExitCode,
                "TestRunner.exe returned non-zero exit code");
        }


        [TestMethod]
        public void Assemblies_With_Different_Config_Files_Pass()
        {
            //
            // Use TestRunner to run the test suite twice in the same invocation
            //
            var results = ProcessExtensions.Execute(
                testRunner,
                string.Format("\"{0}\" \"{1}\"", msTestTests, differentConfigTests));

            //
            // Check stuff
            //
            Assert.AreEqual(
                0, results.ExitCode,
                "TestRunner.exe returned non-zero exit code");
        }


        [TestMethod]
        public void Non_Test_Assembly_Yields_ExitCode_0()
        {
            //
            // Use TestRunner to run the test suite twice in the same invocation
            //
            var results = ProcessExtensions.Execute(testRunner, string.Format("\"{0}\"", referencedAssembly));

            //
            // Check stuff
            //
            Assert.AreEqual(
                0, results.ExitCode,
                "TestRunner.exe returned non-zero exit code");
        }


        [TestMethod]
        public void Non_DotNet_Dll_Yields_ExitCode_0()
        {
            //
            // Use TestRunner to run the test suite twice in the same invocation
            //
            var results = ProcessExtensions.Execute(testRunner, string.Format("\"{0}\"", fakeDll));

            //
            // Check stuff
            //
            Assert.AreEqual(
                0, results.ExitCode,
                "TestRunner.exe returned non-zero exit code");
        }

    }
}
