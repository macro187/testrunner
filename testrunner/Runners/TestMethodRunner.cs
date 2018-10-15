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
        /// <returns>
        /// The outcome of the test
        /// </returns>
        ///
        static public UnitTestOutcome Run(TestMethod testMethod)
        {
            EventHandlers.Raise(new TestBeginEvent() { Name = testMethod.Name });

            bool testInitializeSucceeded = false;
            bool testMethodSucceeded = false;
            bool testCleanupSucceeded = false;
            UnitTestOutcome outcome;

            do
            {
                //
                // Handle [Ignored] [TestMethod]
                //
                if (testMethod.IsIgnored)
                {
                    EventHandlers.Raise(new TestIgnoredEvent());
                    outcome = UnitTestOutcome.NotRunnable;
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
                testInitializeSucceeded = MethodRunner.RunTestInitializeMethod(testMethod.TestClass, instance);

                if (!testInitializeSucceeded)
                {
                    outcome = UnitTestOutcome.Failed;
                    break;
                }

                //
                // Run [TestMethod]
                //
                testMethodSucceeded = MethodRunner.RunTestMethod(testMethod, instance);

                //
                // Run [TestCleanup] method
                //
                testCleanupSucceeded = MethodRunner.RunTestCleanupMethod(testMethod.TestClass, instance);

                outcome =
                    testMethodSucceeded && testCleanupSucceeded
                        ? UnitTestOutcome.Passed
                        : UnitTestOutcome.Failed;
            }
            while (false);

            EventHandlers.Raise(new TestEndEvent());

            return outcome;
        }
        
    }
}
