using System;
using System.Reflection;
using TestRunner.Domain;
using TestRunner.Events;
using TestRunner.Infrastructure;

namespace TestRunner.Runners
{
    static class MethodRunner
    {

        static public bool RunAssemblyInitializeMethod(TestAssembly testAssembly)
        {
            Guard.NotNull(testAssembly, nameof(testAssembly));
            var method = testAssembly.AssemblyInitializeMethod;
            if (method == null) return true;
            EventHandlers.First.Handle(new AssemblyInitializeMethodBeginEvent() { TestAssembly = testAssembly });
            var success = Run(method, null, true, null, false);
            EventHandlers.First.Handle(new AssemblyInitializeMethodEndEvent() { Success = success });
            return success;
        }


        static public bool RunAssemblyCleanupMethod(MethodInfo method)
        {
            if (method == null) return true;
            EventHandlers.First.Handle(new AssemblyCleanupMethodBeginEvent() { Method = method });
            var success = Run(method, null, false, null, false);
            EventHandlers.First.Handle(new AssemblyCleanupMethodEndEvent() { Success = success });
            return success;
        }


        static public bool RunClassInitializeMethod(TestClass testClass)
        {
            Guard.NotNull(testClass, nameof(testClass));
            var method = testClass.ClassInitializeMethod;
            if (method == null) return true;
            EventHandlers.First.Handle(new ClassInitializeMethodBeginEvent() { TestClass = testClass });
            var success = Run(method, null, true, null, false);
            EventHandlers.First.Handle(new ClassInitializeMethodEndEvent() { Success = success });
            return success;
        }


        static public bool RunClassCleanupMethod(MethodInfo method)
        {
            if (method == null) return true;
            EventHandlers.First.Handle(new ClassCleanupMethodBeginEvent() { Method = method });
            var success = Run(method, null, false, null, false);
            EventHandlers.First.Handle(new ClassCleanupMethodEndEvent() { Success = success });
            return success;
        }


        static public void RunTestContextSetter(MethodInfo method, object instance)
        {
            if (method == null) return;
            Guard.NotNull(instance, nameof(instance));
            EventHandlers.First.Handle(new TestContextSetterBeginEvent() { Method = method });
            var success = Run(method, instance, true, null, false);
            EventHandlers.First.Handle(new TestContextSetterEndEvent() { Success = success });
        }


        static public bool RunTestInitializeMethod(MethodInfo method, object instance)
        {
            if (method == null) return true;
            Guard.NotNull(instance, nameof(instance));
            EventHandlers.First.Handle(new TestInitializeMethodBeginEvent() { Method = method });
            var success = Run(method, instance, false, null, false);
            EventHandlers.First.Handle(new TestInitializeMethodEndEvent() { Success = success });
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
            EventHandlers.First.Handle(new TestMethodBeginEvent() { Method = method });
            var success = Run(method, instance, false, expectedException, expectedExceptionAllowDerived);
            EventHandlers.First.Handle(new TestMethodEndEvent() { Success = success });
            return success;
        }


        static public bool RunTestCleanupMethod(MethodInfo method, object instance)
        {
            if (method == null) return true;
            Guard.NotNull(instance, nameof(instance));
            EventHandlers.First.Handle(new TestCleanupMethodBeginEvent() { Method = method });
            var success = Run(method, instance, false, null, false);
            EventHandlers.First.Handle(new TestCleanupMethodEndEvent() { Success = success });
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
                    EventHandlers.First.Handle(
                        new MethodExpectedExceptionEvent() {
                            Expected = expectedException,
                            Exception = e,
                        });
                    return true;
                }

                EventHandlers.First.Handle(new MethodUnexpectedExceptionEvent() { Exception = e });
                return false;
            }
        }
        
    }
}
