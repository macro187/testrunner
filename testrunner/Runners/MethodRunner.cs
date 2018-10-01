using System;
using System.Diagnostics;
using System.Reflection;
using TestRunner.Domain;
using TestRunner.Infrastructure;
using EventHandler = TestRunner.Events.EventHandler;

namespace TestRunner.Runners
{
    static class MethodRunner
    {

        static readonly Stopwatch Stopwatch = new Stopwatch();


        static public bool RunAssemblyInitializeMethod(MethodInfo method)
        {
            if (method == null) return true;
            EventHandler.First.AssemblyInitializeMethodBeginEvent(method);
            StartStopwatch();
            var success = Run(method, null, true, null, false);
            var elapsed = StopStopwatch();
            EventHandler.First.AssemblyInitializeMethodEndEvent(success, elapsed);
            return success;
        }


        static public bool RunAssemblyCleanupMethod(MethodInfo method)
        {
            if (method == null) return true;
            EventHandler.First.AssemblyCleanupMethodBeginEvent(method);
            StartStopwatch();
            var success = Run(method, null, false, null, false);
            var elapsed = StopStopwatch();
            EventHandler.First.AssemblyCleanupMethodEndEvent(success, elapsed);
            return success;
        }


        static public bool RunClassInitializeMethod(MethodInfo method)
        {
            if (method == null) return true;
            EventHandler.First.ClassInitializeMethodBeginEvent(method);
            StartStopwatch();
            var success = Run(method, null, true, null, false);
            var elapsed = StopStopwatch();
            EventHandler.First.ClassInitializeMethodEndEvent(success, elapsed);
            return success;
        }


        static public bool RunClassCleanupMethod(MethodInfo method)
        {
            if (method == null) return true;
            EventHandler.First.ClassCleanupMethodBeginEvent(method);
            StartStopwatch();
            var success = Run(method, null, false, null, false);
            var elapsed = StopStopwatch();
            EventHandler.First.ClassCleanupMethodEndEvent(success, elapsed);
            return success;
        }


        static public void RunTestContextSetter(MethodInfo method, object instance)
        {
            if (method == null) return;
            Guard.NotNull(instance, nameof(instance));
            EventHandler.First.TestContextSetterBeginEvent(method);
            StartStopwatch();
            var success = Run(method, instance, true, null, false);
            var elapsed = StopStopwatch();
            EventHandler.First.TestContextSetterEndEvent(success, elapsed);
        }


        static public bool RunTestInitializeMethod(MethodInfo method, object instance)
        {
            if (method == null) return true;
            Guard.NotNull(instance, nameof(instance));
            EventHandler.First.TestInitializeMethodBeginEvent(method);
            StartStopwatch();
            var success = Run(method, instance, false, null, false);
            var elapsed = StopStopwatch();
            EventHandler.First.TestInitializeMethodEndEvent(success, elapsed);
            return success;
        }


        static public bool RunTestMethod(
            MethodInfo method,
            object instance,
            Type expectedException,
            bool expectedExceptionAllowDerived)
        {
            Guard.NotNull(method, nameof(method));
            Guard.NotNull(instance, nameof(instance));
            EventHandler.First.TestMethodBeginEvent(method);
            StartStopwatch();
            var success = Run(method, instance, false, expectedException, expectedExceptionAllowDerived);
            var elapsed = StopStopwatch();
            EventHandler.First.TestMethodEndEvent(success, elapsed);
            return success;
        }


        static public bool RunTestCleanupMethod(MethodInfo method, object instance)
        {
            if (method == null) return true;
            Guard.NotNull(instance, nameof(instance));
            EventHandler.First.TestCleanupMethodBeginEvent(method);
            StartStopwatch();
            var success = Run(method, instance, false, null, false);
            var elapsed = StopStopwatch();
            EventHandler.First.TestCleanupMethodEndEvent(success, elapsed);
            return success;
        }


        static bool Run(
            MethodInfo method,
            object instance,
            bool takesTestContext,
            Type expectedException,
            bool expectedExceptionAllowDerived)
        {
            Guard.NotNull(method, nameof(method));

            var parameters = takesTestContext ? new object[] { TestContextProxy.Proxy } : null;

            try
            {
                method.Invoke(instance, parameters);
                return true;
            }

            catch (TargetInvocationException tie)
            {
                var e = tie.InnerException;

                var isExactExpectedException =
                    expectedException != null &&
                    e.GetType() == expectedException;

                var isDerivedExpectedException =
                    expectedException != null &&
                    expectedExceptionAllowDerived &&
                    e.GetType().IsSubclassOf(expectedException);

                bool wasExpected =
                    isExactExpectedException ||
                    isDerivedExpectedException;

                if (wasExpected)
                {
                    EventHandler.First.MethodExpectedExceptionEvent(expectedException, e);
                    return true;
                }

                EventHandler.First.MethodUnexpectedExceptionEvent(e);
                return false;
            }
        }


        static void StartStopwatch()
        {
            if (Stopwatch.IsRunning) throw new InvalidOperationException("Stopwatch already running");
            Stopwatch.Reset();
            Stopwatch.Start();
        }


        static long StopStopwatch()
        {
            if (!Stopwatch.IsRunning) throw new InvalidOperationException("Stopwatch isn't running");
            Stopwatch.Stop();
            return Stopwatch.ElapsedMilliseconds;
        }
        
    }
}
