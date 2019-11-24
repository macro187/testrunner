namespace TestRunner.Events
{
    public class TestIgnoredEvent : TestRunnerEvent
    {
        public bool IgnoredFromCommandLine { get; set; }
    }
}
