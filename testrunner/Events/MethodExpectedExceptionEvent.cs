using TestRunner.Domain;

namespace TestRunner.Events
{
    public class MethodExpectedExceptionEvent : TestRunnerEvent
    {
        public string ExpectedFullName { get; set; }
        public ExceptionInfo Exception { get; set; }
    }
}
