using System.IO;
using System.Reflection;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestRunner.Infrastructure;

namespace TestRunner.Tests
{

    [TestClass]
    public class TestRunnerTests
    {
        
        static string here = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        static string testRunner = Path.Combine(here, "TestRunner.exe");
        static string passTests = Path.Combine(here, "TestRunner.Tests.Pass.dll");
        static string failTests = Path.Combine(here, "TestRunner.Tests.Fail.dll");
        static string msTestTests = Path.Combine(here, "TestRunner.Tests.MSTest.dll");
        static string differentConfigTests = Path.Combine(here, "TestRunner.Tests.DifferentConfigValue.dll");
        static string fakeDll = Path.Combine(here, "FakeDll.dll");
        static string referencedAssembly = Path.Combine(here, "TestRunner.Tests.ReferencedAssembly.dll");


        static string Quote(params string[] arguments)
        {
            return string.Join(" ", arguments.Select(arg => "\"" + arg + "\""));
        }


        [TestMethod]
        public void Pass_Yields_ExitCode_0()
        {
            Assert.AreEqual(
                0,
                ProcessExtensions.Execute(testRunner, Quote(passTests)).ExitCode);
        }


        [TestMethod]
        public void Fail_Yields_ExitCode_1()
        {
            Assert.AreEqual(
                1,
                ProcessExtensions.Execute(testRunner, Quote(failTests)).ExitCode);
        }


        [TestMethod]
        public void Non_Existent_File_Yields_ExitCode_1()
        {
            Assert.AreEqual(
                1,
                ProcessExtensions.Execute(testRunner, Quote("no-such.dll")).ExitCode);
        }


        [TestMethod]
        public void MSTest_Suite_Passes()
        {
            var results = ProcessExtensions.Execute(testRunner, Quote(msTestTests));

            Assert.AreEqual(
                0, results.ExitCode,
                "TestRunner.exe returned non-zero exit code");

            Assert.IsTrue(
                results.Output.Contains(MSTest.MSTestTests.TestCleanupMessage),
                "[TestCleanup] method did not run");

            Assert.IsTrue(
                results.Output.Contains(MSTest.MSTestTests.ClassCleanupMessage),
                "[ClassCleanup] method did not run");

            Assert.IsTrue(
                results.Output.Contains(MSTest.MSTestTests.AssemblyCleanupMessage),
                "[AssemblyCleanup] method did not run");

            Assert.IsTrue(
                results.Output.Contains(MSTest.MSTestTests.TraceTestMessage),
                "System.Diagnostics.Trace test message was not printed");

            Assert.IsFalse(
                results.Output.Contains(MSTest.MSTestTests.IgnoredTestMessage),
                "An [Ignore]d test method ran");
        }


        [TestMethod]
        public void Many_Passing_Assemblies_Yields_ExitCode_0()
        {
            Assert.AreEqual(
                0,
                ProcessExtensions.Execute(testRunner, Quote(passTests, passTests)).ExitCode);
        }


        [TestMethod]
        public void One_Failing_Assembly_In_Many_Yields_ExitCode_1()
        {
            Assert.AreEqual(
                1,
                ProcessExtensions.Execute(testRunner, Quote(failTests, passTests)).ExitCode);
        }


        [TestMethod]
        public void Many_Assemblies_With_Different_Config_Files_Pass()
        {
            Assert.AreEqual(
                0,
                ProcessExtensions.Execute(testRunner, Quote(msTestTests, differentConfigTests)).ExitCode);
        }


        [TestMethod]
        public void Non_Test_Assembly_Yields_ExitCode_0()
        {
            Assert.AreEqual(
                0,
                ProcessExtensions.Execute(testRunner, Quote(referencedAssembly)).ExitCode);
        }


        [TestMethod]
        public void Non_DotNet_Dll_Yields_ExitCode_0()
        {
            Assert.AreEqual(
                0,
                ProcessExtensions.Execute(testRunner, Quote(fakeDll)).ExitCode);
        }

    }
}
