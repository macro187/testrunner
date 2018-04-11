using System;
#if NET461
using System.Configuration;
#endif
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
                RunMethod(
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
                        if (!RunTestClass(testClass))
                        {
                            failed++;
                        }
                    }
                }

                //
                // Run [AssemblyCleanup] method
                //
                assemblyCleanupSucceeded =
                    RunMethod(
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


        /// <summary>
        /// Run tests in a [TestClass]
        /// </summary>
        /// <returns>
        /// Whether everything in <paramref name="testClass"/> succeeded
        /// </returns>
        static bool RunTestClass(TestClass testClass)
        {
            Guard.NotNull(testClass, "testClass");

            WriteLine();
            WriteHeading(testClass.FullName);

            bool classInitializeSucceeded = false;
            int ran = 0;
            int passed = 0;
            int failed = 0;
            int ignored = 0;
            bool classCleanupSucceeded = false;

            if (testClass.IsIgnored)
            {
                WriteLine();
                WriteLine("Ignoring all tests because class is decorated with [Ignore]");
                ignored = testClass.TestMethods.Count;
            }
            else
            {
                //
                // Run [ClassInitialize] method
                //
                classInitializeSucceeded =
                    RunMethod(
                        testClass.ClassInitializeMethod, null,
                        true,
                        null, false,
                        "[ClassInitialize]");

                if (classInitializeSucceeded)
                {
                    //
                    // Run [TestMethod]s
                    //
                    foreach (var testMethod in testClass.TestMethods)
                    {
                        switch(
                            RunTest(
                                testMethod,
                                testClass.TestInitializeMethod,
                                testClass.TestCleanupMethod,
                                testClass))
                        {
                            case UnitTestOutcome.Passed:
                                passed++;
                                ran++;
                                break;
                            case UnitTestOutcome.Failed:
                                failed++;
                                ran++;
                                break;
                            case UnitTestOutcome.NotRunnable:
                                ignored++;
                                break;
                        }
                    }

                    //
                    // Run [ClassCleanup] method
                    //
                    classCleanupSucceeded =
                        RunMethod(
                            testClass.ClassCleanupMethod, null,
                            false,
                            null, false,
                            "[ClassCleanup]");
                }
            }

            //
            // Print results
            //
            WriteSubheading("Summary");
            WriteLine();
            WriteLine("ClassInitialize: {0}",
                testClass.ClassInitializeMethod == null
                    ? "Not present"
                    : classInitializeSucceeded
                        ? "Succeeded"
                        : "Failed");
            WriteLine("Total:           {0} tests", testClass.TestMethods.Count);
            WriteLine("Ran:             {0} tests", ran);
            WriteLine("Ignored:         {0} tests", ignored);
            WriteLine("Passed:          {0} tests", passed);
            WriteLine("Failed:          {0} tests", failed);
            WriteLine("ClassCleanup:    {0}",
                testClass.ClassCleanupMethod == null
                    ? "Not present"
                    : classCleanupSucceeded
                        ? "Succeeded"
                        : "Failed");

            return
                classInitializeSucceeded &&
                failed == 0 &&
                classCleanupSucceeded;
        }


        /// <summary>
        /// Run a test method (plus its intialize and cleanup methods, if present)
        /// </summary>
        /// <remarks>
        /// If the test method is decorated with [Ignore], nothing is run
        /// </remarks>
        /// <returns>
        /// The results of the test
        /// </returns>
        static UnitTestOutcome RunTest(
            TestMethod testMethod,
            MethodInfo testInitializeMethod,
            MethodInfo testCleanupMethod,
            TestClass testClass)
        {
            WriteSubheading(testMethod.Name.Replace("_", " "));

            if (testMethod.IsIgnored)
            {
                WriteLine();
                WriteLine("Ignored because method is decorated with [Ignore]");
                return UnitTestOutcome.NotRunnable;
            }

            //
            // Construct an instance of the test class
            //
            var testInstance = Activator.CreateInstance(testClass.Type);

            //
            // Invoke [TestInitialize], [TestMethod], and [TestCleanup]
            //
            bool testInitializeSucceeded = false;
            bool testMethodSucceeded = false;
            bool testCleanupSucceeded = false;

            testInitializeSucceeded =
                RunMethod(
                    testInitializeMethod, testInstance,
                    false,
                    null, false,
                    "[TestInitialize]");

            if (testInitializeSucceeded)
            {
                testMethodSucceeded =
                    RunMethod(
                        testMethod.MethodInfo, testInstance,
                        false,
                        testMethod.ExpectedException, testMethod.AllowDerivedExpectedExceptionTypes,
                        "[TestMethod]");

                testCleanupSucceeded =
                    RunMethod(
                        testCleanupMethod, testInstance,
                        false,
                        null, false,
                        "[TestCleanup]");
            }

            bool passed = testInitializeSucceeded && testMethodSucceeded && testCleanupSucceeded;

            WriteLine();
            WriteLine(passed ? "Passed" : "FAILED");

            return passed ? UnitTestOutcome.Passed : UnitTestOutcome.Failed;
        }


        /// <summary>
        /// Run a test-related method using reflection
        /// </summary>
        /// <returns>
        /// Whether the method ran successfully
        /// </returns>
        static bool RunMethod(
            MethodInfo method,
            object instance,
            bool takesTestContext,
            Type expectedException,
            bool expectedExceptionAllowDerived,
            string prefix)
        {
            prefix = prefix ?? "";

            if (method == null) return true;

            WriteLine();
            WriteLine(prefix + (string.IsNullOrEmpty(prefix) ? "" : " ") + method.Name + "()");

            var watch = new Stopwatch();
            watch.Start();
            bool success = false;
            var parameters = takesTestContext ? new object[] { TestContextProxy.Proxy } : null;
            try
            {
                method.Invoke(instance, parameters);
                watch.Stop();
                success = true;
            }
            catch (TargetInvocationException tie)
            {
                watch.Stop();
                var ex = tie.InnerException;
                bool expected = 
                    expectedException != null &&
                    (
                        ex.GetType() == expectedException ||
                        (expectedExceptionAllowDerived && ex.GetType().IsSubclassOf(expectedException))
                    );
                    
                if (expected)
                {
                    success = true;
                    WriteLine("  [ExpectedException] {0} occurred:", expectedException.FullName);
                }
                WriteLine(StringExtensions.Indent(ExceptionExtensions.FormatException(ex)));
            }

            WriteLine("  {0} ({1:N0} ms)", success ? "Succeeded" : "Failed", watch.ElapsedMilliseconds);

            return success;
        }

    }
}
