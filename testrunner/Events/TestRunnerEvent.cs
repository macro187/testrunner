using System;
using System.Diagnostics;

namespace TestRunner.Events
{
    public abstract class TestRunnerEvent
    {

        public TestRunnerEvent()
        {
            ProcessId = Process.GetCurrentProcess().Id;
            Timestamp = DateTime.Now;
        }


        public int ProcessId { get; set; }
        public DateTime Timestamp { get; set; }

    }
}
