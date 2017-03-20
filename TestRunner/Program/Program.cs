using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using TestRunner.Infrastructure;
using TestRunner.Proxies;

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
                Trace.Listeners.Add(new ConsoleTraceListener());

                //
                // Parse arguments
                //
                var argumentParser = new ArgumentParser(args);
                if (!argumentParser.Success)
                {
                    Console.Out.WriteLine();
                    Console.Out.WriteLine(ArgumentParser.Usage);
                    Console.Out.WriteLine();
                    Console.Out.WriteLine();
                    Console.Out.WriteLine(argumentParser.ErrorMessage);
                    return 1;
                }

                //
                // If --inproc <testassembly>, run tests in <testassembly>
                //
                if (argumentParser.InProc)
                {
                    return RunTestAssembly(argumentParser.AssemblyPaths[0]) ? 0 : 1;
                }

                //
                // Print program banner
                //
                Banner();

                //
                // Reinvoke TestRunner --inproc for each <testassembly> on command line
                //
                bool success = true;
                foreach (var assemblyPath in argumentParser.AssemblyPaths)
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
                Console.Out.WriteLine();
                Console.Out.WriteLine(ue.Message);
                return 1;
            }

            //
            // Handle internal errors
            //
            catch (Exception e)
            {
                Console.Out.WriteLine();
                Console.Out.WriteLine(
                    "An internal error occurred in {0}:",
                    Path.GetFileName(Assembly.GetExecutingAssembly().Location));
                Console.Out.WriteLine(ExceptionExtensions.FormatException(e));
                return 1;
            }
        }


        /// <summary>
        /// Print program information
        /// </summary>
        static void Banner()
        {
            var name = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductName;
            var description = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileDescription;
            var major = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductMajorPart;
            var minor = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductMinorPart;
            var copyright = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).LegalCopyright;
            var authors = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).CompanyName;
            WriteHeading(
                StringExtensions.FormatInvariant("{0} - {1}", name, description),
                StringExtensions.FormatInvariant("Version {0}.{1}", major, minor),
                StringExtensions.FormatInvariant("{0} {1}", copyright, authors));
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

            Console.Out.WriteLine();
            WriteHeading("Assembly: " + assemblyPath);

            //
            // Resolve full path to test assembly
            //
            string fullAssemblyPath = GetFullAssemblyPath(assemblyPath);
            if (!File.Exists(fullAssemblyPath))
            {
                Console.Out.WriteLine();
                Console.Out.WriteLine("Test assembly not found: {0}", fullAssemblyPath);
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
                Console.Out.WriteLine();
                Console.Out.WriteLine("Not a .NET assembly: {0}", fullAssemblyPath);
                return true;
            }
            var testAssembly = TestAssembly.TryCreate(assembly);
            if (testAssembly == null)
            {
                Console.Out.WriteLine();
                Console.Out.WriteLine("Not a test assembly: {0}", fullAssemblyPath);
                return true;
            }
            Console.Out.WriteLine();
            Console.Out.WriteLine("Test Assembly:");
            Console.Out.WriteLine(assembly.Location);

            //
            // Use the test assembly's .config file if present
            //
            UseConfigFile(fullAssemblyPath);

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
        /// Use the test assembly's .config file, if one is present
        /// </summary>
        static void UseConfigFile(string assemblyPath)
        {
            string configPath = assemblyPath + ".config";
            if (!File.Exists(configPath)) return;

            AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", configPath);

            //
            // The following hackery forces the new config file to take effect
            //
            // See http://stackoverflow.com/questions/6150644/change-default-app-config-at-runtime/6151688#6151688
            //
            var initStateField =
                typeof(ConfigurationManager).GetField("s_initState", BindingFlags.NonPublic | BindingFlags.Static);
            if (initStateField != null)
            {
                initStateField.SetValue(null, 0);
            }

            var configSystemField =
                typeof(ConfigurationManager).GetField("s_configSystem", BindingFlags.NonPublic | BindingFlags.Static);
            if (configSystemField != null)
            {
                configSystemField.SetValue(null, null);
            }

            var clientConfigPathsType =
                typeof(ConfigurationManager)
                    .Assembly
                    .GetTypes()
                    .FirstOrDefault(x => x.FullName == "System.Configuration.ClientConfigPaths");
            var currentField =
                clientConfigPathsType != null
                    ? clientConfigPathsType.GetField("s_current", BindingFlags.NonPublic | BindingFlags.Static)
                    : null;
            if (currentField != null)
            {
                currentField.SetValue(null, null);
            }

            Console.Out.WriteLine();
            Console.Out.WriteLine("Configuration File:");
            Console.Out.WriteLine(configPath);

            if (Type.GetType("Mono.Runtime") != null)
            {
                Console.Out.WriteLine();
                Console.Out.WriteLine("WARNING: Running on Mono, configuration file will probably not take effect");
                Console.Out.WriteLine("See https://bugzilla.xamarin.com/show_bug.cgi?id=15741");
            }
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

            Console.Out.WriteLine();
            WriteHeading(testClass.FullName);

            bool classInitializeSucceeded = false;
            int ran = 0;
            int passed = 0;
            int failed = 0;
            int ignored = 0;
            bool classCleanupSucceeded = false;

            if (testClass.IsIgnored)
            {
                Console.Out.WriteLine();
                Console.Out.WriteLine("Ignoring all tests because class is decorated with [Ignore]");
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
                            case TestResult.Passed:
                                passed++;
                                ran++;
                                break;
                            case TestResult.Failed:
                                failed++;
                                ran++;
                                break;
                            case TestResult.Ignored:
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
            Console.Out.WriteLine();
            Console.Out.WriteLine("ClassInitialize: {0}",
                testClass.ClassInitializeMethod == null
                    ? "Not present"
                    : classInitializeSucceeded
                        ? "Succeeded"
                        : "Failed");
            Console.Out.WriteLine("Total:           {0} tests", testClass.TestMethods.Count);
            Console.Out.WriteLine("Ran:             {0} tests", ran);
            Console.Out.WriteLine("Ignored:         {0} tests", ignored);
            Console.Out.WriteLine("Passed:          {0} tests", passed);
            Console.Out.WriteLine("Failed:          {0} tests", failed);
            Console.Out.WriteLine("ClassCleanup:    {0}",
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
        static TestResult RunTest(
            TestMethod testMethod,
            MethodInfo testInitializeMethod,
            MethodInfo testCleanupMethod,
            TestClass testClass)
        {
            WriteSubheading(testMethod.Name.Replace("_", " "));

            if (testMethod.IsIgnored)
            {
                Console.Out.WriteLine();
                Console.Out.WriteLine("Ignored because method is decorated with [Ignore]");
                return TestResult.Ignored;
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

            Console.Out.WriteLine();
            Console.Out.WriteLine(passed ? "Passed" : "FAILED");

            return passed ? TestResult.Passed : TestResult.Failed;
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

            Console.Out.WriteLine();
            Console.Out.WriteLine(prefix + (string.IsNullOrEmpty(prefix) ? "" : " ") + method.Name + "()");

            var watch = new Stopwatch();
            watch.Start();
            bool success = false;
            var parameters = takesTestContext ? new object[] {null} : null;
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
                    Console.Out.WriteLine("  [ExpectedException] {0} occurred:", expectedException.FullName);
                }
                Console.Out.WriteLine(StringExtensions.Indent(ExceptionExtensions.FormatException(ex)));
            }

            Console.Out.WriteLine("  {0} ({1:N0} ms)", success ? "Succeeded" : "Failed", watch.ElapsedMilliseconds);

            return success;
        }


        static void WriteHeading(params string[] lines)
        {
            WriteHeading('=', lines);
        }


        static void WriteSubheading(params string[] lines)
        {
            WriteHeading('-', lines);
        }


        static void WriteHeading(char ruleCharacter, params string[] lines)
        {
            if (lines == null) return;
            if (lines.Length == 0) return;

            var longestLine = lines.Max(line => line.Length);
            var rule = new string(ruleCharacter, longestLine);

            Console.Out.WriteLine();
            Console.Out.WriteLine(rule);
            foreach (var line in lines)
            {
                Console.Out.WriteLine(line);
            }
            Console.Out.WriteLine(rule);
        }


    }
}
