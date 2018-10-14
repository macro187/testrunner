using System;
using System.Linq;
using System.Reflection;
using TestRunner.MSTest;
using TestRunner.Events;
using TestRunner.Infrastructure;
using TestRunner.Results;

namespace TestRunner.Runners
{
    static class MethodRunner
    {

        static public bool RunAssemblyInitializeMethod(TestAssembly testAssembly)
        {
            Guard.NotNull(testAssembly, nameof(testAssembly));
            var method = testAssembly.AssemblyInitializeMethod;
            if (method == null) return true;
            EventHandlers.Raise(
                new AssemblyInitializeMethodBeginEvent() {
                    MethodName = method.Name,
                    FirstTestClassFullName = testAssembly.TestClasses.First().FullName,
                    FirstTestMethodName = testAssembly.TestClasses.First().TestMethods.First().Name,
                });
            var success = Run(method, null, true, null, false);
            EventHandlers.Raise(new AssemblyInitializeMethodEndEvent() { Success = success });
            return success;
        }


        static public bool RunAssemblyCleanupMethod(MethodInfo method)
        {
            if (method == null) return true;
            EventHandlers.Raise(new AssemblyCleanupMethodBeginEvent() { MethodName = method.Name });
            var success = Run(method, null, false, null, false);
            EventHandlers.Raise(new AssemblyCleanupMethodEndEvent() { Success = success });
            return success;
        }


        static public bool RunClassInitializeMethod(TestClass testClass)
        {
            Guard.NotNull(testClass, nameof(testClass));
            var method = testClass.ClassInitializeMethod;
            if (method == null) return true;
            EventHandlers.Raise(
                new ClassInitializeMethodBeginEvent() {
                    MethodName = method.Name,
                    FirstTestMethodName = testClass.TestMethods.First().Name,
                });
            var success = Run(method, null, true, null, false);
            EventHandlers.Raise(new ClassInitializeMethodEndEvent() { Success = success });
            return success;
        }


        static public bool RunClassCleanupMethod(MethodInfo method)
        {
            if (method == null) return true;
            EventHandlers.Raise(new ClassCleanupMethodBeginEvent() { MethodName = method.Name });
            var success = Run(method, null, false, null, false);
            EventHandlers.Raise(new ClassCleanupMethodEndEvent() { Success = success });
            return success;
        }


        static public void RunTestContextSetter(MethodInfo method, object instance)
        {
            if (method == null) return;
            Guard.NotNull(instance, nameof(instance));
            EventHandlers.Raise(new TestContextSetterBeginEvent() { MethodName = method.Name });
            var success = Run(method, instance, true, null, false);
            EventHandlers.Raise(new TestContextSetterEndEvent() { Success = success });
        }


        static public bool RunTestInitializeMethod(MethodInfo method, object instance)
        {
            if (method == null) return true;
            Guard.NotNull(instance, nameof(instance));
            EventHandlers.Raise(new TestInitializeMethodBeginEvent() { MethodName = method.Name });
            var success = Run(method, instance, false, null, false);
            EventHandlers.Raise(new TestInitializeMethodEndEvent() { Success = success });
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
            EventHandlers.Raise(new TestMethodBeginEvent() { MethodName = method.Name });
            var success = Run(method, instance, false, expectedException, expectedExceptionAllowDerived);
            EventHandlers.Raise(new TestMethodEndEvent() { Success = success });
            return success;
        }


        static public bool RunTestCleanupMethod(MethodInfo method, object instance)
        {
            if (method == null) return true;
            Guard.NotNull(instance, nameof(instance));
            EventHandlers.Raise(new TestCleanupMethodBeginEvent() { MethodName = method.Name });
            var success = Run(method, instance, false, null, false);
            EventHandlers.Raise(new TestCleanupMethodEndEvent() { Success = success });
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
                    EventHandlers.Raise(
                        new MethodExpectedExceptionEvent() {
                            ExpectedFullName = expectedException.FullName,
                            Exception = new ExceptionInfo(e),
                        });
                    return true;
                }

                EventHandlers.Raise(new MethodUnexpectedExceptionEvent() { Exception = new ExceptionInfo(e) });
                return false;
            }
        }
        
    }
}
