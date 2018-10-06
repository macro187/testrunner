using TestRunner.Domain;

namespace TestRunner.Events
{
    public class AssemblyInitializeMethodBeginEvent : TestRunnerEvent
    {
        public TestAssembly TestAssembly { get; set; }
    }
}
