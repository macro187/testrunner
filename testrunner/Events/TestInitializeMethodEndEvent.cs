namespace TestRunner.Events
{
    public class TestInitializeMethodEndEvent : TestRunnerEvent
    {
        public bool Success { get; set; }
        public long ElapsedMilliseconds { get; set; }
    }
}
