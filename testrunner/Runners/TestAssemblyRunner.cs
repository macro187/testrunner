using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using TestRunner.MSTest;
using TestRunner.Infrastructure;
using TestRunner.Events;

namespace TestRunner.Runners
{
    static class TestAssemblyRunner
    {

        /// <summary>
        /// Run tests in a test assembly
        /// </summary>
        ///
        public static bool Run(string assemblyPath)
        {
            Guard.NotNull(assemblyPath, nameof(assemblyPath));

            var traceListener = new EventTraceListener();
            Trace.Listeners.Add(traceListener);
            try
            {
                return Run2(assemblyPath);
            }
            finally
            {
                Trace.Listeners.Remove(traceListener);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Reliability",
            "CA2001:AvoidCallingProblematicMethods",
            MessageId = "System.Reflection.Assembly.LoadFrom",
            Justification = "Need to load assemblies in order to run tests")]
        static bool Run2(string assemblyPath)
        {
            var success = false;
            bool assemblyInitializeSucceeded = false;
            int failed = 0;
            bool assemblyCleanupSucceeded = false;

            EventHandlers.Raise(new TestAssemblyBeginEvent() { Path = assemblyPath });

            do
            {
                //
                // Resolve full path to test assembly file
                //
                string fullAssemblyPath =
                    Path.IsPathRooted(assemblyPath)
                        ? assemblyPath
                        : Path.Combine(Environment.CurrentDirectory, assemblyPath);

                if (!File.Exists(fullAssemblyPath))
                {
                    EventHandlers.Raise(new TestAssemblyNotFoundEvent() { Path = fullAssemblyPath });
                    break;
                }

                //
                // Load assembly
                //
                Assembly assembly;
                try
                {
                    assembly = Assembly.LoadFrom(fullAssemblyPath);
                }
                catch (BadImageFormatException)
                {
                    EventHandlers.Raise(new TestAssemblyNotDotNetEvent() { Path = fullAssemblyPath });
                    success = true;
                    break;
                }

                //
                // Interpret as test assembly
                //
                var testAssembly = TestAssembly.TryCreate(assembly);
                if (testAssembly == null)
                {
                    EventHandlers.Raise(new TestAssemblyNotTestEvent() { Path = fullAssemblyPath });
                    success = true;
                    break;
                }

                //
                // Activate the test assembly's .config file if present
                //
                ConfigFileSwitcher.SwitchTo(testAssembly.Assembly.Location + ".config");

                //
                // Run [AssemblyInitialize] method
                //
                assemblyInitializeSucceeded = MethodRunner.RunAssemblyInitializeMethod(testAssembly);
                if (!assemblyInitializeSucceeded) break;

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
                assemblyCleanupSucceeded = MethodRunner.RunAssemblyCleanupMethod(testAssembly);

                success = failed == 0 && assemblyCleanupSucceeded;
            }
            while (false);

            EventHandlers.Raise(new TestAssemblyEndEvent() { Success = success });

            return success;
        }

    }
}
