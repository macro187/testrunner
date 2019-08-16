using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using TestRunner.Infrastructure;

namespace TestRunner.Program
{

    /// <summary>
    /// Command line argument parser
    /// </summary>
    ///
    static class ArgumentParser
    {

        static List<string> _classes = new List<string>();
        static List<string> _methods = new List<string>();
        static List<string> _testFiles = new List<string>();


        /// <summary>
        /// Were the command line arguments valid?
        /// </summary>
        ///
        static public bool Success
        {
            get;
            private set;
        }


        /// <summary>
        /// User-facing error message if not <see cref="Success"/>
        /// </summary>
        ///
        static public string ErrorMessage
        {
            get;
            private set;
        }


        /// <summary>
        /// The specified --class options
        /// </summary>
        ///
        static public IReadOnlyCollection<string> Classes { get; } = new ReadOnlyCollection<string>(_classes);


        /// <summary>
        /// The specified --method options
        /// </summary>
        ///
        static public IReadOnlyCollection<string> Methods { get; } = new ReadOnlyCollection<string>(_methods);


        /// <summary>
        /// The specifed --outputformat option (default: human)
        /// </summary>
        ///
        static public string OutputFormat
        {
            get;
            private set;
        }


        /// <summary>
        /// Was the --help option specified?
        /// </summary>
        ///
        static public bool Help
        {
            get;
            private set;
        }


        /// <summary>
        /// Was the --inproc option specified?
        /// </summary>
        ///
        static public bool InProc
        {
            get;
            private set;
        }


        /// <summary>
        /// Path(s) to test assemblies listed on the command line
        /// </summary>
        ///
        static public IReadOnlyList<string> TestFiles { get; } = new ReadOnlyCollection<string>(_testFiles);


        /// <summary>
        /// Produce user-facing command line usage information
        /// </summary>
        ///
        static public string[] GetUsage()
        {
            var fileName = Path.GetFileName(Assembly.GetExecutingAssembly().Location);
            bool isUnix = new[] { PlatformID.Unix, PlatformID.MacOSX }.Contains(Environment.OSVersion.Platform);

            var shellPrefix =
                isUnix
                    ? "$"
                    : "C:\\>";

            var examplePath =
                isUnix
                    ? "/path/to/"
                    : "C:\\path\\to\\";

            return
                new[] {
                    $"SYNOPSIS",
                    $"",
                    $"    {fileName} [options] <file>...",
                    $"    {fileName} --help",
                    $"",
                    $"DESCRIPTION",
                    $"",
                    $"    Run tests in <file>(s)",
                    $"",
                    $"OPTIONS",
                    $"",
                    $"    --outputformat <outputformat>",
                    $"        Set the output format",
                    $"",
                    $"        human",
                    $"            Human-readable text format (default)",
                    $"",
                    $"        machine",
                    $"            Machine-readable JSON-based format (experimental)",
                    $"",
                    $"    --class <namespace>.<class>",
                    $"    --class <class>",
                    $"        Run the specified test class.",
                    $"",
                    $"        If <namespace> is omitted, run all test classes with the specified",
                    $"        name.",
                    $"",
                    $"        If not specified, run all test classes.",
                    $"",
                    $"        Can be specified multiple times.",
                    $"",
                    $"        Case-sensitive.",
                    $"",
                    $"        Does not override [Ignore] attributes.",
                    $"",
                    $"    --method <namespace>.<class>.<method>",
                    $"    --method <method>",
                    $"        Run the specified test method.",
                    $"",
                    $"        If <namespace> and <class> are omitted, run all test methods with",
                    $"        the specified name (constrained by --class).",
                    $"",
                    $"        If not specified, run all test methods (constrained by --class).",
                    $"",
                    $"        Can be specified multiple times.",
                    $"",
                    $"        Case-sensitive.",
                    $"",
                    $"        Does not override [Ignore] attributes.",
                    $"",
                    $"    --help",
                    $"        Show usage information",
                    $"",
                    $"EXAMPLES",
                    $"",
                    $"    {shellPrefix} {fileName} TestAssembly.dll AnotherTestAssembly.dll",
                    $"",
                    $"    {shellPrefix} {fileName} {examplePath}TestAssembly.dll {examplePath}AnotherTestAssembly.dll",
                    };
        }


        /// <summary>
        /// Decide whether a test class should run given the specified --class options
        /// </summary>
        ///
        static public bool ClassShouldRun(string fullClassName)
        {
            Guard.NotNullOrWhiteSpace(fullClassName, nameof(fullClassName));

            if (!Classes.Any())
            {
                return true;
            }

            if (WasFullClassSpecified(fullClassName))
            {
                return true;
            }

            if (WasClassSpecified(fullClassName))
            {
                return true;
            }

            if (WasFullMethodInClassSpecified(fullClassName))
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// Decide whether a test method should run given the specified --class and --method options
        /// </summary>
        ///
        static public bool MethodShouldRun(string fullMethodName)
        {
            Guard.NotNullOrWhiteSpace(fullMethodName, nameof(fullMethodName));

            if (!Methods.Any())
            {
                return true;
            }

            if (WasFullMethodSpecified(fullMethodName))
            {
                return true;
            }

            if (!WasMethodSpecified(fullMethodName))
            {
                return false;
            }

            var a = fullMethodName.Split('.');
            var fullClassName = string.Join(".", a.Take(a.Length - 1));

            if (!Classes.Any())
            {
                return true;
            }

            if (WasFullClassSpecified(fullClassName))
            {
                return true;
            }

            if (WasClassSpecified(fullClassName))
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// Parse command line arguments
        /// </summary>
        ///
        static public void Parse(string[] args)
        {
            Success = false;
            ErrorMessage = "";
            OutputFormat = OutputFormats.Human;
            InProc = false;
            Help = false;
            Parse(new Queue<string>(args));
        }


        static void Parse(Queue<string> args)
        {
            if (args.Count == 1 && args.Peek() == "--help")
            {
                Help = true;
                Success = true;
                return;
            }

            for (;;)
            {
                if (args.Count == 0) break;
                if (!args.Peek().StartsWith("--", StringComparison.Ordinal)) break;
                var s = args.Dequeue();
                switch (s)
                {
                    case "--outputformat":
                        ParseOutputFormat(args);
                        if (ErrorMessage != "") return;
                        break;

                    case "--class":
                        ParseClass(args);
                        if (ErrorMessage != "") return;
                        break;

                    case "--method":
                        ParseMethod(args);
                        if (ErrorMessage != "") return;
                        break;

                    case "--inproc":
                        InProc = true;
                        break;

                    case "--help":
                        ErrorMessage = $"Unexpected switch {s}";
                        return;

                    default:
                        ErrorMessage = $"Unrecognised switch {s}";
                        return;
                }
            }

            while (args.Count > 0)
            {
                _testFiles.Add(args.Dequeue());
            }

            if (TestFiles.Count == 0)
            {
                ErrorMessage = "No <file>s specified";
                return;
            }

            if (InProc && TestFiles.Count > 1)
            {
                ErrorMessage = "Only one <file> allowed when --inproc";
                return;
            }

            Success = true;
        }


        static void ParseClass(Queue<string> args)
        {
            if (args.Count == 0)
            {
                ErrorMessage = "Expected <class>";
                return;
            }

            _classes.Add(args.Dequeue());
        }


        static void ParseMethod(Queue<string> args)
        {
            if (args.Count == 0)
            {
                ErrorMessage = "Expected <method>";
                return;
            }

            _methods.Add(args.Dequeue());
        }


        static void ParseOutputFormat(Queue<string> args)
        {
            if (args.Count == 0)
            {
                ErrorMessage = "Expected <outputformat>";
                return;
            }

            var s = args.Dequeue();

            switch (s)
            {
                case OutputFormats.Human:
                case OutputFormats.Machine:
                    OutputFormat = s;
                    break;
                default:
                    ErrorMessage = $"Unrecognised <outputformat> {s}";
                    break;
            }
        }


        static bool WasFullClassSpecified(string fullClassName)
        {
            return Classes.Contains(fullClassName);
        }


        static bool WasClassSpecified(string fullClassName)
        {
            var className = fullClassName.Split('.').Last();
            return Classes.Contains(className);
        }


        static bool WasFullMethodInClassSpecified(string fullClassName)
        {
            return Methods.Any(m => m.StartsWith($"{fullClassName}.", StringComparison.Ordinal));
        }


        static bool WasFullMethodSpecified(string fullMethodName)
        {
            return Methods.Contains(fullMethodName);
        }


        static bool WasMethodSpecified(string fullMethodName)
        {
            var methodName = fullMethodName.Split('.').Last();
            return Methods.Contains(methodName);
        }

    }
}
