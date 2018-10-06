namespace TestRunner.Events
{
    public class AssemblyInitializeMethodEndEvent : TestRunnerEvent
    {
        public bool Success { get; set; }
        public long ElapsedMilliseconds { get; set; }
    }
}
