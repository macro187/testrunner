using System;
using System.Reflection;
using TestRunner.Infrastructure;
using TestRunner.Domain;
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


        public override void ProgramBannerEvent(params string[] lines)
        {
            base.ProgramBannerEvent(lines);
        }


        public override void ProgramUsageEvent(string[] lines)
        {
            base.ProgramUsageEvent(lines);
        }


        public override void ProgramUserErrorEvent(UserException exception)
        {
            base.ProgramUserErrorEvent(exception);
        }


        public override void ProgramInternalErrorEvent(Exception exception)
        {
            base.ProgramInternalErrorEvent(exception);
        }


        public override void TestAssemblyBeginEvent(string path)
        {
            base.TestAssemblyBeginEvent(path);
        }


        public override void TestAssemblyNotFoundEvent(string path)
        {
            base.TestAssemblyNotFoundEvent(path);
        }


        public override void TestAssemblyNotDotNetEvent(string path)
        {
            base.TestAssemblyNotDotNetEvent(path);
        }


        public override void TestAssemblyNotTestEvent(string path)
        {
            base.TestAssemblyNotTestEvent(path);
        }


        public override void TestAssemblyConfigFileSwitchedEvent(string path)
        {
            base.TestAssemblyConfigFileSwitchedEvent(path);
        }


        public override void TestAssemblyEndEvent(bool success)
        {
            base.TestAssemblyEndEvent(success);
        }


        public override void TestClassBeginEvent(string fullName)
        {
            base.TestClassBeginEvent(fullName);
        }


        public override void TestClassEndEvent(
            bool success,
            bool classIgnored,
            bool initializePresent,
            bool initializeSucceeded,
            int testsTotal,
            int testsRan,
            int testsIgnored,
            int testsPassed,
            int testsFailed,
            bool cleanupPresent,
            bool cleanupSucceeded
        )
        {
            base.TestClassEndEvent(
                success,
                classIgnored,
                initializePresent,
                initializeSucceeded,
                testsTotal,
                testsRan,
                testsIgnored,
                testsPassed,
                testsFailed,
                cleanupPresent,
                cleanupSucceeded);
        }


        public override void TestBeginEvent(string name)
        {
            base.TestBeginEvent(name);
        }


        public override void TestIgnoredEvent()
        {
            base.TestIgnoredEvent();
        }


        public override void TestEndEvent(UnitTestOutcome outcome)
        {
            base.TestEndEvent(outcome);
        }


        public override void AssemblyInitializeMethodBeginEvent(TestAssembly testAssembly)
        {
            StartStopwatch();
            base.AssemblyInitializeMethodBeginEvent(testAssembly);
        }


        public override void AssemblyInitializeMethodEndEvent(bool success, long elapsedMilliseconds)
        {
            elapsedMilliseconds = StopStopwatch();
            base.AssemblyInitializeMethodEndEvent(success, elapsedMilliseconds);
        }


        public override void AssemblyCleanupMethodBeginEvent(MethodInfo method)
        {
            StartStopwatch();
            base.AssemblyCleanupMethodBeginEvent(method);
        }


        public override void AssemblyCleanupMethodEndEvent(bool success, long elapsedMilliseconds)
        {
            elapsedMilliseconds = StopStopwatch();
            base.AssemblyCleanupMethodEndEvent(success, elapsedMilliseconds);
        }


        public override void ClassInitializeMethodBeginEvent(TestClass testClass)
        {
            StartStopwatch();
            base.ClassInitializeMethodBeginEvent(testClass);
        }


        public override void ClassInitializeMethodEndEvent(bool success, long elapsedMilliseconds)
        {
            elapsedMilliseconds = StopStopwatch();
            base.ClassInitializeMethodEndEvent(success, elapsedMilliseconds);
        }


        public override void ClassCleanupMethodBeginEvent(MethodInfo method)
        {
            StartStopwatch();
            base.ClassCleanupMethodBeginEvent(method);
        }


        public override void ClassCleanupMethodEndEvent(bool success, long elapsedMilliseconds)
        {
            elapsedMilliseconds = StopStopwatch();
            base.ClassCleanupMethodEndEvent(success, elapsedMilliseconds);
        }


        public override void TestContextSetterBeginEvent(MethodInfo method)
        {
            StartStopwatch();
            base.TestContextSetterBeginEvent(method);
        }


        public override void TestContextSetterEndEvent(bool success, long elapsedMilliseconds)
        {
            elapsedMilliseconds = StopStopwatch();
            base.TestContextSetterEndEvent(success, elapsedMilliseconds);
        }


        public override void TestInitializeMethodBeginEvent(MethodInfo method)
        {
            StartStopwatch();
            base.TestInitializeMethodBeginEvent(method);
        }


        public override void TestInitializeMethodEndEvent(bool success, long elapsedMilliseconds)
        {
            elapsedMilliseconds = StopStopwatch();
            base.TestInitializeMethodEndEvent(success, elapsedMilliseconds);
        }


        public override void TestMethodBeginEvent(MethodInfo method)
        {
            StartStopwatch();
            base.TestMethodBeginEvent(method);
        }


        public override void TestMethodEndEvent(bool success, long elapsedMilliseconds)
        {
            elapsedMilliseconds = StopStopwatch();
            base.TestMethodEndEvent(success, elapsedMilliseconds);
        }


        public override void TestCleanupMethodBeginEvent(MethodInfo method)
        {
            StartStopwatch();
            base.TestCleanupMethodBeginEvent(method);
        }


        public override void TestCleanupMethodEndEvent(bool success, long elapsedMilliseconds)
        {
            elapsedMilliseconds = StopStopwatch();
            base.TestCleanupMethodEndEvent(success, elapsedMilliseconds);
        }


        public override void MethodExpectedExceptionEvent(Type expected, Exception exception)
        {
            base.MethodExpectedExceptionEvent(expected, exception);
        }


        public override void MethodUnexpectedExceptionEvent(Exception exception)
        {
            base.MethodUnexpectedExceptionEvent(exception);
        }


        public override void OutputTraceEvent(string message = "")
        {
            base.OutputTraceEvent(message);
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
