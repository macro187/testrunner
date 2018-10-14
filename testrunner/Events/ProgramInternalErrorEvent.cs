using TestRunner.MSTest;

namespace TestRunner.Events
{
    public class ProgramInternalErrorEvent : TestRunnerEvent
    {
        public ExceptionInfo Exception { get; set; }
    }
}
