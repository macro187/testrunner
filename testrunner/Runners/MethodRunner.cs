using System;
using System.Diagnostics;
using System.Reflection;
using TestRunner.Domain;
using TestRunner.Infrastructure;
using static TestRunner.Events.EventHandler;

namespace TestRunner.Runners
{
    static class MethodRunner
    {

        static readonly Stopwatch Stopwatch = new Stopwatch();


        static public bool RunAssemblyInitializeMethod(MethodInfo method)
        {
            if (method == null) return true;
            AssemblyInitializeMethodBeginEvent(method);
            StartStopwatch();
            var result = Run(method, null, true, null, false);
            var elapsed = StopStopwatch();
            AssemblyInitializeMethodEndEvent(result, elapsed);
            return result;
        }


        static public bool RunAssemblyCleanupMethod(MethodInfo method)
        {
            if (method == null) return true;
            AssemblyCleanupMethodBeginEvent(method);
            StartStopwatch();
            var result = Run(method, null, false, null, false);
            var elapsed = StopStopwatch();
            AssemblyCleanupMethodEndEvent(result, elapsed);
            return result;
        }


        static public bool RunClassInitializeMethod(MethodInfo method)
        {
            if (method == null) return true;
            ClassInitializeMethodBeginEvent(method);
            StartStopwatch();
            var result = Run(method, null, true, null, false);
            var elapsed = StopStopwatch();
            ClassInitializeMethodEndEvent(result, elapsed);
            return result;
        }


        static public bool RunClassCleanupMethod(MethodInfo method)
        {
            if (method == null) return true;
            ClassCleanupMethodBeginEvent(method);
            StartStopwatch();
            var result = Run(method, null, false, null, false);
            var elapsed = StopStopwatch();
            ClassCleanupMethodEndEvent(result, elapsed);
            return result;
        }


        static public void RunTestContextSetter(MethodInfo method, object instance)
        {
            if (method == null) return;
            Guard.NotNull(instance, nameof(instance));
            TestContextSetterBeginEvent(method);
            StartStopwatch();
            var result = Run(method, instance, true, null, false);
            var elapsed = StopStopwatch();
            TestContextSetterEndEvent(result, elapsed);
        }


        static public bool RunTestInitializeMethod(MethodInfo method, object instance)
        {
            if (method == null) return true;
            Guard.NotNull(instance, nameof(instance));
            TestInitializeMethodBeginEvent(method);
            StartStopwatch();
            var result = Run(method, instance, false, null, false);
            var elapsed = StopStopwatch();
            TestInitializeMethodEndEvent(result, elapsed);
            return result;
        }


        static public bool RunTestMethod(
            MethodInfo method,
            object instance,
            Type expectedException,
            bool expectedExceptionAllowDerived)
        {
            Guard.NotNull(method, nameof(method));
            Guard.NotNull(instance, nameof(instance));
            TestMethodBeginEvent(method);
            StartStopwatch();
            var result = Run(method, instance, false, expectedException, expectedExceptionAllowDerived);
            var elapsed = StopStopwatch();
            TestMethodEndEvent(result, elapsed);
            return result;
        }


        static public bool RunTestCleanupMethod(MethodInfo method, object instance)
        {
            if (method == null) return true;
            Guard.NotNull(instance, nameof(instance));
            TestCleanupMethodBeginEvent(method);
            StartStopwatch();
            var result = Run(method, instance, false, null, false);
            var elapsed = StopStopwatch();
            TestCleanupMethodEndEvent(result, elapsed);
            return result;
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
                    MethodExpectedExceptionEvent(expectedException, e);
                    return true;
                }

                MethodUnexpectedExceptionEvent(e);
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
