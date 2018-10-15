using TestRunner.Results;

namespace TestRunner.Events
{
    public class TestEndEvent : TestRunnerEvent
    {
        public TestResult Result { get; set; } = new TestResult();
    }
}
