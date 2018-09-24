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

        static public bool RunAssemblyInitializeMethod(MethodInfo method)
        {
            if (method == null) return true;
            AssemblyInitializeMethodBeginEvent(method);
            var result = Run(method, null, true, null, false);
            AssemblyInitializeMethodEndEvent(result);
            return result;
        }


        static public bool RunAssemblyCleanupMethod(MethodInfo method)
        {
            if (method == null) return true;
            AssemblyCleanupMethodBeginEvent(method);
            var result = Run(method, null, false, null, false);
            AssemblyCleanupMethodEndEvent(result);
            return result;
        }


        static public bool RunClassInitializeMethod(MethodInfo method)
        {
            if (method == null) return true;
            ClassInitializeMethodBeginEvent(method);
            var result = Run(method, null, true, null, false);
            ClassInitializeMethodEndEvent(result);
            return result;
        }


        static public bool RunClassCleanupMethod(MethodInfo method)
        {
            if (method == null) return true;
            ClassCleanupMethodBeginEvent(method);
            var result = Run(method, null, false, null, false);
            ClassCleanupMethodEndEvent(result);
            return result;
        }


        static public void RunTestContextSetter(MethodInfo method, object instance)
        {
            if (method == null) return;
            Guard.NotNull(instance, nameof(instance));
            TestContextSetterBeginEvent(method);
            var result = Run(method, instance, true, null, false);
            TestContextSetterEndEvent(result);
        }


        static public bool RunTestInitializeMethod(MethodInfo method, object instance)
        {
            if (method == null) return true;
            Guard.NotNull(instance, nameof(instance));
            TestInitializeMethodBeginEvent(method);
            var result = Run(method, instance, false, null, false);
            TestInitializeMethodEndEvent(result);
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
            var result = Run(method, instance, false, expectedException, expectedExceptionAllowDerived);
            TestMethodEndEvent(result);
            return result;
        }


        static public bool RunTestCleanupMethod(MethodInfo method, object instance)
        {
            if (method == null) return true;
            Guard.NotNull(instance, nameof(instance));
            TestCleanupMethodBeginEvent(method);
            var result = Run(method, instance, false, null, false);
            TestCleanupMethodEndEvent(result);
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

            var watch = new Stopwatch();
            watch.Start();
            bool success = false;
            var parameters = takesTestContext ? new object[] { TestContextProxy.Proxy } : null;
            try
            {
                method.Invoke(instance, parameters);
                watch.Stop();
                success = true;
            }
            catch (TargetInvocationException tie)
            {
                watch.Stop();
                var ex = tie.InnerException;
                bool expected = 
                    expectedException != null &&
                    (
                        ex.GetType() == expectedException ||
                        (expectedExceptionAllowDerived && ex.GetType().IsSubclassOf(expectedException))
                    );
                    
                if (expected)
                {
                    MethodExpectedExceptionEvent(expectedException, ex);
                    success = true;
                }
                else
                {
                    MethodUnexpectedExceptionEvent(ex);
                }
            }

            MethodTimingEvent(watch.ElapsedMilliseconds);

            return success;
        }
        
    }
}
