using System.Reflection;

namespace TestRunner.Events
{
    public class ClassCleanupMethodBeginEvent : TestRunnerEvent
    {
        public MethodInfo Method;
    }
}
