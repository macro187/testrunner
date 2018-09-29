using System;
using System.Reflection;
using TestRunner.Domain;
using EventHandler = TestRunner.Events.EventHandler;

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
        /// The results of the test
        /// </returns>
        ///
        static public UnitTestOutcome Run(
            TestMethod testMethod,
            MethodInfo testInitializeMethod,
            MethodInfo testCleanupMethod,
            TestClass testClass)
        {
            TestContext.TestName = testMethod.Name;
            try
            {
                EventHandler.First.TestBeginEvent(testMethod.Name);

                if (testMethod.IsIgnored)
                {
                    EventHandler.First.TestIgnoredEvent();
                    return UnitTestOutcome.NotRunnable;
                }

                //
                // Construct an instance of the test class
                //
                var testInstance = Activator.CreateInstance(testClass.Type);

                //
                // Set the instance's TestContext property, if present
                //
                MethodRunner.RunTestContextSetter(testClass.TestContextSetter, testInstance);

                //
                // Invoke [TestInitialize], [TestMethod], and [TestCleanup]
                //
                bool testInitializeSucceeded = false;
                bool testMethodSucceeded = false;
                bool testCleanupSucceeded = false;

                TestContext.CurrentTestOutcome = UnitTestOutcome.InProgress;

                testInitializeSucceeded = MethodRunner.RunTestInitializeMethod(testInitializeMethod, testInstance);

                if (testInitializeSucceeded)
                {
                    testMethodSucceeded =
                        MethodRunner.RunTestMethod(
                            testMethod.MethodInfo, testInstance,
                            testMethod.ExpectedException,
                            testMethod.AllowDerivedExpectedExceptionTypes);

                    TestContext.CurrentTestOutcome =
                        testMethodSucceeded
                            ? UnitTestOutcome.Passed
                            : UnitTestOutcome.Failed;

                    testCleanupSucceeded = MethodRunner.RunTestCleanupMethod(testCleanupMethod, testInstance);
                }

                bool passed = testInitializeSucceeded && testMethodSucceeded && testCleanupSucceeded;

                EventHandler.First.TestEndEvent(passed);

                return passed ? UnitTestOutcome.Passed : UnitTestOutcome.Failed;
            }
            finally
            {
                TestContext.TestName = null;
                TestContext.CurrentTestOutcome = UnitTestOutcome.Unknown;
            }
        }
        
    }
}
