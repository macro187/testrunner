namespace TestRunner.Events
{
    public class TestAssemblyConfigFileSwitchedEvent : TestRunnerEvent
    {
        public string Path { get; set; }
    }
}
