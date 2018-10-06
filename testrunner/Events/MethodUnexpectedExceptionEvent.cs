using System;

namespace TestRunner.Events
{
    public class MethodUnexpectedExceptionEvent : TestRunnerEvent
    {
        public Exception Exception { get; set; }
    }
}
