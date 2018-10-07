using TestRunner.Domain;

namespace TestRunner.Events
{
    public class MethodUnexpectedExceptionEvent : TestRunnerEvent
    {
        public ExceptionInfo Exception { get; set; }
    }
}
