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
                if (!ParseArgs(args))
                {
                    Usage();
                    return 1;
                }

                //
                // Resolve full path to test assembly
                //
                string fullAssemblyPath = GetFullAssemblyPath(assemblyPath);
                if (!File.Exists(fullAssemblyPath))
                {
                    Console.WriteLine("The test assembly '{0}' could not be found", fullAssemblyPath);
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
                var result = RunTests(assembly);
                return result ? 0 : 1;
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


        static string assemblyPath;


        static bool ParseArgs(string[] args)
        {
            if (args.Length != 1) return false;
            assemblyPath = args[0];
            return true;
        }


        /// <summary>
        /// Print program information
        /// </summary>
        static void Banner()
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
        static void Usage()
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


        static string GetFullAssemblyPath(string path)
        {
            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(
                    Environment.CurrentDirectory,
                    path);
            }

            return path;
        }


        static void UseConfigFile(Assembly assembly)
        {
            string configPath = assembly.Location + ".config";
            if (File.Exists(configPath))
            {
                AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", configPath);
                Console.WriteLine();
                Console.WriteLine(string.Format("Using configuration file: '{0}'", configPath));
            }
        }


        /// <summary>
        /// Run tests in an assembly
        /// </summary>
        public static bool RunTests(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            Console.WriteLine();
            Console.WriteLine("Test Assembly:");
            Console.WriteLine(assembly.Location);

            try
            {
                // Get test classes.
                var classes = assembly.GetTypes()
                    .Where(t => t.GetCustomAttributes(typeof(TestClassAttribute), false).Count() != 0)
                    .OrderBy(t => t.Name);

                // Get test methods for each class.
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

                        WriteSubheading("[TestClass]", current.FullName);

                        object instance = assembly.CreateInstance(current.FullName);

                        // Run test initialize.
                        var initializeMethod = current.GetMethods()
                            .FirstOrDefault(m => m.GetCustomAttributes(typeof(TestInitializeAttribute), false).Any());

                        if (initializeMethod != null)
                        {
                            initializeMethod.Invoke(instance, null);
                        }

                        // Run test methods.
                        foreach (var method in methods)
                        {
                            try
                            {
                                stats.AddGlobalCount();
                                Console.WriteLine();
                                Console.WriteLine("[TestMethod]");
                                Console.WriteLine(method.Name + "()");
                                stats.StartLocalTime();
                                method.Invoke(instance, null);
                                Console.WriteLine("  Passed ({0} s)", stats.LocalTime.TotalSeconds);
                                stats.AddGlobalPassCount();
                            }
                            catch (Exception ex)
                            {
                                stats.AddGlobalFailCount();
                                Console.WriteLine();
                                Console.WriteLine(Indent(FormatException(UnwrapTargetInvocationException(ex))));
                                Console.WriteLine("  Failed ({0} s)", stats.LocalTime.TotalSeconds);
                                continue;
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
                        Console.WriteLine("  An unexpected error occured:");
                        Console.WriteLine(Indent(FormatException(UnwrapTargetInvocationException(ex))));
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
                Console.WriteLine("  An unexpected error occured:");
                Console.WriteLine(Indent(FormatException(UnwrapTargetInvocationException(ex))));
                return false;
            }
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
            return theString.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
        }


    }
}
