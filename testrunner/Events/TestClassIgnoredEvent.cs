namespace TestRunner.Events
{
    public class TestClassIgnoredEvent : TestRunnerEvent
    {
        public bool IgnoredFromCommandLine { get; set; }
    }
}
