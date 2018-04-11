using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using TestRunner.Infrastructure;
using TestRunner.Domain;
using static TestRunner.Infrastructure.ConsoleExtensions;

namespace TestRunner.Program
{
    static class Program
    {

        /// <summary>
        /// Program entry point
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Environment.Exit(Main2(args));
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Required to handle unexpected exceptions")]
        static int Main2(string[] args)
        {
            try
            {
                //
                // Route trace output to stdout
                //
                Trace.Listeners.Add(new TestRunnerTraceListener());

                //
                // Parse arguments
                //
                ArgumentParser.Parse(args);
                if (!ArgumentParser.Success)
                {
                    WriteLine();
                    WriteLine(ArgumentParser.GetUsage());
                    WriteLine();
                    WriteLine();
                    WriteLine(ArgumentParser.ErrorMessage);
                    return 1;
                }

                //
                // If --inproc <testassembly>, run tests in <testassembly>
                //
                if (ArgumentParser.InProc)
                {
                    return RunTestAssembly(ArgumentParser.AssemblyPaths[0]) ? 0 : 1;
                }

                //
                // Print program banner
                //
                Banner();

                //
                // Reinvoke TestRunner --inproc for each <testassembly> on command line
                //
                bool success = true;
                foreach (var assemblyPath in ArgumentParser.AssemblyPaths)
                {
                    if (ExecuteInNewProcess(assemblyPath) != 0) success = false;
                }
                return success ? 0 : 1;
            }

            //
            // Handle user-facing errors
            //
            catch (UserException ue)
            {
                WriteLine();
                WriteLine(ue.Message);
                return 1;
            }

            //
            // Handle internal errors
            //
            catch (Exception e)
            {
                WriteLine();
                WriteLine(
                    "An internal error occurred in {0}:",
                    Path.GetFileName(Assembly.GetExecutingAssembly().Location));
                WriteLine(ExceptionExtensions.FormatException(e));
                return 1;
            }
        }


        /// <summary>
        /// Print program information
        /// </summary>
        static void Banner()
        {
            var name = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductName;
            var version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
            var copyright = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).LegalCopyright;
            var authors = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).CompanyName;

            WriteHeading(
                $"{name} v{version}",
                copyright,
                authors
                );
        }


        /// <summary>
        /// Reinvoke TestRunner to run tests in a test assembly in a separate process
        /// </summary>
        static int ExecuteInNewProcess(string assemblyPath)
        {
            //
            // TODO
            // Teach how to reinvoke using mono.exe if that's how the initial invocation was done
            //
            var testRunner = Assembly.GetExecutingAssembly().Location;
            return ProcessExtensions.Execute(testRunner, "--inproc \"" + assemblyPath + "\"").ExitCode;
        }


        /// <summary>
        /// Run tests in a test assembly
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Reliability",
            "CA2001:AvoidCallingProblematicMethods",
            MessageId = "System.Reflection.Assembly.LoadFrom",
            Justification = "Need to load assemblies in order to run tests")]
        static bool RunTestAssembly(string assemblyPath)
        {
            Guard.NotNull(assemblyPath, "assemblyPath");

            WriteLine();
            WriteHeading("Assembly: " + assemblyPath);

            //
            // Resolve full path to test assembly
            //
            string fullAssemblyPath = GetFullAssemblyPath(assemblyPath);
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
            // Use the test assembly's .config file if present
            //
            ConfigFileSwitcher.SwitchTo(fullAssemblyPath + ".config");

            bool assemblyInitializeSucceeded = false;
            int failed = 0;
            bool assemblyCleanupSucceeded = false;

            //
            // Run [AssemblyInitialize] method
            //
            assemblyInitializeSucceeded =
                MethodRunner.Run(
                    testAssembly.AssemblyInitializeMethod, null,
                    true,
                    null, false,
                    "[AssemblyInitialize]");

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


        /// <summary>
        /// Resolve full path to test assembly
        /// </summary>
        static string GetFullAssemblyPath(string path)
        {
            return Path.IsPathRooted(path)
                ? path
                : Path.Combine(Environment.CurrentDirectory, path);
        }


    }
}
