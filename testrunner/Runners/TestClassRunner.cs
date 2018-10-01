using System.Linq;
using TestRunner.Domain;
using TestRunner.Infrastructure;
using EventHandler = TestRunner.Events.EventHandler;

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

            bool success = false;
            bool classIgnored = false;
            bool classInitializeSucceeded = false;
            int ran = 0;
            int passed = 0;
            int failed = 0;
            int ignored = 0;
            bool classCleanupSucceeded = false;

            TestContext.FullyQualifiedTestClassName = testClass.FullName;
            EventHandler.First.TestClassBeginEvent(testClass.FullName);

            do
            {
                if (testClass.IsIgnored)
                {
                    classIgnored = true;
                    ignored = testClass.TestMethods.Count;
                    success = true;
                    break;
                }

                //
                // Run [ClassInitialize] method
                //
                TestContext.TestName = testClass.TestMethods.First().Name;
                TestContext.CurrentTestOutcome = UnitTestOutcome.InProgress;

                classInitializeSucceeded = MethodRunner.RunClassInitializeMethod(testClass.ClassInitializeMethod);

                TestContext.TestName = null;
                TestContext.CurrentTestOutcome = UnitTestOutcome.Unknown;

                if (!classInitializeSucceeded) break;

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
                classCleanupSucceeded = MethodRunner.RunClassCleanupMethod(testClass.ClassCleanupMethod);

                success = failed == 0 && classCleanupSucceeded;
            }
            while (false);

            EventHandler.First.TestClassEndEvent(
                success,
                classIgnored,
                testClass.ClassInitializeMethod != null,
                classInitializeSucceeded,
                testClass.TestMethods.Count,
                ran,
                ignored,
                passed,
                failed,
                testClass.ClassCleanupMethod != null,
                classCleanupSucceeded);

            TestContext.FullyQualifiedTestClassName = null;

            return success;
        }
        
    }
}
