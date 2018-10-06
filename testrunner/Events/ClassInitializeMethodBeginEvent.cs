using TestRunner.Domain;

namespace TestRunner.Events
{
    public class ClassInitializeMethodBeginEvent : TestRunnerEvent
    {
        public TestClass TestClass;
    }
}
