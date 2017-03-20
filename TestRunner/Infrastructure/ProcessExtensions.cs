using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace TestRunner.Infrastructure
{
    public static class ProcessExtensions
    {

        public static ProcessExecuteResults Execute(string fileName, string arguments)
        {
            Guard.NotNull(fileName, "fileName");
            Guard.NotNull(arguments, "arguments");

            bool echoCommandLine = false;
            bool echoOutput = true;

            var standardOutput = new StringBuilder();
            var errorOutput = new StringBuilder();
            var output = new StringBuilder();
            int exitCode;

            using (var proc = new Process())
            {
                bool exited = false;

                proc.StartInfo.FileName = fileName;
                proc.StartInfo.Arguments = arguments;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.OutputDataReceived += (s,e) => {
                    if (echoOutput)
                    {
                        Console.Out.WriteLine(e.Data ?? "");
                    }
                    standardOutput.AppendLine(e.Data ?? "");
                    output.AppendLine(e.Data ?? "");
                };
                proc.ErrorDataReceived += (s,e) => {
                    if (echoOutput)
                    {
                        Console.Error.WriteLine(e.Data ?? "");
                    }
                    errorOutput.AppendLine(e.Data ?? "");
                    output.AppendLine(e.Data ?? "");
                };
                proc.EnableRaisingEvents = true;
                proc.Exited += (s,e) => exited = true;

                if (echoCommandLine)
                {
                    Console.Out.WriteLine("{0} {1}", fileName, arguments);
                }
                proc.Start();
                proc.BeginOutputReadLine();
                while (!exited) Thread.Yield();

                exitCode = proc.ExitCode;
            }

            return new ProcessExecuteResults(
                standardOutput.ToString(),
                errorOutput.ToString(),
                output.ToString(),
                exitCode);
        }

    }


    public class ProcessExecuteResults
    {

        public ProcessExecuteResults(string standardOutput, string errorOutput, string output, int exitCode)
        {
            Guard.NotNull(standardOutput, "standardOutput");
            Guard.NotNull(errorOutput, "errorOutput");
            Guard.NotNull(output, "output");

            StandardOutput = standardOutput;
            ErrorOutput = errorOutput;
            Output = output;
            ExitCode = exitCode;
        }

        public string StandardOutput { get; private set; }
        public string ErrorOutput { get; private set; }
        public string Output { get; private set; }
        public int ExitCode { get; private set; }

    }

}
