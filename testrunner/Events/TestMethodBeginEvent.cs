using System.Reflection;

namespace TestRunner.Events
{
    public class TestMethodBeginEvent : TestRunnerEvent
    {
        public MethodInfo Method { get; set; }
    }
}
