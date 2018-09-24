using System;
using System.Diagnostics;
using System.Reflection;
using TestRunner.Domain;
using static TestRunner.Events.EventHandler;

namespace TestRunner.Runners
{
    static class MethodRunner
    {

        static public bool RunAssemblyInitializeMethod(MethodInfo method)
        {
            return Run(
                method, null,
                true,
                null, false,
                "[AssemblyInitialize]");
        }


        static public bool RunAssemblyCleanupMethod(MethodInfo method)
        {
            return Run(
                method, null,
                false,
                null, false,
                "[AssemblyCleanup]");
        }


        static public bool RunClassInitializeMethod(MethodInfo method)
        {
            return Run(
                method, null,
                true,
                null, false,
                "[ClassInitialize]");
        }


        static public bool RunClassCleanupMethod(MethodInfo method)
        {
            return Run(
                method, null,
                false,
                null, false,
                "[ClassCleanup]");
        }


        static public void RunTestContextSetter(
            MethodInfo method,
            object instance)
        {
            if (method == null) return;
            Run(method, instance, true, null, false, null);
        }


        static public bool RunTestInitializeMethod(
            MethodInfo method,
            object instance)
        {
            return Run(
                method, instance,
                false,
                null, false,
                "[TestInitialize]");
        }


        static public bool RunTestMethod(
            MethodInfo method,
            object instance,
            Type expectedException,
            bool expectedExceptionAllowDerived)
        {
            return Run(
                method, instance,
                false,
                expectedException, expectedExceptionAllowDerived,
                "[TestMethod]");
        }


        static public bool RunTestCleanupMethod(
            MethodInfo method,
            object instance)
        {
            return Run(
                method, instance,
                false,
                null, false,
                "[TestCleanup]");
        }


        static bool Run(
            MethodInfo method,
            object instance,
            bool takesTestContext,
            Type expectedException,
            bool expectedExceptionAllowDerived,
            string prefix)
        {
            prefix = prefix ?? "";

            if (method == null) return true;

            MethodBeginEvent(prefix, method.Name);

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
                    success = true;
                    MethodExpectedExceptionEvent(expectedException.FullName);
                }

                MethodExceptionEvent(ex);
            }

            MethodSummaryEvent(success, watch.ElapsedMilliseconds);

            MethodEndEvent();

            return success;
        }
        
    }
}
