namespace TestRunner.Events
{
    public class TestAssemblyNotDotNetEvent : TestRunnerEvent
    {
        public string Path { get; set; }
    }
}
