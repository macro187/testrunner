using System;
using TestRunner.MSTest;
using TestRunner.Events;

namespace TestRunner.Runners
{
    static class TestMethodRunner
    {

        /// <summary>
        /// Run a test method (plus its intialize and cleanup methods, if present)
        /// </summary>
        ///
        /// <remarks>
        /// If the test method is decorated with [Ignore], nothing is run
        /// </remarks>
        ///
        static public bool Run(TestMethod testMethod)
        {
            EventHandlers.Raise(new TestBeginEvent() { Name = testMethod.Name });

            var success = false;

            do
            {
                //
                // Handle [Ignored] [TestMethod]
                //
                if (testMethod.IsIgnored)
                {
                    EventHandlers.Raise(new TestIgnoredEvent());
                    success = true;
                    break;
                }

                //
                // Create instance of [TestClass]
                //
                var instance = Activator.CreateInstance(testMethod.TestClass.Type);

                //
                // Set TestContext property (if present)
                //
                MethodRunner.RunTestContextSetter(testMethod.TestClass, instance);

                //
                // Run [TestInitialize] method
                //
                if (!MethodRunner.RunTestInitializeMethod(testMethod.TestClass, instance)) break;

                //
                // Run [TestMethod]
                //
                var testMethodSucceeded = MethodRunner.RunTestMethod(testMethod, instance);

                //
                // Run [TestCleanup] method
                //
                var testCleanupSucceeded = MethodRunner.RunTestCleanupMethod(testMethod.TestClass, instance);

                success = testMethodSucceeded && testCleanupSucceeded;
            }
            while (false);

            EventHandlers.Raise(new TestEndEvent());

            return success;
        }
        
    }
}
