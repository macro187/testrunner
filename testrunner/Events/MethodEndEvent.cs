using TestRunner.Results;

namespace TestRunner.Events
{
    public abstract class MethodEndEvent : TestRunnerEvent
    {
        public MethodResult Result { get; set; } = new MethodResult();
    }
}
