namespace TestRunner.Events
{
    public class TestInitializeMethodBeginEvent : TestRunnerEvent
    {
        public string MethodName { get; set; }
    }
}
