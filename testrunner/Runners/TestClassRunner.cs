using System.Linq;
using TestRunner.Domain;
using TestRunner.Infrastructure;
using static TestRunner.Events.EventHandler;

namespace TestRunner.Runners
{
    static class TestClassRunner
    {

        /// <summary>
        /// Run tests in a [TestClass]
        /// </summary>
        ///
        /// <returns>
        /// Whether everything in <paramref name="testClass"/> succeeded
        /// </returns>
        ///
        static public bool Run(TestClass testClass)
        {
            Guard.NotNull(testClass, nameof(testClass));

            TestContext.FullyQualifiedTestClassName = testClass.FullName;
            try
            {
                TestClassBeginEvent(testClass.FullName);

                bool classInitializeSucceeded = false;
                int ran = 0;
                int passed = 0;
                int failed = 0;
                int ignored = 0;
                bool classCleanupSucceeded = false;

                if (testClass.IsIgnored)
                {
                    TestClassIgnoredEvent();
                    ignored = testClass.TestMethods.Count;
                }
                else
                {
                    //
                    // Run [ClassInitialize] method
                    //
                    TestContext.TestName = testClass.TestMethods.First().Name;
                    TestContext.CurrentTestOutcome = UnitTestOutcome.InProgress;

                    classInitializeSucceeded =
                        MethodRunner.Run(
                            testClass.ClassInitializeMethod, null,
                            true,
                            null, false,
                            "[ClassInitialize]");

                    TestContext.TestName = null;
                    TestContext.CurrentTestOutcome = UnitTestOutcome.Unknown;

                    if (classInitializeSucceeded)
                    {
                        //
                        // Run [TestMethod]s
                        //
                        foreach (var testMethod in testClass.TestMethods)
                        {
                            switch(
                                TestMethodRunner.Run(
                                    testMethod,
                                    testClass.TestInitializeMethod,
                                    testClass.TestCleanupMethod,
                                    testClass))
                            {
                                case UnitTestOutcome.Passed:
                                    passed++;
                                    ran++;
                                    break;
                                case UnitTestOutcome.Failed:
                                    failed++;
                                    ran++;
                                    break;
                                case UnitTestOutcome.NotRunnable:
                                    ignored++;
                                    break;
                            }
                        }

                        //
                        // Run [ClassCleanup] method
                        //
                        classCleanupSucceeded =
                            MethodRunner.Run(
                                testClass.ClassCleanupMethod, null,
                                false,
                                null, false,
                                "[ClassCleanup]");
                    }
                }

                //
                // Print results
                //
                TestClassSummaryEvent(
                    testClass.ClassInitializeMethod != null,
                    classInitializeSucceeded,
                    testClass.TestMethods.Count,
                    ran,
                    ignored,
                    passed,
                    failed,
                    testClass.ClassCleanupMethod != null,
                    classCleanupSucceeded);

                return
                    classInitializeSucceeded &&
                    failed == 0 &&
                    classCleanupSucceeded;
            }
            finally
            {
                TestContext.FullyQualifiedTestClassName = null;
            }
        }
        
    }
}
