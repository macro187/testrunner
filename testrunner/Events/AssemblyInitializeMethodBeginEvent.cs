namespace TestRunner.Events
{
    public class AssemblyInitializeMethodBeginEvent : TestRunnerEvent
    {
        public string MethodName { get; set; }
        public string FirstTestClassFullName { get; set; }
        public string FirstTestMethodName { get; set; }
    }
}
