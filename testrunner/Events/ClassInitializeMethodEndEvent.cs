namespace TestRunner.Events
{
    public class ClassInitializeMethodEndEvent : TestRunnerEvent
    {
        public bool Success { get; set; }
        public long ElapsedMilliseconds { get; set; }
    }
}
