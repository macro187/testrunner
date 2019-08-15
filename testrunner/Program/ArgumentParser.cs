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
                    $"        Run the specified test class or, if <namespace> is omitted, all",
                    $"        test classes with the specified name.",
                    $"",
                    $"        Can be specified multiple times.",
                    $"",
                    $"        If not specified, all test classes are run.",
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
        static public bool ClassShouldRun(string fullTestClassName)
        {
            Guard.NotNullOrWhiteSpace(fullTestClassName, nameof(fullTestClassName));

            if (!Classes.Any())
            {
                return true;
            }

            if (Classes.Contains(fullTestClassName))
            {
                return true;
            }

            var testClassName = fullTestClassName.Split('.').Last();
            if (Classes.Contains(testClassName))
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

    }
}
