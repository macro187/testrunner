using System.Reflection;

namespace TestRunner.Events
{
    public class TestContextSetterBeginEvent : TestRunnerEvent
    {
        public MethodInfo Method;
    }
}
