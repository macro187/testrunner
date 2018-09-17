using System;
using System.Diagnostics;
using System.Reflection;
using TestRunner.Domain;
using TestRunner.Infrastructure;
using static TestRunner.Infrastructure.ConsoleExtensions;

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

            WriteLine();
            WriteLine(prefix + (string.IsNullOrEmpty(prefix) ? "" : " ") + method.Name + "()");

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
                    WriteLine("  [ExpectedException] {0} occurred:", expectedException.FullName);
                }
                WriteLine(StringExtensions.Indent(ExceptionExtensions.FormatException(ex)));
            }

            WriteLine("  {0} ({1:N0} ms)", success ? "Succeeded" : "Failed", watch.ElapsedMilliseconds);

            return success;
        }
        
    }
}
