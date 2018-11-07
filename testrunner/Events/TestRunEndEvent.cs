using TestRunner.Results;

namespace TestRunner.Events
{
    public class TestRunEndEvent : TestRunnerEvent
    {
        public TestRunResult Result { get; set; } = new TestRunResult();
    }
}
