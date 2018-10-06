using System.Reflection;

namespace TestRunner.Events
{
    public class TestCleanupMethodBeginEvent : TestRunnerEvent
    {
        public MethodInfo Method { get; set; }
    }
}
