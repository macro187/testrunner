namespace TestRunner.Events
{
    public class TestAssemblyNotFoundEvent : TestRunnerEvent
    {
        public string Path { get; set; }
    }
}
