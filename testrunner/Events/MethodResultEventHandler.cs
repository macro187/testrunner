using System;
using System.Diagnostics;
using TestRunner.Results;

namespace TestRunner.Events
{

    /// <summary>
    /// Event handler that accumulates method execution information and populates end event results with it
    /// </summary>
    ///
    public class MethodResultEventHandler : EventHandler
    {

        readonly Stopwatch Stopwatch = new Stopwatch();
        bool inMethod;
        ExceptionInfo exception;
        bool exceptionWasExpected;


        protected override void Handle(AssemblyInitializeMethodBeginEvent e) { HandleBegin(); }
        protected override void Handle(AssemblyInitializeMethodEndEvent e) { HandleEnd(e); }
        protected override void Handle(AssemblyCleanupMethodBeginEvent e) { HandleBegin(); }
        protected override void Handle(AssemblyCleanupMethodEndEvent e) { HandleEnd(e); }
        protected override void Handle(ClassInitializeMethodBeginEvent e) { HandleBegin(); }
        protected override void Handle(ClassInitializeMethodEndEvent e) { HandleEnd(e); }
        protected override void Handle(ClassCleanupMethodBeginEvent e) { HandleBegin(); }
        protected override void Handle(ClassCleanupMethodEndEvent e) { HandleEnd(e); }
        protected override void Handle(TestContextSetterBeginEvent e) { HandleBegin(); }
        protected override void Handle(TestContextSetterEndEvent e) { HandleEnd(e); }
        protected override void Handle(TestInitializeMethodBeginEvent e) { HandleBegin(); }
        protected override void Handle(TestInitializeMethodEndEvent e) { HandleEnd(e); }
        protected override void Handle(TestMethodBeginEvent e) { HandleBegin(); }
        protected override void Handle(TestMethodEndEvent e) { HandleEnd(e); }
        protected override void Handle(TestCleanupMethodBeginEvent e) { HandleBegin(); }
        protected override void Handle(TestCleanupMethodEndEvent e) { HandleEnd(e); }


        protected override void Handle(MethodExpectedExceptionEvent e)
        {
            exception = e.Exception;
            exceptionWasExpected = true;
        }


        protected override void Handle(MethodUnexpectedExceptionEvent e)
        {
            exception = e.Exception;
            exceptionWasExpected = false;
        }


        void HandleBegin()
        {
            if (inMethod) throw new InvalidOperationException("Method began before previous one ended");
            inMethod = true;
            exception = null;
            exceptionWasExpected = false;
            StartStopwatch();
        }


        void HandleEnd(MethodEndEvent e)
        {
            if (!inMethod) throw new InvalidOperationException("Method ended before it started");
            e.Result.ElapsedMilliseconds = StopStopwatch();
            e.Result.Exception = exception;
            e.Result.Success = exception == null || exceptionWasExpected;
            inMethod = false;
        }


        void StartStopwatch()
        {
            Stopwatch.Reset();
            Stopwatch.Start();
        }


        long StopStopwatch()
        {
            Stopwatch.Stop();
            return Stopwatch.ElapsedMilliseconds;
        }

    }
}
