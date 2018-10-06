using System;
using System.Reflection;
using TestRunner.Domain;
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
        static public UnitTestOutcome Run(
            TestMethod testMethod,
            MethodInfo testInitializeMethod,
            MethodInfo testCleanupMethod,
            TestClass testClass)
        {
            EventHandlers.First.Handle(new TestBeginEvent() { Name = testMethod.Name });

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
                    EventHandlers.First.Handle(new TestIgnoredEvent());
                    outcome = UnitTestOutcome.NotRunnable;
                    break;
                }

                //
                // Create instance of [TestClass]
                //
                var instance = Activator.CreateInstance(testClass.Type);

                //
                // Set TestContext property (if present)
                //
                MethodRunner.RunTestContextSetter(testClass.TestContextSetter, instance);

                //
                // Run [TestInitialize] method
                //
                testInitializeSucceeded = MethodRunner.RunTestInitializeMethod(testInitializeMethod, instance);

                if (!testInitializeSucceeded)
                {
                    outcome = UnitTestOutcome.Failed;
                    break;
                }

                //
                // Run [TestMethod]
                //
                testMethodSucceeded =
                    MethodRunner.RunTestMethod(
                        testMethod.MethodInfo, instance,
                        testMethod.ExpectedException,
                        testMethod.AllowDerivedExpectedExceptionTypes);

                //
                // Run [TestCleanup] method
                //
                testCleanupSucceeded =
                    MethodRunner.RunTestCleanupMethod(
                        testCleanupMethod,
                        instance);

                outcome =
                    testMethodSucceeded && testCleanupSucceeded
                        ? UnitTestOutcome.Passed
                        : UnitTestOutcome.Failed;
            }
            while (false);

            EventHandlers.First.Handle(new TestEndEvent() { Outcome = outcome });

            return outcome;
        }
        
    }
}
