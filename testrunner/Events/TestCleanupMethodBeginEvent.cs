namespace TestRunner.Events
{
    public class TestCleanupMethodBeginEvent : TestRunnerEvent
    {
        public string MethodName { get; set; }
    }
}
