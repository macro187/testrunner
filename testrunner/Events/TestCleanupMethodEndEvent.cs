namespace TestRunner.Events
{
    public class TestCleanupMethodEndEvent : TestRunnerEvent
    {
        public bool Success { get; set; }
        public long ElapsedMilliseconds { get; set; }
    }
}
