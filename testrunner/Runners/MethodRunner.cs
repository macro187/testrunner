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


        static public bool RunAssemblyCleanupMethod(TestAssembly testAssembly)
        {
            Guard.NotNull(testAssembly, nameof(testAssembly));
            var method = testAssembly.AssemblyCleanupMethod;
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


        static public bool RunClassCleanupMethod(TestClass testClass)
        {
            Guard.NotNull(testClass, nameof(testClass));
            var method = testClass.ClassCleanupMethod;
            if (method == null) return true;
            EventHandlers.Raise(new ClassCleanupMethodBeginEvent() { MethodName = method.Name });
            var success = Run(method, null, false, null, false);
            EventHandlers.Raise(new ClassCleanupMethodEndEvent() { Success = success });
            return success;
        }


        static public void RunTestContextSetter(TestClass testClass, object instance)
        {
            Guard.NotNull(testClass, nameof(testClass));
            Guard.NotNull(instance, nameof(instance));
            var method = testClass.TestContextSetter;
            if (method == null) return;
            EventHandlers.Raise(new TestContextSetterBeginEvent() { MethodName = method.Name });
            var success = Run(method, instance, true, null, false);
            EventHandlers.Raise(new TestContextSetterEndEvent() { Success = success });
        }


        static public bool RunTestInitializeMethod(TestClass testClass, object instance)
        {
            Guard.NotNull(testClass, nameof(testClass));
            Guard.NotNull(instance, nameof(instance));
            var method = testClass.TestInitializeMethod;
            if (method == null) return true;
            EventHandlers.Raise(new TestInitializeMethodBeginEvent() { MethodName = method.Name });
            var success = Run(method, instance, false, null, false);
            EventHandlers.Raise(new TestInitializeMethodEndEvent() { Success = success });
            return success;
        }


        static public bool RunTestMethod(TestMethod testMethod, object instance)
        {
            Guard.NotNull(testMethod, nameof(testMethod));
            Guard.NotNull(instance, nameof(instance));
            EventHandlers.Raise(new TestMethodBeginEvent() { MethodName = testMethod.Name });
            var success = Run(
                testMethod.MethodInfo,
                instance,
                false,
                testMethod.ExpectedException,
                testMethod.AllowDerivedExpectedExceptionTypes);
            EventHandlers.Raise(new TestMethodEndEvent() { Success = success });
            return success;
        }


        static public bool RunTestCleanupMethod(TestClass testClass, object instance)
        {
            Guard.NotNull(testClass, nameof(testClass));
            Guard.NotNull(instance, nameof(instance));
            var method = testClass.TestCleanupMethod;
            if (method == null) return true;
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
