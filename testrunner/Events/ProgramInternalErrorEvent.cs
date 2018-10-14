using TestRunner.Results;

namespace TestRunner.Events
{
    public class ProgramInternalErrorEvent : TestRunnerEvent
    {
        public ExceptionInfo Exception { get; set; }
    }
}
