using TestRunner.Domain;

namespace TestRunner.Events
{
    public class ProgramInternalErrorEvent : TestRunnerEvent
    {
        public ExceptionInfo Exception { get; set; }
    }
}
