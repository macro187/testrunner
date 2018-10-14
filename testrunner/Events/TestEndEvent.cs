using TestRunner.MSTest;

namespace TestRunner.Events
{
    public class TestEndEvent : TestRunnerEvent
    {
        public UnitTestOutcome Outcome { get; set; }
    }
}
