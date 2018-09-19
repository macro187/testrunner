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

        /// <summary>
        /// Run a method from a test class using reflection
        /// </summary>
        ///
        /// <returns>
        /// Whether the method ran successfully
        /// </returns>
        ///
        static public bool Run(
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
