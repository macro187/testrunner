using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace TestRunner.Infrastructure
{
    public static class ProcessExtensions
    {

        static ProcessExtensions()
        {
            var driverPath = Process.GetCurrentProcess().MainModule.FileName;
            var driverName = Path.GetFileNameWithoutExtension(driverPath).ToLowerInvariant();
            switch (driverName)
            {
                case "dotnet":
                case "mono":
                    DotnetDriver = driverPath;
                    break;
            }
        }


        /// <summary>
        /// Path to the .NET "driver" program (dotnet or mono) running the process, or <c>null</c> if none
        /// </summary>
        ///
        static string DotnetDriver { get; }


        public static ProcessExecuteResults ExecuteDotnet(string fileName, string arguments)
        {
            if (DotnetDriver != null)
            {
                arguments = fileName + " " + arguments;
                fileName = DotnetDriver;
            }
            return Execute(fileName, arguments);
        }


        public static int ExecuteDotnet(
            string fileName,
            string arguments,
            Action<Process, string> onStandardOutput,
            Action<Process, string> onErrorOutput)
        {
            if (DotnetDriver != null)
            {
                arguments = fileName + " " + arguments;
                fileName = DotnetDriver;
            }
            return Execute(fileName, arguments, onStandardOutput, onErrorOutput);
        }


        static ProcessExecuteResults Execute(string fileName, string arguments)
        {
            var standardOutput = new StringBuilder();
            var errorOutput = new StringBuilder();
            var output = new StringBuilder();
            var exitCode = Execute(
                fileName,
                arguments,
                (_, line) => {
                    Console.Out.WriteLine(line);
                    standardOutput.AppendLine(line);
                    output.AppendLine(line);
                },
                (_, line) => {
                    Console.Error.WriteLine(line);
                    errorOutput.AppendLine(line);
                    output.AppendLine(line);
                });
            return new ProcessExecuteResults(
                standardOutput.ToString(),
                errorOutput.ToString(),
                output.ToString(),
                exitCode);
        }


        static int Execute(
            string fileName,
            string arguments,
            Action<Process, string> onStandardOutput,
            Action<Process, string> onErrorOutput)
        {
            Guard.NotNull(fileName, nameof(fileName));
            Guard.NotNull(arguments, nameof(arguments));
            Guard.NotNull(onStandardOutput, nameof(onStandardOutput));
            Guard.NotNull(onErrorOutput, nameof(onErrorOutput));

            using (var proc = new Process())
            {
                bool exited = false;

                proc.StartInfo.FileName = fileName;
                proc.StartInfo.Arguments = arguments;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.OutputDataReceived += (_,e) => {
                    onStandardOutput(proc, e.Data ?? "");
                };
                proc.ErrorDataReceived += (_,e) => {
                    onErrorOutput(proc, e.Data ?? "");
                };
                proc.EnableRaisingEvents = true;
                proc.Exited += (_,__) => {
                    exited = true;
                };

                proc.Start();
                proc.BeginOutputReadLine();
                proc.BeginErrorReadLine();
                while (!exited) Thread.Yield();
                proc.WaitForExit();

                return proc.ExitCode;
            }
        }

    }
}
