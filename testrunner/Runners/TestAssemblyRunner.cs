using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using TestRunner.Domain;
using TestRunner.Infrastructure;
using TestRunner.Events;
using static TestRunner.Infrastructure.ConsoleExtensions;

namespace TestRunner.Runners
{
    static class TestAssemblyRunner
    {

        /// <summary>
        /// Run tests in a test assembly
        /// </summary>
        ///
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Reliability",
            "CA2001:AvoidCallingProblematicMethods",
            MessageId = "System.Reflection.Assembly.LoadFrom",
            Justification = "Need to load assemblies in order to run tests")]
        static public bool Run(string assemblyPath)
        {
            Guard.NotNull(assemblyPath, nameof(assemblyPath));

            WriteLine();
            WriteHeading("Assembly: " + assemblyPath);

            //
            // Resolve full path to test assembly
            //
            string fullAssemblyPath =
                Path.IsPathRooted(assemblyPath)
                    ? assemblyPath
                    : Path.Combine(Environment.CurrentDirectory, assemblyPath);
            if (!File.Exists(fullAssemblyPath))
            {
                WriteLine();
                WriteLine("Test assembly not found: {0}", fullAssemblyPath);
                return false;
            }

            //
            // Load test assembly
            //
            Assembly assembly;
            try
            {
                assembly = Assembly.LoadFrom(fullAssemblyPath);
            }
            catch (BadImageFormatException)
            {
                WriteLine();
                WriteLine("Not a .NET assembly: {0}", fullAssemblyPath);
                return true;
            }
            var testAssembly = TestAssembly.TryCreate(assembly);
            if (testAssembly == null)
            {
                WriteLine();
                WriteLine("Not a test assembly: {0}", fullAssemblyPath);
                return true;
            }
            WriteLine();
            WriteLine("Test Assembly:");
            WriteLine(assembly.Location);

            //
            // Run the test assembly with System.Diagnostics.Trace output redirected to EventHandler
            //
            var traceListener = new EventTraceListener();
            Trace.Listeners.Add(traceListener);
            bool result;
            try
            {
                result = Run(testAssembly);
            }
            finally
            {
                Trace.Listeners.Remove(traceListener);
            }

            return result;
        }


        static bool Run(TestAssembly testAssembly)
        {
            bool assemblyInitializeSucceeded = false;
            int failed = 0;
            bool assemblyCleanupSucceeded = false;

            //
            // Use the test assembly's .config file if present
            //
            ConfigFileSwitcher.SwitchTo(testAssembly.Assembly.Location + ".config");

            //
            // Run [AssemblyInitialize] method
            //
            TestContext.FullyQualifiedTestClassName = testAssembly.TestClasses.First().FullName;
            TestContext.TestName = testAssembly.TestClasses.First().TestMethods.First().Name;
            TestContext.CurrentTestOutcome = UnitTestOutcome.InProgress;

            assemblyInitializeSucceeded =
                MethodRunner.Run(
                    testAssembly.AssemblyInitializeMethod, null,
                    true,
                    null, false,
                    "[AssemblyInitialize]");

            TestContext.FullyQualifiedTestClassName = null;
            TestContext.TestName = null;
            TestContext.CurrentTestOutcome = UnitTestOutcome.Unknown;

            if (assemblyInitializeSucceeded)
            {
                //
                // Run tests in each [TestClass]
                //
                if (assemblyInitializeSucceeded)
                {
                    foreach (var testClass in testAssembly.TestClasses)
                    {
                        if (!TestClassRunner.Run(testClass))
                        {
                            failed++;
                        }
                    }
                }

                //
                // Run [AssemblyCleanup] method
                //
                assemblyCleanupSucceeded =
                    MethodRunner.Run(
                        testAssembly.AssemblyCleanupMethod, null,
                        false,
                        null, false,
                        "[AssemblyCleanup]");
            }

            return
                assemblyInitializeSucceeded &&
                failed == 0 &&
                assemblyCleanupSucceeded;
        }

    }
}
