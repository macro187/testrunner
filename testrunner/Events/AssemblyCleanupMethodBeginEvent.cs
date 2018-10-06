using System.Reflection;

namespace TestRunner.Events
{
    public class AssemblyCleanupMethodBeginEvent : TestRunnerEvent
    {
        public MethodInfo Method;
    }
}
