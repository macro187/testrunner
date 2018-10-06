namespace TestRunner.Events
{
    public class TestContextSetterEndEvent : TestRunnerEvent
    {
        public bool Success { get; set; }
        public long ElapsedMilliseconds { get; set; }
    }
}
