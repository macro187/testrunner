using System;
using System.Reflection;
using TestRunner.Domain;
using TestRunner.Infrastructure;
using EventHandler = TestRunner.Events.EventHandler;

namespace TestRunner.Runners
{
    static class MethodRunner
    {

        static public bool RunAssemblyInitializeMethod(TestAssembly testAssembly)
        {
            Guard.NotNull(testAssembly, nameof(testAssembly));
            var method = testAssembly.AssemblyInitializeMethod;
            if (method == null) return true;
            EventHandler.First.AssemblyInitializeMethodBeginEvent(testAssembly);
            var success = Run(method, null, true, null, false);
            EventHandler.First.AssemblyInitializeMethodEndEvent(success, -1);
            return success;
        }


        static public bool RunAssemblyCleanupMethod(MethodInfo method)
        {
            if (method == null) return true;
            EventHandler.First.AssemblyCleanupMethodBeginEvent(method);
            var success = Run(method, null, false, null, false);
            EventHandler.First.AssemblyCleanupMethodEndEvent(success, -1);
            return success;
        }


        static public bool RunClassInitializeMethod(TestClass testClass)
        {
            Guard.NotNull(testClass, nameof(testClass));
            var method = testClass.ClassInitializeMethod;
            if (method == null) return true;
            EventHandler.First.ClassInitializeMethodBeginEvent(testClass);
            var success = Run(method, null, true, null, false);
            EventHandler.First.ClassInitializeMethodEndEvent(success, -1);
            return success;
        }


        static public bool RunClassCleanupMethod(MethodInfo method)
        {
            if (method == null) return true;
            EventHandler.First.ClassCleanupMethodBeginEvent(method);
            var success = Run(method, null, false, null, false);
            EventHandler.First.ClassCleanupMethodEndEvent(success, -1);
            return success;
        }


        static public void RunTestContextSetter(MethodInfo method, object instance)
        {
            if (method == null) return;
            Guard.NotNull(instance, nameof(instance));
            EventHandler.First.TestContextSetterBeginEvent(method);
            var success = Run(method, instance, true, null, false);
            EventHandler.First.TestContextSetterEndEvent(success, -1);
        }


        static public bool RunTestInitializeMethod(MethodInfo method, object instance)
        {
            if (method == null) return true;
            Guard.NotNull(instance, nameof(instance));
            EventHandler.First.TestInitializeMethodBeginEvent(method);
            var success = Run(method, instance, false, null, false);
            EventHandler.First.TestInitializeMethodEndEvent(success, -1);
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
            var success = Run(method, instance, false, expectedException, expectedExceptionAllowDerived);
            EventHandler.First.TestMethodEndEvent(success, -1);
            return success;
        }


        static public bool RunTestCleanupMethod(MethodInfo method, object instance)
        {
            if (method == null) return true;
            Guard.NotNull(instance, nameof(instance));
            EventHandler.First.TestCleanupMethodBeginEvent(method);
            var success = Run(method, instance, false, null, false);
            EventHandler.First.TestCleanupMethodEndEvent(success, -1);
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
        
    }
}
