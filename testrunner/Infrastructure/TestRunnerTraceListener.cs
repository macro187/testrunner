using System;
using System.Diagnostics;

namespace TestRunner.Infrastructure
{
    public class TestRunnerTraceListener : TraceListener
    {

        public override void Write(string message)
        {
            Console.Out.Write(message);
        }

        public override void WriteLine(string message)
        {
            Console.Out.WriteLine(message);
        }

    }
}
