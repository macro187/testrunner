using System;

namespace TestRunner.Events
{
    public class ProgramInternalErrorEvent : TestRunnerEvent
    {
        public Exception Exception { get; set; }
    }
}
