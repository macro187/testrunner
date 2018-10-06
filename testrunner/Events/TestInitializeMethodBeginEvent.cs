using System.Reflection;

namespace TestRunner.Events
{
    public class TestInitializeMethodBeginEvent : TestRunnerEvent
    {
        public MethodInfo Method { get; set; }
    }
}
