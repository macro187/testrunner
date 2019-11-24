using TestRunner.Results;

namespace TestRunner.Events
{
    public class TestClassEndEvent : TestRunnerEvent
    {
        public TestClassResult Result { get; set; } = new TestClassResult();
    }
}
