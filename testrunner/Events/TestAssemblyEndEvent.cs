namespace TestRunner.Events
{
    public class TestAssemblyEndEvent : TestRunnerEvent
    {
        public bool Success { get; set; }
    }
}
