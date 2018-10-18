using System;
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Reliability",
            "CA2001:AvoidCallingProblematicMethods",
            MessageId = "System.Reflection.Assembly.LoadFrom",
            Justification = "Need to load assemblies in order to run tests")]
        public static void Run(string assemblyPath)
        {
            Guard.NotNull(assemblyPath, nameof(assemblyPath));

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
                    break;
                }

                //
                // Interpret as test assembly
                //
                var testAssembly = TestAssembly.TryCreate(assembly);
                if (testAssembly == null)
                {
                    EventHandlers.Raise(new TestAssemblyNotTestEvent() { Path = fullAssemblyPath });
                    break;
                }

                //
                // Activate the test assembly's .config file if present
                //
                ConfigFileSwitcher.SwitchTo(testAssembly.Assembly.Location + ".config");

                //
                // Run [AssemblyInitialize] method
                //
                if (!MethodRunner.RunAssemblyInitializeMethod(testAssembly)) break;

                //
                // Run tests in each [TestClass]
                //
                foreach (var testClass in testAssembly.TestClasses)
                {
                    TestClassRunner.Run(testClass);
                }

                //
                // Run [AssemblyCleanup] method
                //
                MethodRunner.RunAssemblyCleanupMethod(testAssembly);
            }
            while (false);

            EventHandlers.Raise(new TestAssemblyEndEvent());
        }

    }
}
