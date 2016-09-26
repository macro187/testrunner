using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestRunner
{
    static class Program
    {

        /// <summary>
        /// Program entry point
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            try
            {
                //
                // Route trace output to stdout
                //
                Trace.Listeners.Add(new ConsoleTraceListener());

                //
                // Print program banner
                //
                Banner();

                //
                // Parse arguments
                //
                var argumentParser = new ArgumentParser(args);
                if (!argumentParser.Success)
                {
                    Console.WriteLine();
                    Console.WriteLine(argumentParser.Usage);
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine(argumentParser.ErrorMessage);
                    return 1;
                }

                //
                // Resolve full path to test assembly
                //
                string fullAssemblyPath = GetFullAssemblyPath(argumentParser.AssemblyPath);
                if (!File.Exists(fullAssemblyPath))
                {
                    Console.WriteLine();
                    Console.WriteLine("Assembly '{0}' not found", fullAssemblyPath);
                    return 1;
                }

                //
                // Load test assembly
                //
                var assembly = Assembly.LoadFrom(fullAssemblyPath);

                //
                // Pull in test assembly .config file if present
                //
                UseConfigFile(assembly);

                //
                // Run tests in assembly
                //
                var success = RunTestAssembly(assembly);
                return success ? 0 : 1;
            }

            //
            // Handle internal errors
            //
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine(
                    "An internal error occurred in {0}:",
                    Path.GetFileName(Assembly.GetExecutingAssembly().Location));
                Console.WriteLine(FormatException(e));
                return 1;
            }
        }


        /// <summary>
        /// Print program information
        /// </summary>
        static void Banner()
        {
            var name = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductName;
            var major = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductMajorPart;
            var minor = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductMinorPart;
            var copyright = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).LegalCopyright;
            var description = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).Comments;
            WriteHeading(
                $"{name} - {description}",
                $"Version {major}.{minor}",
                copyright);
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
        /// Activate the test assembly's .config file, if one is present
        /// </summary>
        static void UseConfigFile(Assembly assembly)
        {
            string configPath = assembly.Location + ".config";
            if (!File.Exists(configPath)) return;
            AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", configPath);
            Console.WriteLine();
            Console.WriteLine(string.Format("Using configuration file: '{0}'", configPath));
        }


        /// <summary>
        /// Run tests in a test assembly
        /// </summary>
        public static bool RunTestAssembly(Assembly testAssembly)
        {
            if (testAssembly == null) throw new ArgumentNullException(nameof(testAssembly));

            Console.WriteLine();
            Console.WriteLine(testAssembly.Location);

            var testClasses =
                testAssembly.GetTypes()
                    .Where(t => t.GetCustomAttributes(typeof(TestClassAttribute), false).Any())
                    .OrderBy(t => t.Name);

            int failed = 0;
            foreach (var testClass in testClasses)
            {
                if (!RunTestClass(testClass))
                {
                    failed++;
                }
            }

            return (failed == 0);
        }


        /// <summary>
        /// Run tests in a [TestClass]
        /// </summary>
        /// <returns>
        /// Whether everything in <paramref name="testClass"/> succeeded
        /// </returns>
        static bool RunTestClass(Type testClass)
        {
            if (testClass == null) throw new ArgumentNullException(nameof(testClass));

            Console.WriteLine();
            WriteHeading(testClass.FullName);

            bool ignore = testClass.GetCustomAttributes(typeof(IgnoreAttribute), false).Any();

            //
            // Locate methods
            //
            var testInitializeMethod = testClass.GetMethods()
                .FirstOrDefault(m => m.GetCustomAttributes(typeof(TestInitializeAttribute), false).Any());

            var testCleanupMethod = testClass.GetMethods()
                .FirstOrDefault(m => m.GetCustomAttributes(typeof(TestCleanupAttribute), false).Any());

            var testMethods = testClass.GetMethods()
                .Where(m => m.GetCustomAttributes(typeof(TestMethodAttribute), false).Count() != 0)
                .OrderBy(m => m.Name)
                .ToList();

            //
            // Construct an instance of the test class
            //
            var testInstance = Activator.CreateInstance(testClass);

            //
            // Run tests
            //
            int ran = 0;
            int passed = 0;
            int failed = 0;
            int ignored = 0;
            if (!ignore)
            {
                foreach (var testMethod in testMethods)
                {
                    switch(RunTest(testMethod, testInitializeMethod, testCleanupMethod, testInstance))
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
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Ignoring all tests because class is decorated with [Ignore]");
                ignored = testMethods.Count;
            }

            //
            // Print results
            //
            WriteSubheading("Summary");
            Console.WriteLine();
            Console.WriteLine("Total:   " + testMethods.Count.ToString() + " tests");
            Console.WriteLine("Ran:     " + ran.ToString() + " tests");
            Console.WriteLine("Ignored: " + ignored.ToString() + " tests");
            Console.WriteLine("Passed:  " + passed.ToString() + " tests");
            Console.WriteLine("Failed:  " + failed.ToString() + " tests");

            return (failed == 0);
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
            MethodInfo testMethod,
            MethodInfo testInitializeMethod,
            MethodInfo testCleanupMethod,
            object testInstance)
        {
            WriteSubheading(testMethod.Name.Replace("_", " "));

            bool ignore = testMethod.GetCustomAttributes(typeof(IgnoreAttribute), false).Any();
            if (ignore)
            {
                Console.WriteLine();
                Console.WriteLine("Ignored because method is decorated with [Ignore]");
                return TestResult.Ignored;
            }

            if (
                RunInstanceMethod(testInitializeMethod, testInstance, "[TestInitialize]") &&
                RunInstanceMethod(testMethod, testInstance, "[TestMethod]") &&
                RunInstanceMethod(testCleanupMethod, testInstance, "[TestCleanup]"))
            {
                Console.WriteLine();
                Console.WriteLine("Passed");
                return TestResult.Passed;
            }

            Console.WriteLine();
            Console.WriteLine("FAILED");
            return TestResult.Failed;
        }


        /// <summary>
        /// Run an instance method
        /// </summary>
        /// <returns>
        /// Whether the method ran successfully
        /// </returns>
        static bool RunInstanceMethod(MethodInfo method, object testInstance, string prefix)
        {
            if (testInstance == null) throw new ArgumentNullException(nameof(testInstance));
            prefix = prefix ?? "";

            if (method == null) return true;

            Console.WriteLine();
            Console.WriteLine(prefix + (prefix != "" ? " " : "") + method.Name + "()");

            var watch = new Stopwatch();
            watch.Start();
            bool success = false;
            try
            {
                method.Invoke(testInstance, null);
                watch.Stop();
                success = true;
            }
            catch (Exception ex)
            {
                watch.Stop();
                ex = UnwrapTargetInvocationException(ex);
                Console.WriteLine(Indent(FormatException(ex)));
            }

            Console.WriteLine("  {0} ({1:N0} ms)", success ? "Succeeded" : "Failed", watch.ElapsedMilliseconds);

            return success;
        }


        static Exception UnwrapTargetInvocationException(Exception ex)
        {
            var tie = ex as TargetInvocationException;
            if (tie == null) return ex;
            return tie.InnerException;
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

            Console.WriteLine();
            Console.WriteLine(rule);
            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }
            Console.WriteLine(rule);
        }


        static string FormatException(Exception e)
        {
            if (e == null) return "";
            var sb = new StringBuilder();
            sb.AppendLine(e.Message);
            sb.AppendLine("Type: " + e.GetType().FullName);
            if (e.Data != null)
            {
                foreach (var key in e.Data.Keys)
                {
                    sb.AppendLine(string.Format(
                        "Data.{0}: {1}",
                        key.ToString(),
                        e.Data[key].ToString()));
                }
            }
            if (!string.IsNullOrWhiteSpace(e.Source))
            {
                sb.AppendLine("Source: " + e.Source);
            }
            if (!string.IsNullOrWhiteSpace(e.HelpLink))
            {
                sb.AppendLine("HelpLink: " + e.HelpLink);
            }
            if (!string.IsNullOrWhiteSpace(e.StackTrace))
            {
                sb.AppendLine("StackTrace:");
                sb.AppendLine(Indent(FormatStackTrace(e.StackTrace)));
            }
            if (e.InnerException != null)
            {
                sb.AppendLine("InnerException:");
                sb.AppendLine(Indent(FormatException(e.InnerException)));
            }
            return sb.ToString();
        }


        static string FormatStackTrace(string stackTrace)
        {
            return string.Join(
                Environment.NewLine,
                SplitLines(stackTrace)
                    .Select(line => line.Trim())
                    .SelectMany(line => {
                        var i = line.IndexOf(" in ");
                        if (i <= 0) return new[] {line};
                        var inPart = line.Substring(i + 1);
                        var atPart = line.Substring(0, i);
                        return new[] {atPart, Indent(inPart)};
                        }));
        }


        static string Indent(string theString)
        {
            if (theString == null) throw new ArgumentNullException(nameof(theString));
            var lines = SplitLines(theString);
            var indentedLines = lines.Select(s => "  " + s);
            return string.Join(Environment.NewLine, indentedLines);
        }


        static string[] SplitLines(string theString)
        {
            theString = theString.Replace("\r\n", "\n").Replace("\r", "\n");
            if (theString.EndsWith("\n"))
            {
                theString = theString.Substring(0, theString.Length-1);
            }
            return theString.Split('\n');
        }


    }
}
