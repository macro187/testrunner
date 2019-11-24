namespace TestRunner.Events
{
    public class ClassInitializeMethodBeginEvent : TestRunnerEvent
    {
        public string MethodName { get; set; }
        public string FirstTestMethodName { get; set; }
    }
}
