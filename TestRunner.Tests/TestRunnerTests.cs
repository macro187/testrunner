using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            var args = string.Format("\"{0}\"", testSuite);
            string output;
            int exitCode;
            using (var proc = new Process())
            {
                bool exited = false;
                var sb = new StringBuilder();

                proc.StartInfo.FileName = testRunner;
                proc.StartInfo.Arguments = args;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.OutputDataReceived += (s,e) => {
                    Console.WriteLine(e.Data ?? "");
                    sb.AppendLine(e.Data ?? "");
                };
                proc.EnableRaisingEvents = true;
                proc.Exited += (s,e) => exited = true;

                Console.WriteLine("{0} {1}", testRunner, args);
                proc.Start();
                proc.BeginOutputReadLine();
                while (!exited) Thread.Yield();

                exitCode = proc.ExitCode;
                output = sb.ToString();
            }

            //
            // Check stuff
            //
            Assert.AreEqual(
                0, exitCode,
                "TestRunner.exe returned non-zero exit code");

            Assert.IsTrue(
                output.Contains(TestSuite.TestSuiteTests.TestCleanupMessage),
                "[TestCleanup] method did not run");

            Assert.IsTrue(
                output.Contains(TestSuite.TestSuiteTests.ClassCleanupMessage),
                "[ClassCleanup] method did not run");

            Assert.IsTrue(
                output.Contains(TestSuite.TestSuiteTests.AssemblyCleanupMessage),
                "[AssemblyCleanup] method did not run");

            Assert.IsFalse(
                output.Contains(TestSuite.TestSuiteTests.IgnoredTestMessage),
                "An [Ignore]d test method ran");
        }

    }
}
