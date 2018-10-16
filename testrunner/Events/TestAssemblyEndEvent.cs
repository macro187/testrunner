using TestRunner.Results;

namespace TestRunner.Events
{
    public class TestAssemblyEndEvent : TestRunnerEvent
    {
        public TestAssemblyResult Result { get; set; } = new TestAssemblyResult();
    }
}
