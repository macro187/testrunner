using TestRunner.Domain;

namespace TestRunner.Events
{
    public class TestEndEvent : TestRunnerEvent
    {
        public UnitTestOutcome Outcome { get; set; }
    }
}
