using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TestRunner
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
            AssemblyPath = "";
            Parse(args);
        }


        /// <summary>
        /// User-facing command line usage information
        /// </summary>
        public string Usage
        {
            get
            {
                var fileName = Path.GetFileName(Assembly.GetExecutingAssembly().Location);
                var programName = Path.GetFileNameWithoutExtension(fileName);
                bool isUnix = new[] { PlatformID.Unix, PlatformID.MacOSX }.Contains(Environment.OSVersion.Platform);
                var shellPrefix =
                    isUnix
                        ? "$"
                        : "C:\\>";
                var examplePath =
                    isUnix
                        ? "/path/to/"
                        : "C:\\path\\to\\";

                return string.Join(
                    Environment.NewLine,
                    new[] {
                        $"SYNOPSIS",
                        $"",
                        $"    {fileName} <assemblypath>",
                        $"",
                        $"OPTIONS",
                        $"",
                        $"    <assemblypath>",
                        $"        Path to an assembly containing MSTest unit tests",
                        $"",
                        $"EXAMPLES",
                        $"",
                        $"    {shellPrefix} {fileName} MyTestAssembly.dll",
                        $"",
                        $"    {shellPrefix} {fileName} {examplePath}MyTestAssembly.dll"
                        });
            }
        }


        void Parse(string[] args)
        {
            if (args.Length != 1)
            {
                ErrorMessage = "No <assemblypath> specified";
                return;
            }
            AssemblyPath = args[0];
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


        /// <summary>
        /// The value of the &lt;assemblypath&gt; argument
        /// </summary>
        public string AssemblyPath
        {
            get;
            private set;
        }

    }
}
