using TestRunner.Infrastructure;

namespace TestRunner.Events
{
    public class ProgramUserErrorEvent : TestRunnerEvent
    {
        public UserException Exception { get; set; }
    }
}
