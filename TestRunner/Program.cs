using System;
using System.Collections.Generic;
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

        [STAThread]
        static int Main(string[] args)
        {
            Banner();

            if (args.Count() != 1)
            {
                Usage();
                return 1;
            }

            string assemblyPath = GetFullAssemblyPath(args[0]);

            if (!File.Exists(assemblyPath))
            {
                Console.WriteLine("The specified assembly could not be found at '{0}'.", assemblyPath);
                return 1;
            }

            var isAllPassed = RunTests(assemblyPath);
            return isAllPassed ? 0 : 1;
        }


        /// <summary>
        /// Print program information
        /// </summary>
        private static void Banner()
        {
            WriteHeading(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "{0} v{1}.{2}",
                    FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductName,
                    FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductMajorPart,
                    FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductMinorPart),
                FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).LegalCopyright);
        }


        /// <summary>
        /// Print usage information
        /// </summary>
        private static void Usage()
        {
            var description = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).Comments;
            var fileName = Path.GetFileName(Assembly.GetExecutingAssembly().Location);

            Console.WriteLine();
            Console.WriteLine(description);
            Console.WriteLine();
            Console.WriteLine("Usage");
            Console.WriteLine();
            Console.WriteLine("    {0} <assemblypath>", fileName);
            Console.WriteLine();
            Console.WriteLine("        assemblypath - Path to an assembly containing MSTest unit tests");
            Console.WriteLine();
            Console.WriteLine("Examples");
            Console.WriteLine();
            Console.WriteLine("    {0} MyTestAssembly.dll", fileName);
            Console.WriteLine();
            Console.WriteLine("    {0} C:\\foo\\MyTestAssembly.dll", fileName);
        }


        /// <summary>
        /// Runs the tests.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        public static bool RunTests(string assemblyPath)
        {
            try
            {
                // 1. Load the assembly.
                Assembly assembly = GetAssembly(assemblyPath);

                // 2. Get test classes.
                var classes = assembly.GetTypes()
                    .Where(t => t.GetCustomAttributes(typeof(TestClassAttribute), false).Count() != 0)
                    .OrderBy(t => t.Name);

                if (!classes.Any())
                    return true;

                // 3. Get test methods for each class.
                Stats stats = new Stats();
                List<MethodInfo> methods = null;
                foreach (var current in classes)
                {
                    try
                    {
                        methods = current.GetMethods()
                            .Where(m => m.GetCustomAttributes(typeof(TestMethodAttribute), false).Count() != 0)
                            .OrderBy(m => m.Name)
                            .ToList();

                        if (!methods.Any())
                            continue;

                        string title = String.Format("{0}: {1} test(s)", current.FullName, methods.Count());
                        WriteSubheading(title);

                        object instance = assembly.CreateInstance(current.FullName);

                        // 3.5 Run test initialize.
                        var initializeMethod = current.GetMethods()
                            .FirstOrDefault(m => m.GetCustomAttributes(typeof(TestInitializeAttribute), false).Any());

                        if (initializeMethod != null)
                        {
                            initializeMethod.Invoke(instance, null);
                        }

                        // 4. Run test methods.
                        foreach (var method in methods)
                        {
                            try
                            {
                                stats.AddGlobalCount();
                                Console.WriteLine();
                                Console.WriteLine("Running test: {0}", method.Name);
                                stats.StartLocalTime();
                                method.Invoke(instance, null);
                                Console.WriteLine("  Passed ({0} s).", stats.LocalTime.TotalSeconds);
                                stats.AddGlobalPassCount();
                            }
                            catch (Exception ex)
                            {
                                stats.AddGlobalFailCount();
                                // Check for a failing assert.
                                if (ex.InnerException != null && ex.InnerException.GetType() == typeof(AssertFailedException))
                                {
                                    Console.WriteLine("  Failed: {0} ({1} s).", ex.InnerException.Message, stats.LocalTime.TotalSeconds);
                                    continue;
                                }

                                // Unexpected error.
                                Console.WriteLine("  An unexpected error occured: {0}", ex.Message);
                                if (ex.InnerException != null)
                                    Console.WriteLine("  Reason: {0}", ex.InnerException.Message);
                            }
                            finally
                            {
                                stats.ResetLocalTime();
                            }
                        }
                        Console.WriteLine();
                        Console.WriteLine(stats.GetFinalResult());
                    }
                    catch (Exception ex)
                    {
                        // Unexpected error.
                        Console.WriteLine("  An unexpected error occured: {0}", ex.Message);
                        if (ex.InnerException != null)
                            Console.WriteLine("  Reason: {0}", ex.InnerException.Message);
                        Console.WriteLine();
                        if (methods != null)
                        {
                            foreach (var method in methods)
                            {
                                stats.AddGlobalFailCount();
                            }
                        }
                    }
                }
                Console.WriteLine();

                return stats.GlobalFailCount == 0;
            }
            catch (Exception ex)
            {
                // Unexpected error.
                Console.WriteLine("  An unexpected error occured: {0}", ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine("  Reason: {0}", ex.InnerException.Message);

                return false;
            }
        }


        private static Assembly GetAssembly(string assemblyPath)
        {
            Assembly assembly = Assembly.LoadFrom(assemblyPath);

            string configPath = assembly.Location + ".config";
            if (File.Exists(configPath))
            {
                AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", configPath);
                Console.WriteLine(String.Format("Configuration file loaded from: '{0}'", configPath));
                Console.WriteLine();
            }

            return assembly;
        }


        private static void WriteHeading(params string[] lines)
        {
            WriteHeading('=', lines);
        }


        private static void WriteSubheading(params string[] lines)
        {
            WriteHeading('-', lines);
        }


        private static void WriteHeading(char ruleCharacter, params string[] lines)
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


        private static string GetFullAssemblyPath(string path)
        {
            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(
                    Environment.CurrentDirectory,
                    path);
            }

            return path;
        }


    }
}
