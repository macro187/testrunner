using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
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
            var here = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var testRunner = Path.Combine(here, "TestRunner.exe");
            var testSuite = Path.Combine(here, "TestRunner.TestSuite.dll");
            var args = string.Format("\"{0}\"", testSuite);

            using (var proc = new Process())
            {
                bool exited = false;

                proc.StartInfo.FileName = testRunner;
                proc.StartInfo.Arguments = args;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.OutputDataReceived += (s,e) => Console.WriteLine(e.Data ?? "");
                proc.EnableRaisingEvents = true;
                proc.Exited += (s,e) => exited = true;

                Console.WriteLine("{0} {1}", testRunner, args);
                proc.Start();
                proc.BeginOutputReadLine();
                while (!exited) Thread.Yield();

                Assert.AreEqual(0, proc.ExitCode);
            }
        }

    }
}
