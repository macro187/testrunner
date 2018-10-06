using System;

namespace TestRunner.Events
{
    public class MethodExpectedExceptionEvent : TestRunnerEvent
    {
        public Type Expected { get; set; }
        public Exception Exception { get; set; }
    }
}
