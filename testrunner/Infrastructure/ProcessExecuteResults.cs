
namespace TestRunner.Infrastructure
{
    public class ProcessExecuteResults
    {

        public ProcessExecuteResults(string standardOutput, string errorOutput, string output, int exitCode)
        {
            Guard.NotNull(standardOutput, nameof(standardOutput));
            Guard.NotNull(errorOutput, nameof(errorOutput));
            Guard.NotNull(output, nameof(output));
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
