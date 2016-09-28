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

        static bool testRunnerDone = false;
        static string testRunnerOutput = "";


        [TestMethod]
        public void Run_TestSuite_With_TestRunner()
        {
            try
            {
                var here = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var testRunner = Path.Combine(here, "TestRunner.exe");
                var testSuite = Path.Combine(here, "TestRunner.TestSuite.dll");
                var args = string.Format("\"{0}\"", testSuite);

                using (var proc = new Process())
                {
                    bool exited = false;
                    var output = new StringBuilder();

                    proc.StartInfo.FileName = testRunner;
                    proc.StartInfo.Arguments = args;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.OutputDataReceived += (s,e) => {
                        Console.WriteLine(e.Data ?? "");
                        output.AppendLine(e.Data ?? "");
                    };
                    proc.EnableRaisingEvents = true;
                    proc.Exited += (s,e) => exited = true;

                    Console.WriteLine("{0} {1}", testRunner, args);
                    proc.Start();
                    proc.BeginOutputReadLine();
                    while (!exited) Thread.Yield();
                    testRunnerOutput = output.ToString();

                    Assert.AreEqual(0, proc.ExitCode);
                }
            }
            finally
            {
                testRunnerDone = true;
            }
        }


        [TestMethod]
        public void TestCleanup_Ran()
        {
            while (!testRunnerDone) Thread.Yield();
            Assert.IsTrue(testRunnerOutput.Contains(TestSuite.TestSuiteTests.TestCleanupMessage));
        }


        [TestMethod]
        public void ClassCleanup_Ran()
        {
            while (!testRunnerDone) Thread.Yield();
            Assert.IsTrue(testRunnerOutput.Contains(TestSuite.TestSuiteTests.ClassCleanupMessage));
        }


        [TestMethod]
        public void AssemblyCleanup_Ran()
        {
            while (!testRunnerDone) Thread.Yield();
            Assert.IsTrue(testRunnerOutput.Contains(TestSuite.TestSuiteTests.AssemblyCleanupMessage));
        }

    }
}
