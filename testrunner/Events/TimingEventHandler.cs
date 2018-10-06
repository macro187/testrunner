using System;
using System.Diagnostics;

namespace TestRunner.Events
{

    /// <summary>
    /// Event handler that measures execution times
    /// </summary>
    ///
    public class TimingEventHandler : EventHandler
    {

        readonly Stopwatch Stopwatch = new Stopwatch();


        protected override void Handle(AssemblyInitializeMethodBeginEvent e)
        {
            StartStopwatch();
        }


        protected override void Handle(AssemblyInitializeMethodEndEvent e)
        {
            e.ElapsedMilliseconds = StopStopwatch();
        }


        protected override void Handle(AssemblyCleanupMethodBeginEvent e)
        {
            StartStopwatch();
        }


        protected override void Handle(AssemblyCleanupMethodEndEvent e)
        {
            e.ElapsedMilliseconds = StopStopwatch();
        }


        protected override void Handle(ClassInitializeMethodBeginEvent e)
        {
            StartStopwatch();
        }


        protected override void Handle(ClassInitializeMethodEndEvent e)
        {
            e.ElapsedMilliseconds = StopStopwatch();
        }


        protected override void Handle(ClassCleanupMethodBeginEvent e)
        {
            StartStopwatch();
        }


        protected override void Handle(ClassCleanupMethodEndEvent e)
        {
            e.ElapsedMilliseconds = StopStopwatch();
        }


        protected override void Handle(TestContextSetterBeginEvent e)
        {
            StartStopwatch();
        }


        protected override void Handle(TestContextSetterEndEvent e)
        {
            e.ElapsedMilliseconds = StopStopwatch();
        }


        protected override void Handle(TestInitializeMethodBeginEvent e)
        {
            StartStopwatch();
        }


        protected override void Handle(TestInitializeMethodEndEvent e)
        {
            e.ElapsedMilliseconds = StopStopwatch();
        }


        protected override void Handle(TestMethodBeginEvent e)
        {
            StartStopwatch();
        }


        protected override void Handle(TestMethodEndEvent e)
        {
            e.ElapsedMilliseconds = StopStopwatch();
        }


        protected override void Handle(TestCleanupMethodBeginEvent e)
        {
            StartStopwatch();
        }


        protected override void Handle(TestCleanupMethodEndEvent e)
        {
            e.ElapsedMilliseconds = StopStopwatch();
        }


        void StartStopwatch()
        {
            if (Stopwatch.IsRunning) throw new InvalidOperationException("Stopwatch already running");
            Stopwatch.Reset();
            Stopwatch.Start();
        }


        long StopStopwatch()
        {
            if (!Stopwatch.IsRunning) throw new InvalidOperationException("Stopwatch isn't running");
            Stopwatch.Stop();
            return Stopwatch.ElapsedMilliseconds;
        }

    }
}
