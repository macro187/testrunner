namespace TestRunner.Events
{
    public class AssemblyCleanupMethodEndEvent : TestRunnerEvent
    {
        public bool Success { get; set; }
        public long ElapsedMilliseconds { get; set; }
    }
}
