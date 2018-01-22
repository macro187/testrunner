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
    internal class ArgumentParser
    {
        
        public ArgumentParser(string[] args)
        {
            Success = false;
            ErrorMessage = "";
            InProc = false;
            _assemblyPaths = new List<string>();
            Parse(new Queue<string>(args));
        }


        /// <summary>
        /// User-facing command line usage information
        /// </summary>
        public static string Usage
        {
            get
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
                    StringExtensions.FormatInvariant(
                        string.Join(
                            Environment.NewLine,
                            new[] {
                                "SYNOPSIS",
                                "",
                                "    {0} <assemblypath> [<assemblypath> [...]]",
                                "",
                                "OPTIONS",
                                "",
                                "    <assemblypath>",
                                "        Path to a file that may be an assembly containing MSTest unit tests",
                                "",
                                "EXAMPLES",
                                "",
                                "    {1} {0} TestAssembly.dll AnotherTestAssembly.dll",
                                "",
                                "    {1} {0} {2}TestAssembly.dll {2}AnotherTestAssembly.dll",
                                }),
                        fileName,
                        shellPrefix,
                        examplePath);
            }
        }


        void Parse(Queue<string> args)
        {
            for (;;)
            {
                if (args.Count == 0) break;
                if (!args.Peek().StartsWith("--", StringComparison.Ordinal)) break;
                var s = args.Dequeue();
                switch (s)
                {
                    case "--inproc":
                        InProc = true;
                        break;
                    default:
                        ErrorMessage = "Unrecognised switch " + s;
                        return;
                }
            }

            while (args.Count > 0)
            {
                _assemblyPaths.Add(args.Dequeue());
            }

            if (AssemblyPaths.Count == 0)
            {
                ErrorMessage = "No <assemblypath>s specified";
                return;
            }

            if (InProc && AssemblyPaths.Count > 1)
            {
                ErrorMessage = "Only one <assemblypath> allowed when --inproc";
                return;
            }

            Success = true;
        }


        /// <summary>
        /// Were the command line arguments valid?
        /// </summary>
        public bool Success
        {
            get;
            private set;
        }


        /// <summary>
        /// User-facing error message if not <see cref="Success"/>
        /// </summary>
        public string ErrorMessage
        {
            get;
            private set;
        }


        public bool InProc
        {
            get;
            private set;
        }


        /// <summary>
        /// Path(s) to test assemblies listed on the command line
        /// </summary>
        public IList<string> AssemblyPaths
        {
            get
            {
                return new ReadOnlyCollection<string>(_assemblyPaths);
            }
        }

        List<string> _assemblyPaths;

    }
}
