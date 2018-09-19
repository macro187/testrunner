using System;
using System.Reflection;
using TestRunner.Domain;
using static TestRunner.Events.EventHandler;

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
                TestMethodBeginEvent(testMethod.Name);

                if (testMethod.IsIgnored)
                {
                    TestMethodIgnoredEvent();
                    return UnitTestOutcome.NotRunnable;
                }

                //
                // Construct an instance of the test class
                //
                var testInstance = Activator.CreateInstance(testClass.Type);

                //
                // Set the instance's TestContext property, if present
                //
                if (testClass.TestContextSetter != null)
                {
                    MethodRunner.Run(
                        testClass.TestContextSetter, testInstance,
                        true,
                        null, false,
                        null);
                }

                //
                // Invoke [TestInitialize], [TestMethod], and [TestCleanup]
                //
                bool testInitializeSucceeded = false;
                bool testMethodSucceeded = false;
                bool testCleanupSucceeded = false;

                TestContext.CurrentTestOutcome = UnitTestOutcome.InProgress;

                testInitializeSucceeded =
                    MethodRunner.Run(
                        testInitializeMethod, testInstance,
                        false,
                        null, false,
                        "[TestInitialize]");

                if (testInitializeSucceeded)
                {
                    testMethodSucceeded =
                        MethodRunner.Run(
                            testMethod.MethodInfo, testInstance,
                            false,
                            testMethod.ExpectedException, testMethod.AllowDerivedExpectedExceptionTypes,
                            "[TestMethod]");

                    TestContext.CurrentTestOutcome =
                        testMethodSucceeded
                            ? UnitTestOutcome.Passed
                            : UnitTestOutcome.Failed;

                    testCleanupSucceeded =
                        MethodRunner.Run(
                            testCleanupMethod, testInstance,
                            false,
                            null, false,
                            "[TestCleanup]");
                }

                bool passed = testInitializeSucceeded && testMethodSucceeded && testCleanupSucceeded;

                TestMethodEndEvent(passed);

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
